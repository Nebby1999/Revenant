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

namespace Revenant.Components
{
    public class RevenantJetpackController : NetworkBehaviour, IStatItemBehavior
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

        public bool ConsumingFuel { get; set;  }
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
                if(!ConsumingFuel)
                {
                    AddFuel(fuelRestoredPerSecond * Time.fixedDeltaTime);
                }
            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            if(fuelText)
            {
                fuelText.text = String.Format("{0} / {1}", CurrentFuel, MaxFuel);
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
    }
}