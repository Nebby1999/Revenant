using RevenantMod.Survivors;
using RoR2;
using UnityEngine;
using AR = EntityStates.Huntress.ArrowRain;

namespace EntityStates.RevenantMod
{
    /// <summary>
    /// This state always comes from BaseBeginBarrage, the revenant readies to fire. Afterwards it transitions into a BaseFireBarrageState.
    /// </summary>
    public abstract class BaseReadyBarrageState : BaseSkillState
    {
        public static string leftMuzzleString;
        public static string rightMuzzleString;

        public RevenantFuelController fuelController { get; private set; }
        [SerializeField] public float maxDuration;

        private GameObject _areaIndicatorInstance;
        private CameraTargetParams.AimRequest _aimRequest;
        private bool _firing;
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

            if(cameraTargetParams)
            {
                _aimRequest = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }

            if(AR.areaIndicatorPrefab)
            {
                _areaIndicatorInstance = GameObject.Instantiate(AR.areaIndicatorPrefab);
            }
        }

        public override void Update()
        {
            base.Update();
            if (_areaIndicatorInstance)
            {
                float maxDistance = 2048;
                if (Physics.Raycast(GetAimRay(), out var hitinfo, maxDistance, LayerIndex.world.mask))
                {
                    _areaIndicatorInstance.transform.position = hitinfo.point;
                    _areaIndicatorInstance.transform.up = hitinfo.normal;
                }
            }
        }

        // I originally based this off huntress' special, the main issue that made me separate the main barrage state into "Ready" and "Fire" is because the authority checked for the input, but the rocket version of the special utilizes _orbs_, which are server side exclusive.
        //So as a result, each "firing" state will handle the attack with how it should be dealt... BulletAttack under Authority for the Laser, and Orb under Server for the Rocket.
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = true;
                characterMotor.useGravity = false;
                characterMotor.velocity = Vector3.zero;
            }

            if (isAuthority && inputBank)
            {
                if (!_firing && skillLocator && skillLocator.utility.IsReady() && inputBank.skill3.justPressed)
                {
                    outer.SetNextStateToMain();
                }
                else if (!_firing && (fixedAge >= maxDuration || inputBank.skill1.justPressed || inputBank.skill4.justPressed))
                {
                    _firing = true;
                }

                if (_firing)
                {
                    outer.SetNextState(GetFiringState());
                }
            }
        }

        protected abstract BaseFireBarrageState GetFiringState();

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);
            if(nextState is BaseFireBarrageState baseFireBarrageState)
            {
                baseFireBarrageState.areaIndicatorInstance = _areaIndicatorInstance;
                baseFireBarrageState.aimRequest = _aimRequest;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}