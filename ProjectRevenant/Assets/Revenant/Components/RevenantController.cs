using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Moonstorm;
using RoR2;
using RoR2.HudOverlay;
using UnityEngine.Networking;
using TMPro;
using R2API;

namespace RevenantMod.Components
{
    public class RevenantController : NetworkBehaviour, IStatItemBehavior, IOnDamageDealtServerReceiver
    {
        [Header("Cached Components")]
        public CharacterBody characterBody;

        [Header("Fuel Values")]
        [SerializeField] private float baseMaxFuel;
        [SerializeField] private float extraFuelPerExtraJump;
        [SerializeField] private float extraFuelPerExtraJumpStrength;
        [SerializeField] private float fuelRestoredPerSecond;

        [Header("UI")]
        [SerializeField] private GameObject overlayPrefab;
        public string overlayChildLocatorEntry;

        [Header("Anti Coagulant")]
        [SerializeField] private float antiCoagulantDuration;
        [SerializeField] private float fuelPerHit;

        [Header("Other")]
        [SerializeField] private float baseBleedChance;
        
        public float FuelRestoreCoefficient
        {
            get
            {
                return fuelRestoreCoefficient;
            }
            set
            {
                fuelRestoreCoefficient = Mathf.Abs(value);
            }
        }
        private float fuelRestoreCoefficient = 1;
        public float MaxFuel { get; private set; }
        public float CurrentFuel => currentFuel;
        [SyncVar] private float currentFuel;

        private int originalJumpCount;
        private float originalJumpPower;
        private OverlayController overlayController;
        private TextMeshProUGUI fuelText;
        private void Awake()
        {
            originalJumpCount = characterBody.baseJumpCount;
            originalJumpPower = characterBody.baseJumpPower;
            RecalculateMaxFuel(0, 0);
            if(NetworkServer.active)
            {
                currentFuel = MaxFuel;
            }
        }

        private void OnEnable()
        {
            OverlayCreationParams creationParams = default(OverlayCreationParams);
            creationParams.prefab = overlayPrefab;
            creationParams.childLocatorEntry = overlayChildLocatorEntry;
            overlayController = HudOverlayManager.AddOverlay(gameObject, creationParams);
            overlayController.onInstanceAdded += OnOverlayInstanceAdded;
            overlayController.onInstanceRemove += OnOverlayInstanceRemoved;
        }

        private void OnDisable()
        {
            if(overlayController != null)
            {
                overlayController.onInstanceAdded -= OnOverlayInstanceAdded;
                overlayController.onInstanceRemove -= OnOverlayInstanceRemoved;
                HudOverlayManager.RemoveOverlay(overlayController);
            }
        }

        private void FixedUpdate()
        {
            if(NetworkServer.active)
            {
                AddFuel(fuelRestoredPerSecond * fuelRestoreCoefficient * Time.fixedDeltaTime);
            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            if(fuelText)
            {
                fuelText.text = String.Format("{0} / {1}", CurrentFuel.ToString("0.00"), MaxFuel.ToString("0.00"));
            }
        }
        public void RecalculateStatsStart()
        {

        }

        public void RecalculateStatsEnd()
        {
            var newJumpCount = characterBody.maxJumpCount - originalJumpCount;
            var newJumpPower = characterBody.jumpPower - originalJumpPower;
            RecalculateMaxFuel(newJumpPower, newJumpCount);
            characterBody.jumpPower = originalJumpPower;
            characterBody.maxJumpCount = originalJumpCount;
            characterBody.bleedChance += baseBleedChance;
        }

        [Server]
        public void AddFuel(float amount)
        {
            var newFuel = currentFuel + amount;
            if (newFuel > MaxFuel)
                newFuel = MaxFuel;
            if (newFuel < 0)
                newFuel = 0;
            currentFuel = newFuel;
        }

        private void RecalculateMaxFuel(float newJumpPower, int newJumpCount)
        {
            var newMaxFuel = baseMaxFuel;
            newMaxFuel += extraFuelPerExtraJump * newJumpCount;
            newMaxFuel += extraFuelPerExtraJumpStrength * newJumpPower;
            MaxFuel = newMaxFuel;
        }
        private void OnOverlayInstanceRemoved(OverlayController arg1, GameObject arg2)
        {
            fuelText = null;
        }

        private void OnOverlayInstanceAdded(OverlayController arg1, GameObject arg2)
        {
            fuelText = arg2.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void OnDamageDealtServer(DamageReport damageReport)
        {
            var attackerIndex = damageReport.attackerBodyIndex;
            var victimBody = damageReport.victimBody;
            var damageInfo = damageReport.damageInfo;

            if (attackerIndex != characterBody.bodyIndex || !victimBody)
                return;

            if (damageInfo.procCoefficient <= 0)
                return;

            if (victimBody.HasBuff(RevenantContent.BuffDefs.bdAntiCoagulant) && victimBody.healthComponent)
            {
                float num = fuelPerHit * damageInfo.procCoefficient * victimBody.GetBuffCount(RevenantContent.BuffDefs.bdAntiCoagulant);
                RevLog.Info($"Restoring {num} fuel");
                AddFuel(num);
            }

            if(DamageAPI.HasModdedDamageType(damageInfo, DamageTypes.AntiCoagulant.dtAntiCoagulant))
            {
                victimBody.AddTimedBuff(RevenantContent.BuffDefs.bdAntiCoagulant, antiCoagulantDuration * damageReport.damageInfo.procCoefficient);
            }
        }
    }
}