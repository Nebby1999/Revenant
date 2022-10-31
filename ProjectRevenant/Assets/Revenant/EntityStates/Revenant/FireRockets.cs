using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EntityStates.Revenant.Weapon
{
    public class FireRockets : RevenantRocketLauncherBaseState
    {
        public static GameObject rocketPrefab;
        public static GameObject guidedRocketPrefab;
        public static float damageCoef;
        public static float force;

        private MuzzleTransforms muzzleTransforms;
        private float damage;
        private bool crit;
        public override void OnEnter()
        {
            base.OnEnter();
            damage = damageStat * damageCoef;
            crit = RollCrit();
            muzzleTransforms = GetMuzzle();
            SpawnMuzzleEffect(muzzleTransforms);
            FireRocket();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public void FireRocket()
        {
            FireProjectileInfo info = CreateProjectileFindo();
            if(isAuthority)
            {
                switch(CurrentMuzzle)
                {
                    case Muzzle.Left:
                        info.position = muzzleTransforms.leftMuzzle.position;
                        ProjectileManager.instance.FireProjectile(info);
                        break;
                    case Muzzle.Right:
                        info.position = muzzleTransforms.rightMuzzle.position;
                        ProjectileManager.instance.FireProjectile(info);
                        break;
                    case Muzzle.Both:
                        info.position = muzzleTransforms.leftMuzzle.position;
                        ProjectileManager.instance.FireProjectile(info);
                        info.position = muzzleTransforms.rightMuzzle.position;
                        ProjectileManager.instance.FireProjectile(info);
                        break;
                }
            }
        }


        public FireProjectileInfo CreateProjectileFindo()
        {
            FireProjectileInfo pInfo = new FireProjectileInfo();

            pInfo.projectilePrefab = CurrentMuzzle == Muzzle.Both ? guidedRocketPrefab : rocketPrefab;
            pInfo.position = aimRay.origin;
            pInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
            pInfo.owner = gameObject;
            pInfo.damage = damage;
            pInfo.force = force;
            pInfo.crit = crit;
            pInfo.damageColorIndex = DamageColorIndex.Default;
            return pInfo;
        }
    }
}
