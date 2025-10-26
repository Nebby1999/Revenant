using MSU;
using R2API;
using RoR2;
using RoR2.ConVar;
using RoR2.HudOverlay;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RevenantMod.Survivors
{
    public class RevenantFuelController : NetworkBehaviour, IStatItemBehavior, IOnDamageDealtServerReceiver
    {
        public static BoolConVar infiniteFuel = new BoolConVar("revenantmod_infinite_fuel", ConVarFlags.ExecuteOnServer, "0", "toggles infinite fuel");
        public CharacterBody characterBody { get; private set; }

        public EntityStateMachine jetpackStateMachine => _jetpackStateMachine;
        [SerializeField] private EntityStateMachine _jetpackStateMachine;

        [Header("Fuel Values")]
        [SerializeField] private float _baseFuel;
        [SerializeField] private float _extraFuelPerExtraJump;
        [SerializeField] private float _extraFuelPerExtraJumpStrength;
        [SerializeField] private float _baseFuelRestoredPerSecond;
        [SerializeField] private float _levelFuelRestoredPerSecond;
        [SerializeField] private float _fuelPenaltyRestorationCoefficient;

        [Header("UI")]
        [SerializeField] private GameObject _overlayPrefab;
        [SerializeField] private string _overlayHUDChildLocatorEntry;
        [SerializeField] private float _fuelPenaltyColorTimer;

        /// <summary>
        /// The maximum amount of fuel, this is calculated dynamically based off extra jump strength and extra jump count.
        /// </summary>
        public float maxFuel { get; private set; }

        /// <summary>
        /// The current amount of fuel restored per second, this is calculated dynamically based off wether we're in penalty mode, alongside other factors.
        /// </summary>
        public float fuelRestoredPerSecond { get; private set; }
        public bool hasFuel => currentFuel > 0;
        public float currentFuel => _currentFuel;
        [SyncVar]
        private float _currentFuel;

        public bool isInPenalty => _isInPenalty;
        [SyncVar(hook = nameof(OnPenaltySet))]
        private bool _isInPenalty;
        private bool _isPendingPenaltyServer;

        private int _extraJumpCount;
        private float _extraJumpPower;

        private int _originalJumpCount;
        private float _originalJumpPower;

        private OverlayController _overlayController;
        private HGTextMeshProUGUI _fuelText;
        private Color _textColor;
        private int _colorSwitcher;
        private float _fuelPenaltyTimerStopwatch;

        [SyncVar(hook = nameof(OnFuckWeaverSet))]
        private HurtBoxReference _fuckWeaver;

        private void OnFuckWeaverSet(HurtBoxReference fuckWeaver)
        {
            _fuckWeaver = fuckWeaver;
            RevLog.Info("FuckWeaver reference is set to " + fuckWeaver.ResolveHurtBox());
        }
        #region Messages
        private void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            if (!characterBody)
                return;

            _originalJumpCount = characterBody.baseJumpCount;
            _originalJumpPower = characterBody.baseJumpPower;

            //As we're setting the initial state, we should just recalculate the base max fuel.
            RecalculateFuelStats();

            if(NetworkServer.active)
            {
                _currentFuel = maxFuel;
            }
        }

        private void OnEnable()
        {
            OverlayCreationParams creationParams = new OverlayCreationParams
            {
                prefab = _overlayPrefab,
                childLocatorEntry = _overlayHUDChildLocatorEntry,
            };

            _overlayController = HudOverlayManager.AddOverlay(gameObject, creationParams);
            _overlayController.onInstanceAdded += OnOverlayInstanceAdded;
            _overlayController.onInstanceRemove += OnOverlayInstanceRemoved;
        }

        private void OnDisable()
        {
            if(_overlayController != null)
            {
                _overlayController.onInstanceAdded -= OnOverlayInstanceAdded;
                _overlayController.onInstanceRemove -= OnOverlayInstanceRemoved;
                HudOverlayManager.RemoveOverlay(_overlayController);
            }
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (_isPendingPenaltyServer && !_isInPenalty)
                {
                    SetIsInPenalty(true);
                }

                AddFuel(fuelRestoredPerSecond * Time.fixedDeltaTime);

                //Remove penalty once we're over 100 fuel, if somehow our max fuel is less than base fuel, then just use base fuel.
                if (_isInPenalty && currentFuel >= Mathf.Min(_baseFuel, maxFuel))
                {
                    SetIsInPenalty(false);
                }
            }
        }

        [Server]
        private void SetIsInPenalty(bool isInPenaltyValue)
        {
            _isPendingPenaltyServer = false;
            _isInPenalty = isInPenaltyValue;
            RecalculateFuelStats();
        }

        //This will only get called if we call the proper method (SetIsInPenalty(bool)), that method already recalculates the stats on the server side, so no need to recalculate again.
        private void OnPenaltySet(bool penaltyState)
        {
            _isInPenalty = penaltyState;

            if(!NetworkServer.active)
                RecalculateFuelStats();
        }

        private void Update()
        {
            UpdateUI();
        }
        #endregion

        #region UI
        private void OnOverlayInstanceRemoved(OverlayController arg1, GameObject arg2)
        {
            _fuelText = null;
        }

        private void OnOverlayInstanceAdded(OverlayController arg1, GameObject arg2)
        {
            _fuelText = arg2.GetComponentInChildren<HGTextMeshProUGUI>();
        }
        private void UpdateUI()
        {
            if (!_fuelText)
                return;

            if (_fuelText)
            {
                _fuelText.text = String.Format("{0} / {1}", currentFuel.ToString("0.00"), maxFuel.ToString("0.00"));
            }

            Color newColor = _textColor;

            if(_isInPenalty == false)
            {
                newColor = Color.white;
            }
            else
            {
                _fuelPenaltyTimerStopwatch += Time.deltaTime;
                if (_fuelPenaltyTimerStopwatch >= _fuelPenaltyColorTimer)
                {
                    _colorSwitcher++;
                    _fuelPenaltyTimerStopwatch -= _fuelPenaltyColorTimer;
                }
                newColor = (_colorSwitcher % 2 == 0) ? Color.red : Color.white;
            }

            if (newColor != _textColor)
            {
                _textColor = newColor;
                _fuelText.color = _textColor;
            }
        }
        #endregion

        private void RecalculateFuelStats()
        {
            float levelMinusOne = characterBody.level - 1;
            //Max fuel calculation
            var newMaxFuel = _baseFuel;
            newMaxFuel += _extraFuelPerExtraJump * _extraJumpCount;
            newMaxFuel += _extraFuelPerExtraJumpStrength * _extraJumpPower;
            maxFuel = newMaxFuel;

            //Fuel restoration calculation
            var newFuelRestoredPerSecond = _baseFuelRestoredPerSecond;
            var levelFuelRestoredPerSecond = _levelFuelRestoredPerSecond * levelMinusOne;
            newFuelRestoredPerSecond += levelFuelRestoredPerSecond;
            newFuelRestoredPerSecond *= _isInPenalty ? _fuelPenaltyRestorationCoefficient : 1f;
            fuelRestoredPerSecond = newFuelRestoredPerSecond;
        }

        /// <summary>
        /// Adds fuel to the Revenant Jetpack. Runs only on the Server.
        /// </summary>
        /// <param name="amount">The amount of fuel to add.</param>
        [Server]
        public void AddFuel(float amount)
        {
            AddFuelInternal(amount);
        }

        /// <summary>
        /// Command version for adding fuel under authority.
        /// </summary>
        /// <param name="v"></param>
        [Command]
        public void CmdAddFuel(float amount)
        {
            AddFuelInternal(amount);
        }

        public void SpendFuel(float amount)
        {
            AddFuelInternal(-amount);
        }

        [Command]
        public void CmdSpendFuel(float amount)
        {
            AddFuelInternal(-amount);
        }

        [Server]
        private void AddFuelInternal(float amount)
        {
            if(infiniteFuel.value)
            {
                _currentFuel = maxFuel;
                return;
            }

            var newFuelUnclamped = currentFuel + amount;

            //If we've ran out of fuel, and we're not in penalty already, begin pending the penalty.
            if (newFuelUnclamped < 0 && !_isInPenalty)
            {
                _isPendingPenaltyServer = true;
            }
            _currentFuel = Mathf.Clamp(newFuelUnclamped, 0, maxFuel);
        }


        public void RecalculateStatsEnd()
        {
           //Jump count and JumpPower get transformed into increased fuel with Revenant, as such, we're going to calculate the delta between the original values and the recalculated values.
            _extraJumpCount = characterBody.maxJumpCount - _originalJumpCount;
            _extraJumpPower = characterBody.jumpPower - _originalJumpPower;

            //After we get the delta, set back the actual values.
            characterBody.jumpPower = _originalJumpPower;
            characterBody.maxJumpCount = _originalJumpCount;

            //And recalculate our fuel
            RecalculateFuelStats();
        }

        public void RecalculateStatsStart()
        {
            //Nothin.
        }

        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if(!damageReport.victimBody || !damageReport.victimBody.mainHurtBox)
            {
                return;
            }
            _fuckWeaver = HurtBoxReference.FromHurtBox(damageReport.victimBody.AsValidOrNull()?.mainHurtBox);
        }
    }
}