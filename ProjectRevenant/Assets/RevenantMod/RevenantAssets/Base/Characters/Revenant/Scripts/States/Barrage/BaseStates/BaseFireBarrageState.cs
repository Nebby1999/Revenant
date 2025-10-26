using RevenantMod.Survivors;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RevenantMod
{
    public abstract class BaseFireBarrageState : BaseSkillState
    {
        public static string leftMuzzleString;
        public static string rightMuzzleString;

        [SerializeField] public int baseRoundAmount;
        [SerializeField] public float fuelCostPerExtraRound;
        [SerializeField] public float firingDuration;
        [SerializeField] public float minSpread;
        [SerializeField] public float maxSpread;
        [SerializeField] public float totalDamageCoefficient;
        [SerializeField] public float force;
        [SerializeField] public float procCoefficient;
        public RevenantFuelController fuelController { get; private set; }
        public GameObject areaIndicatorInstance { get; set; }
        public CameraTargetParams.AimRequest aimRequest { get; set; }

        protected float perRoundDamage { get; private set; }
        protected bool isCrit { get; private set; }
        private int _totalRoundsToFire;
        private int _roundsFired;
        private float _timeBetweenRounds;
        private float _roundStopwatch;
        public override void OnEnter()
        {
            base.OnEnter();
            fuelController = GetComponent<RevenantFuelController>();
            if(characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = true;
                characterMotor.useGravity = false;
                characterMotor.velocity = Vector3.zero;
            }

            int roundsToFire = baseRoundAmount;
            int extraRounds = Mathf.CeilToInt(fuelController.currentFuel / fuelCostPerExtraRound);
            roundsToFire += extraRounds;
            _totalRoundsToFire = roundsToFire;

            _timeBetweenRounds = (firingDuration / _totalRoundsToFire) / attackSpeedStat;
            isCrit = RollCrit();
            float perRoundDamageCoefficient = totalDamageCoefficient / _totalRoundsToFire;
            perRoundDamage = damageStat * perRoundDamageCoefficient;
        }

        public override void Update()
        {
            base.Update();
            if (areaIndicatorInstance)
            {
                float maxDistance = 2048;
                if (Physics.Raycast(GetAimRay(), out var hitinfo, maxDistance, LayerIndex.world.mask))
                {
                    areaIndicatorInstance.transform.position = hitinfo.point;
                    areaIndicatorInstance.transform.up = hitinfo.normal;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = true;
                characterMotor.useGravity = false;
                characterMotor.velocity = Vector3.zero;
            }

            //Normally i would make the firing logic be authoritative, however, the rocket barrage utilizes orbs for performance since it creates an absurd amount of objects, as a result, the FireRound methods needs to handle authoritativeness, nevertheless, fuel spending is still authority for consistency with the other states.
            _roundStopwatch += Time.fixedDeltaTime;
            if(_roundStopwatch >= _timeBetweenRounds && _roundsFired < _totalRoundsToFire)
            {
                _roundStopwatch -= _timeBetweenRounds;
                FireRound();
                _roundsFired++;
                if(_roundsFired > baseRoundAmount && isAuthority)
                {
                    if(NetworkServer.active)
                    {
                        fuelController.SpendFuel(fuelCostPerExtraRound);
                    }
                    else
                    {
                        fuelController.CmdSpendFuel(fuelCostPerExtraRound);
                    }
                }
            }

            if(fixedAge >= firingDuration && _roundsFired >= _totalRoundsToFire && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        protected abstract void FireRound();


        public override void OnExit()
        {
            if (characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = false;
                characterMotor.useGravity = true;
            }

            if(fuelController) //This will enusre we enter into the penalty.
            {
                if(NetworkServer.active)
                {
                    fuelController.SpendFuel(fuelController.maxFuel);
                }
                else
                {
                    fuelController.CmdSpendFuel(fuelController.maxFuel);
                }
            }

            if(areaIndicatorInstance)
            {
                Destroy(areaIndicatorInstance);
            }

            aimRequest?.Dispose();
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}