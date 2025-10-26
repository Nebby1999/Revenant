using RoR2;
using UnityEngine;

namespace EntityStates.RevenantMod
{
    /// <summary>
    /// In BeginBarrage, Revenant is hoisted upwards, it then transitions into a BaseReadyBarrageState
    /// </summary>
    public abstract class BaseBeginBarrageState : BaseState
    {
        [SerializeField] public float basePrepDuration;
        [SerializeField] public float dashDuration;
        [SerializeField] public float jumpCoefficient;
        [SerializeField] public Vector3 dashVector;

        private float prepDuration;
        private CameraTargetParams.AimRequest aimRequest;
        private Vector3 worldDashVector;
        private bool beginDash;

        public override void OnEnter()
        {
            base.OnEnter();
            prepDuration = basePrepDuration / attackSpeedStat;
            if(characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = true;
                characterMotor.useGravity = false;
                characterMotor.velocity = Vector3.zero;
            }
            if(cameraTargetParams)
            {
                aimRequest = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }

            Vector3 direction = GetAimRay().direction;
            direction.y = 0f;
            direction.Normalize();
            Vector3 up = Vector3.up;
            worldDashVector = Matrix4x4.TRS(transform.position, Util.QuaternionSafeLookRotation(direction, up), new Vector3(1, 1, 1)).MultiplyPoint3x4(dashVector) - transform.position;
            worldDashVector.Normalize();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= prepDuration && !beginDash)
            {
                beginDash = true;
            }
            if (beginDash && characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = true;
                characterMotor.useGravity = false;
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion = worldDashVector * characterBody.jumpPower * jumpCoefficient * Time.fixedDeltaTime;
            }
            if (fixedAge >= dashDuration + prepDuration && isAuthority)
            {
                outer.SetNextState(GetReadyBarrageState());
            }
        }

        protected abstract BaseReadyBarrageState GetReadyBarrageState();

        public override void OnExit()
        {
            base.OnExit();
            aimRequest?.Dispose();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}