using Revenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Moonstorm;

namespace EntityStates.Revenant
{
    public class DashState : RevenantCharacterMain
    {
        private const string tkn = "REV_REVENANT_UTIL_DASH_DESC";
        public static float duration;
        public static float baseDashSpeed;
        public static float maxDashSpeedCoefficient;
        public static Vector3 onAirMaxSpeed;
        public static float onAirDashSpeedCoefficient;

        [TokenModifier(tkn, StatTypes.Default, 0)]
        public static float TokenModifier_FuelConsumed => RevenantAssets.LoadAsset<RevenantFuelSkillDef>("RevenantBodyDash").baseFuelCost;

        private Vector3 forwardDirection;
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
                    forwardDirection = (inputBank.moveVector == Vector3.zero ? characterDirection.forward : inputBank.moveVector).normalized;
                }
            }

            CalculateDashSpeed();

            if(characterMotor && characterDirection)
            {
                characterMotor.Motor.ForceUnground();
                Vector3 newVelocityXZ = forwardDirection * dashSpeed;
                characterMotor.velocity = new Vector3(newVelocityXZ.x, characterMotor.velocity.y, newVelocityXZ.z);
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
                if(x > onAirMaxSpeed.x)
                {
                    float newX = Mathf.SmoothDamp(x, onAirMaxSpeed.x, ref currentXVelocity, 0.1f);
                    currentVelocity.x = xNegative ? -newX : newX;
                }
                if(z > onAirMaxSpeed.z)
                {
                    float newZ = Mathf.SmoothDamp(z, onAirMaxSpeed.z, ref currentZVelocity, 0.1f);
                    currentVelocity.z = zNegative ? -newZ : newZ;
                }
                characterMotor.velocity = currentVelocity;
            }
            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            forwardDirection = reader.ReadVector3();
        }
    }
}