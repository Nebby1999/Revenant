using Revenant.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Revenant
{
    public class RevenantCharacterMain : GenericCharacterMain
    {
        public static float jetpackVelocity;
        public static float fuelConsumedPerSecond;
        
        public RevenantJetpackController JetpackController { get; private set; }
        public bool HasFuel => JetpackController.CurrentFuel > 0;
        public override void OnEnter()
        {
            base.OnEnter();
            JetpackController = GetComponent<RevenantJetpackController>();
        }
        public override void ProcessJump()
        {
            base.ProcessJump();
            if(hasCharacterMotor && hasInputBank && isAuthority)
            {
                bool jumpIsDown = inputBank.jump.down;
                bool notGrounded = !isGrounded;
                JetpackController.ConsumingFuel = jumpIsDown;
                if(jumpIsDown && notGrounded && HasFuel)
                {
                    float y = characterMotor.velocity.y;
                    y += jetpackVelocity * Time.fixedDeltaTime;
                    characterMotor.velocity = new Vector3(characterMotor.velocity.x, y, characterMotor.velocity.z);
                    if(NetworkServer.active)
                        JetpackController.AddFuel(-fuelConsumedPerSecond * Time.fixedDeltaTime);
                }
            }
        }
    }
}
