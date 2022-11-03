using Revenant.Components;
using RoR2;
using RoR2.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FL = EntityStates.GolemMonster.FireLaser;

namespace EntityStates.Revenant.RocketLauncher
{
    public abstract class RevenantRocketLauncherBaseState : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public struct MuzzleTransforms
        {
            public Transform leftMuzzle;
            public string leftMuzzleChildLocatorName;
            public Transform rightMuzzle;
            public string rightMuzzleChildLocatorName;
        }
        public enum Muzzle
        {
            Right = 0,
            Left = 1,
            Both = 2,
        }
        [SerializeField]
        public string leftMuzzle;
        [SerializeField]
        public string rightMuzzle;
        [SerializeField]
        public float baseDuration;
        [SerializeField]
        public GameObject muzzleEffectPrefab;
        protected Muzzle CurrentMuzzle { get; private set; }
        protected float duration;
        protected Ray aimRay;

        public RevenantController JetpackController { get; private set; }
        public bool HasFuel => JetpackController.CurrentFuel > 0;
        public void SetStep(int i) => CurrentMuzzle = (Muzzle)i;
        public override void OnEnter()
        {
            base.OnEnter();
            JetpackController = GetComponent<RevenantController>();
            duration = baseDuration / attackSpeedStat;
            aimRay = GetAimRay();
            StartAimMode(aimRay, 2f);
        }

        protected MuzzleTransforms GetMuzzle()
        {
            switch(CurrentMuzzle)
            {
                case Muzzle.Left:
                    return new MuzzleTransforms
                    {
                        leftMuzzle = FindModelChild(leftMuzzle),
                        leftMuzzleChildLocatorName = leftMuzzle
                    };
                case Muzzle.Right:
                    return new MuzzleTransforms
                    {
                        rightMuzzle = FindModelChild(rightMuzzle),
                        rightMuzzleChildLocatorName = rightMuzzle,
                    };
                case Muzzle.Both:
                    return new MuzzleTransforms
                    {
                        leftMuzzle = FindModelChild(leftMuzzle),
                        leftMuzzleChildLocatorName = leftMuzzle,
                        rightMuzzle = FindModelChild(rightMuzzle),
                        rightMuzzleChildLocatorName = rightMuzzle,
                    };
                default:
                    return default(MuzzleTransforms);
            }
        }
        protected void SpawnMuzzleEffect(MuzzleTransforms muzzleTransforms)
        {
            if (muzzleEffectPrefab)
            {
                switch (CurrentMuzzle)
                {
                    case Muzzle.Left:
                        EffectManager.SimpleMuzzleFlash(muzzleEffectPrefab, gameObject, muzzleTransforms.leftMuzzleChildLocatorName, false);
                        break;
                    case Muzzle.Right:
                        EffectManager.SimpleMuzzleFlash(muzzleEffectPrefab, gameObject, muzzleTransforms.rightMuzzleChildLocatorName, false);
                        break;
                    case Muzzle.Both:
                        EffectManager.SimpleMuzzleFlash(muzzleEffectPrefab, gameObject, muzzleTransforms.leftMuzzleChildLocatorName, false);
                        EffectManager.SimpleMuzzleFlash(muzzleEffectPrefab, gameObject, muzzleTransforms.rightMuzzleChildLocatorName, false);
                        break;
                }
            }
        }

    }
}
