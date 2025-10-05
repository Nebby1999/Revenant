using MSU;
using R2API;
using RoR2;
using RoR2.HudOverlay;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RevenantMod.Survivors
{
    public class RevenantJetpackController : NetworkBehaviour, IStatItemBehavior
    {
        public CharacterBody characterBody { get; private set; }

        [Header("Fuel Values")]
        [SerializeField] private float _baseFuel;
        [SerializeField] private float _extraFuelPerExtraJump;
        [SerializeField] private float _extraFuelPerExtraJumpStrength;
        [SerializeField] private float _fuelRestoredPerSecond;

        [Header("UI")]
        [SerializeField] private GameObject _overlayPrefab;
        [SerializeField] private string _overlayHUDChildLocatorEntry;

        public bool hasFuel => currentFuel > 0;
        public float maxFuel { get; private set; }
        public float currentFuel => _currentFuel;
        [SyncVar]
        private float _currentFuel;

        private int _originalJumpCount;
        private float _originalJumpPower;
        private OverlayController _overlayController;
        private HGTextMeshProUGUI _fuelText;

        #region Messages
        private void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            if (!characterBody)
                return;

            _originalJumpCount = characterBody.baseJumpCount;
            _originalJumpPower = characterBody.baseJumpPower;

            //As we're setting the initial state, we should just recalculate the base max fuel, the moment recalcstats runs we'll get the proper values.
            RecalculateMaxFuel(0, 0);

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
            if(NetworkServer.active)
            {
                AddFuel(_fuelRestoredPerSecond * Time.fixedDeltaTime);
            }
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
            if (_fuelText)
            {
                _fuelText.text = String.Format("{0} / {1}", currentFuel.ToString("0.00"), maxFuel.ToString("0.00"));
            }
        }
        #endregion

        private void RecalculateMaxFuel(int extraJumpCount, float extraJumpPower)
        {
            var newMaxFuel = _baseFuel;
            newMaxFuel += _extraFuelPerExtraJump * extraJumpCount;
            newMaxFuel += _extraFuelPerExtraJumpStrength * extraJumpPower;
            maxFuel = newMaxFuel;
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
            var newFuel = Mathf.Clamp(currentFuel + amount, 0, maxFuel);
            _currentFuel = newFuel;
        }


        public void RecalculateStatsEnd()
        {
            //These when the character first spawns is technically 0, after that, the difference between the original amount with the new one is the values that we must use to recalculate the fuel.
            var jumpCountAddition = characterBody.maxJumpCount - _originalJumpCount;
            var jumpPowerAddition = characterBody.jumpPower - _originalJumpPower;

            RecalculateMaxFuel(jumpCountAddition, jumpPowerAddition);

            characterBody.jumpPower = _originalJumpPower;
            characterBody.maxJumpCount = _originalJumpCount;
        }

        public void RecalculateStatsStart()
        {
            //Nothin.
        }

    }
}