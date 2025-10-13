using RevenantMod;
using RevenantMod.Survivors;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RevenantMod
{
    public class JetpackOn : BaseState
    {
        public const float DEFAULT_WORLD_GRAVITY = -30f;
        public RevenantFuelController fuelController { get; private set; }

        public GameObject jetEffect;
        public static string leftJetChildLocatorEntry;
        public static string rightJetChildLocatorEntry;

        public static float maxYVelocity;
        [Tooltip("The base thrust, this value is added to yVelocity per second.")]
        public static float baseThrustStrength;
        public static float fallingThrustMultiplier;
        [Tooltip("If we're actively thrusting, this value is deducted from the fuel each second.")]
        public static float fuelConsumedPersecond;
        public static float xzMovementBonusMultiplier = 2f;

        private float _gravityModifier;
        private float _thrust;
        private Transform tLeft;
        private Transform tRight;
        private Vector3 _xzMovementBonus;
        public override void OnEnter()
        {
            base.OnEnter();
            fuelController = GetComponent<RevenantFuelController>();
            characterBody.isSprinting = true;

            _gravityModifier = DEFAULT_WORLD_GRAVITY + Mathf.Abs(Physics.gravity.y);
            _thrust = baseThrustStrength + _gravityModifier;

            ChildLocator childLocator = GetModelChildLocator();
            if (!childLocator)
                return;

            //The skin spawns the effects on the thrusters, so this is OK
            tLeft = childLocator.FindChild(leftJetChildLocatorEntry);
            tRight = childLocator.FindChild(rightJetChildLocatorEntry);

            if(tLeft)
                tLeft.gameObject.SetActive(true);

            if(tRight)
                tRight.gameObject.SetActive(true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority || !fuelController || !inputBank || !characterMotor)
                return;

            if(NetworkServer.active)
            {
                fuelController.SpendFuel(fuelConsumedPersecond * Time.fixedDeltaTime);
            }
            else
            {
                fuelController.CmdSpendFuel(fuelConsumedPersecond * Time.fixedDeltaTime);
            }

            Vector3 computedXZBonus = Vector3.zero;
            Vector3 motorVelocity = characterMotor.velocity;

            float appliedThrust = _thrust;
            if(motorVelocity.y < 0)
            {
                appliedThrust *= fallingThrustMultiplier;
            }

            //Calculate the next y velocity. If it's less than the maxVelocity then appli it. Otherwise, do not.
            //This should stop revenant from cancelling massive y velocity going upwards by using it's jetpack.
            float nextMotorYVelocity = motorVelocity.y + (appliedThrust * Time.fixedDeltaTime);
            if(nextMotorYVelocity <= maxYVelocity)
            {
                motorVelocity.y = nextMotorYVelocity;
            }

            float movementSpeedWithBonus = characterBody.moveSpeed * xzMovementBonusMultiplier;
            computedXZBonus = inputBank.moveVector * movementSpeedWithBonus;
            computedXZBonus.y = 0;

            _xzMovementBonus = Vector3.MoveTowards(_xzMovementBonus, computedXZBonus, (characterBody.acceleration / 2) * Time.fixedDeltaTime);
            if(Mathf.Abs(motorVelocity.x) <= Mathf.Abs(_xzMovementBonus.x))
            {
                motorVelocity.x = _xzMovementBonus.x;
            }
            if(Mathf.Abs(motorVelocity.z) <= Mathf.Abs(_xzMovementBonus.z))
            {
                motorVelocity.z = _xzMovementBonus.z;
            }


            characterMotor.velocity = motorVelocity;
        }

        public override void OnExit()
        {
            if(tLeft)
                tLeft.gameObject.SetActive(false);

            if(tRight)
                tRight.gameObject.SetActive(false);

            base.OnExit();
        }
        
    }
}