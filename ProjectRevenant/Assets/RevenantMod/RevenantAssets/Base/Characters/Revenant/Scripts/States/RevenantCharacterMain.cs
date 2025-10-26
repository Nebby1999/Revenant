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
        private RevenantFuelController _fuelController;
        private EntityStateMachine _jetpackStateMachine;

        private bool _jumpButtonState;
        private bool _isInInitialJump;
        private bool _nextStateIsDashState;
        public override void OnEnter()
        {
            base.OnEnter();
            _fuelController = GetComponent<RevenantFuelController>();

            if (_fuelController)
            {
                _jetpackStateMachine = _fuelController.jetpackStateMachine;
            }
        }

        public override void ProcessJump()
        {
            //We'll check if we're starting a jump.
            if(isAuthority && isGrounded && jumpInputReceived)
            {
                _isInInitialJump = true;
            }
            base.ProcessJump();

            //are we not the authority? bail out, also proceed to check if we're not gounded, and we have our jetpack state machine
            if (!isAuthority || !_jetpackStateMachine)
                return;

            bool isAlreadyTurnedOn = _jetpackStateMachine.state.GetType() == typeof(JetpackOn);
            //If we're grounded, ensure we go back to idle.
            if(isGrounded && isAlreadyTurnedOn)
            {
                _jetpackStateMachine.SetNextState(new Idle());
                return;
            }

            _jumpButtonState = base.inputBank.jump.down;

            //Once we start goin down, we disable the "_isInInitialJump", which will enable our jetpack.
            if(characterMotor.velocity.y < 0)
            {
                _isInInitialJump = false;
            }

            //If we've started going down, we can turn on our jetpack.
            bool shouldTurnOn = _jumpButtonState && !_isInInitialJump && !characterMotor.isGrounded && _fuelController.hasFuel && !_fuelController.isInPenalty;

            if(shouldTurnOn && !isAlreadyTurnedOn)
            {
                _jetpackStateMachine.SetNextState(new JetpackOn());
            }
            if(!shouldTurnOn && isAlreadyTurnedOn)
            {
                _jetpackStateMachine.SetNextState(new Idle());
            }
        }


        public override void OnExit()
        {
            if(isAuthority && _jetpackStateMachine && !_nextStateIsDashState)
            {
                _jetpackStateMachine.SetNextState(new Idle());
            }
            base.OnExit();
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);

            if(nextState is DashState dashState)
            {
                _nextStateIsDashState = true;
            }
        }
    }
}