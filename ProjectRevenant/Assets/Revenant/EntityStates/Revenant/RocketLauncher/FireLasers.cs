using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm;
using UnityEngine;
using FL = EntityStates.GolemMonster.FireLaser;

namespace EntityStates.Revenant.RocketLauncher
{
    public class FireLasers : RevenantRocketLauncherBaseState
    {
        private const string tkn = "REV_REVENANT_PRIMARY_LASER_DESC";

        [HideInInspector]
        public static GameObject hitEffectPrefab;
        [HideInInspector]
        public static GameObject tracerEffectPrefab;
        public static float laserRadius;
        [TokenModifier(tkn, StatTypes.MultiplyByN, 0, "100")]
        public static float damageCoef;
        public static float force;

        private MuzzleTransforms muzzleTransforms;
        public override void OnEnter()
        {
            base.OnEnter();
            muzzleEffectPrefab = FL.effectPrefab;
            hitEffectPrefab = FL.hitEffectPrefab;
            tracerEffectPrefab = FL.tracerEffectPrefab;

            muzzleTransforms = GetMuzzle();
            SpawnMuzzleEffect(muzzleTransforms);
            FireBullet();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public void FireBullet()
        {
            if(isAuthority)
            {
                switch(CurrentMuzzle)
                {
                    case Muzzle.Left:
                        CreateBulletAttack(muzzleTransforms.leftMuzzleChildLocatorName).Fire();
                        break;
                    case Muzzle.Right:
                        CreateBulletAttack(muzzleTransforms.rightMuzzleChildLocatorName).Fire();
                        break;
                    case Muzzle.Both:
                        var ba = CreateBulletAttack(muzzleTransforms.leftMuzzleChildLocatorName);
                        ba.Fire();

                        ba = CreateBulletAttack(muzzleTransforms.rightMuzzleChildLocatorName);
                        ba.Fire();
                        break;
                }
            }
        }
        public BulletAttack CreateBulletAttack(string muzzle)
        {
            BulletAttack bulletAttack = new BulletAttack();
            bulletAttack.owner = base.gameObject;
            bulletAttack.weapon = base.gameObject;
            bulletAttack.origin = aimRay.origin;
            bulletAttack.aimVector = aimRay.direction;
            bulletAttack.minSpread = 0f;
            bulletAttack.maxSpread = 0;
            bulletAttack.damage = damageCoef * damageStat;
            bulletAttack.force = force;
            bulletAttack.tracerEffectPrefab = tracerEffectPrefab;
            bulletAttack.muzzleName = muzzle;
            bulletAttack.hitEffectPrefab = hitEffectPrefab;
            bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
            bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
            bulletAttack.radius = laserRadius;
            bulletAttack.smartCollision = true;
            return bulletAttack;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
