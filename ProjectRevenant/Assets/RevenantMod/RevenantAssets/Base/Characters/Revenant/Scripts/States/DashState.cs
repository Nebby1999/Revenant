using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RevenantMod
{
    public class DashState : RevenantCharacterMain
    {
        public static float duration;
        public static float baseDashSpeed;
        public static float maxDashSpeedCoefficient;
        public static Vector3 onAirMaxSpeed;
        public static float onAirDashSpeedCoefficient;

        private Vector3 fwdDirection;
        private float dashSpeed;
        private float currentXVelocity;
        private float currentZVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            if(isAuthority)
            {
                if(inputBank && characterDirection)
                {
                    fwdDirection = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;
                }
            }

            CalculateDashSpeed();

            if(characterMotor && characterDirection)
            {
                characterMotor.Motor.ForceUnground();
                Vector3 newVelocity = fwdDirection * dashSpeed;
                characterMotor.velocity = new Vector3(newVelocity.x, characterMotor.velocity.y, newVelocity.z);
            }


            Vector3 vector = characterMotor ? characterMotor.velocity : Vector3.zero;
            previousPosition = vector;
        }

        private void CalculateDashSpeed()
        {
            var baseDashSpeed = moveSpeedStat;
            var baseSprintingSpeed = characterBody.baseMoveSpeed * characterBody.sprintingSpeedMultiplier;
            var maxDashSpeed = baseSprintingSpeed * maxDashSpeedCoefficient;
            if (baseDashSpeed > maxDashSpeed)
                baseDashSpeed = maxDashSpeed;

            var speed = baseDashSpeed * DashState.baseDashSpeed;
            dashSpeed = isGrounded ? speed : speed * onAirDashSpeedCoefficient; //On air the dash is too strong, lol
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isGrounded && characterMotor)
            {
                Vector3 currentVelocity = characterMotor.velocity;
                bool xNegative = currentVelocity.x < 0;
                bool zNegative = currentVelocity.z < 0;
                float x = Mathf.Abs(currentVelocity.x);
                float z = Mathf.Abs(currentVelocity.z);
                if (x > onAirMaxSpeed.x)
                {
                    float newX = Mathf.SmoothDamp(x, onAirMaxSpeed.x, ref currentXVelocity, 0.1f);
                    currentVelocity.x = xNegative ? -newX : newX;
                }
                if (z > onAirMaxSpeed.z)
                {
                    float newZ = Mathf.SmoothDamp(z, onAirMaxSpeed.z, ref currentZVelocity, 0.1f);
                    currentVelocity.z = zNegative ? -newZ : newZ;
                }
                characterMotor.velocity = currentVelocity;
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(fwdDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            fwdDirection = reader.ReadVector3();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}