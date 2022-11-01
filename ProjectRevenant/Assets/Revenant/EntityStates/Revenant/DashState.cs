using Revenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Revenant
{
    public class DashState : RevenantCharacterMain
    {
        public static float duration;
        public static float baseDashSpeed;
        public static float maxDashSpeedCoefficient;
        public static Vector3 onAirMaxSpeed = new Vector3(20, 0, 20);
        public static float onAirDashSpeedCoefficient;

        private Vector3 forwardDirection;
        private float dashSpeed;
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
            if (!isGrounded)
            {

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