using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EntityStates.Revenant.RocketLauncher
{
    public abstract class BaseRocketLauncherBarrage : RevenantRocketLauncherBaseState
    {
        [SerializeField] public float maxDuration;

        private CameraTargetParams.AimRequest aimRequest;
        private bool firing;
        public override void OnEnter()
        {
            base.OnEnter();
            if (characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = true;
                characterMotor.useGravity = false;
                characterMotor.velocity = Vector3.zero;
            }
            if (cameraTargetParams)
            {
                aimRequest = cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
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

            if (isAuthority && inputBank)
            {
                if(!firing && skillLocator && skillLocator.utility.IsReady() && inputBank.skill3.justPressed)
                {
                    outer.SetNextStateToMain();
                }
                else if(!firing && (fixedAge >= maxDuration || inputBank.skill1.justPressed || inputBank.skill4.justPressed))
                {
                    firing = true;
                }
                if(firing)
                {
                    DoFire();
                }
            }
        }

        protected virtual void DoFire() { }

        public override void OnExit()
        {
            if (characterMotor)
            {
                characterMotor.disableAirControlUntilCollision = false;
                characterMotor.useGravity = true;
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
