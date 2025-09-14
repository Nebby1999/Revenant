using RevenantMod.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Revenant
{
    public class RevenantCharacterMain : GenericCharacterMain
    {
        public const float defaultWorldGravity = -30;
        public static float baseJetpackUpwardThrust;
        public static float maxJetpackUpwardThrust;
        public static float fallingVelocityBoostCoefficient;
        public static float fuelConsumedPerSecond;
        public static float timeBeforeJumpBecomesThrust;

        private float gravityModifier;
        private float upwardThrust;
        private float timer;
        public RevenantController JetpackController { get; private set; }
        public bool HasFuel => JetpackController.CurrentFuel > 0;
        
        public override void OnEnter()
        {
            base.OnEnter();
            JetpackController = GetComponent<RevenantController>();

            //This makes it so the upward thrust is not as effective in low gravity scenes, such as Moon 2.
            //Moon 2 has a gravity of -20, so -30 + 20 = -10, which means that the thrust will be baseJetpackUpwardThrust - 10.
            gravityModifier = defaultWorldGravity + Mathf.Abs(Physics.gravity.y);
            upwardThrust = baseJetpackUpwardThrust + gravityModifier;
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            timer = isGrounded ? 0 : timer + Time.fixedDeltaTime;
            JetpackController.FuelRestoreCoefficient = isGrounded ? 1 : 0.5f;
            if(hasCharacterMotor && hasInputBank && isAuthority)
            {
                bool jumpIsDown = inputBank.jump.down;
                bool notGrounded = !isGrounded;
                if(jumpIsDown && notGrounded && HasFuel && timer > timeBeforeJumpBecomesThrust)
                {
                    float y = characterMotor.velocity.y;
                    y += upwardThrust * (y < 0 ? fallingVelocityBoostCoefficient : 1) * Time.fixedDeltaTime; //Are we falling? increase thrust.
                    y = Mathf.Min(y, maxJetpackUpwardThrust);
                    characterMotor.velocity = new Vector3(characterMotor.velocity.x, y, characterMotor.velocity.z);
                    if(NetworkServer.active)
                        JetpackController.AddFuel(-fuelConsumedPerSecond * Time.fixedDeltaTime);
                }
            }
        }
 
    }
}
