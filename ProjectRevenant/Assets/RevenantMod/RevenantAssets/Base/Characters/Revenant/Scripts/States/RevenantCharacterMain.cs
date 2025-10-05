using HG;
using RevenantMod;
using RevenantMod.Survivors;
using RoR2;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RevenantMod
{
    public class RevenantCharacterMain : GenericCharacterMain
    {
        public const float DEFAULT_WORLD_GRAVITY = -30;
        public RevenantJetpackController JetpackController { get; private set; }

        public static float jetpackEnableFromJumpTime;
        public static float maxYVelocity;
        [Tooltip("The base thrust, this value is added to yVelocity per second.")]
        public static float baseThrust;
        [Tooltip("As long as yVelocity < 0, this is added to baseThrust.")]
        public static float fallingThrustBoost;
        [Tooltip("If we're actively thrusting, this value is deducted from the fuel each second.")]
        public static float fuelConsumedPersecond;
        public static float xzMovementBonusMultiplier = 1.1f;

        private float _gravityModifier;
        private float _thrust;
        private float _jetpackEnableTimer;
        private Vector3 _xzMovementBonus;
        public override void OnEnter()
        {
            base.OnEnter();

            JetpackController = GetComponent<RevenantJetpackController>();

            //This is done so that the thrust is equal to the gravity of the area. The default world gravity is -30, if you add the current gravity's absolute value to the default world gravity we can get a reasonable thrust modifier to make the thrust feel "good" on different scenes. Specially useful for moon2
            _gravityModifier = DEFAULT_WORLD_GRAVITY + Mathf.Abs(Physics.gravity.y);
            _thrust = baseThrust + _gravityModifier;
            stringBuilder = new StringBuilder();
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            stringBuilder.Clear();

            if (!JetpackController && !hasInputBank && !hasCharacterMotor)
                return;

            if (!isAuthority)
                return;

            if (isGrounded)
            {
                _jetpackEnableTimer = 0;
                _xzMovementBonus = Vector3.zero;
            }
            else
            {
                _jetpackEnableTimer += Time.fixedDeltaTime;
            }

            //Calculate the movement speed of the character, this will come in useful for the xz mobility boost.
            float movementSpeed = characterBody.moveSpeed * (characterBody.isSprinting ? characterBody.sprintingSpeedMultiplier : 1);

            Vector3 computedXzBonus = Vector3.zero;
            Vector3 motorVelocity = characterMotor.velocity;
            //We're down and past the timer, enable jetpack controls.
            if(_jetpackEnableTimer > jetpackEnableFromJumpTime && JetpackController.hasFuel && inputBank.jump.down)
            {
                
                //Vertical boost
                float thrust = _thrust;
                float motorYVelocity = motorVelocity.y;
                
                if (motorYVelocity < 0)
                {
                    //add a boost to our thurst if we're actively falling
                    thrust *= fallingThrustBoost;
                }

                //Calculate the next velocity. If its less than the maxVelocity then apply it. Otherwise dont.
                //This should stop revenant from cancelling massive Y velocity going upwards by using its jetpack.
                float nextMotorYVelocity = motorYVelocity + (thrust) * Time.fixedDeltaTime;
                if(nextMotorYVelocity <= maxYVelocity)
                {
                    motorVelocity.y = nextMotorYVelocity;
                }

                //Directional Boost
                float movementSpeedWithBonus = movementSpeed * xzMovementBonusMultiplier;
                computedXzBonus = moveVector * movementSpeedWithBonus;
                computedXzBonus.y = 0;


                if(NetworkServer.active)
                {
                    JetpackController.SpendFuel(fuelConsumedPersecond * Time.fixedDeltaTime);
                }
                else
                {
                    JetpackController.CmdSpendFuel(fuelConsumedPersecond * Time.fixedDeltaTime);
                }
            }

            _xzMovementBonus = Vector3.MoveTowards(_xzMovementBonus, computedXzBonus, characterBody.acceleration * Time.fixedDeltaTime);
            characterMotor.velocity = motorVelocity;
        }
    }
}