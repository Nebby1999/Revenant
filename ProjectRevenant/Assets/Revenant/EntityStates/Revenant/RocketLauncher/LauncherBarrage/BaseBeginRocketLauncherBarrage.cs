using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Revenant.RocketLauncher
{
    public abstract class BaseBeginRocketLauncherBarrage : RevenantRocketLauncherBaseState
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
            if(fixedAge >= prepDuration && !beginDash)
            {
                beginDash = true;
            }
            if(beginDash && characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = true;
                characterMotor.useGravity = false;
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion = worldDashVector * characterBody.jumpPower * jumpCoefficient * Time.fixedDeltaTime;
            }
            if(fixedAge >= dashDuration + prepDuration && isAuthority)
            {
                outer.SetNextState(InstantiateNextState());
            }
        }

        protected virtual RevenantRocketLauncherBaseState InstantiateNextState() => null;
        public override void OnExit()
        {
            //characterBody.RemoveBuff(buffDef);
            aimRequest?.Dispose();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
