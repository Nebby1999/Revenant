using R2API;
using RevenantMod.Survivors;
using RoR2;
using UnityEngine;
using FL = EntityStates.GolemMonster.FireLaser;

namespace EntityStates.RevenantMod.Weapon
{
    public class FireLasers : BaseLauncherState
    {
        public static GameObject muzzleEffectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static float baseDuration;
        public static float laserRadius;
        public static float damageCoefficient;
        public static float force;

        private float _duration;
        public override void OnEnter()
        {
            base.OnEnter();

            muzzleEffectPrefab = FL.effectPrefab;
            hitEffectPrefab = FL.hitEffectPrefab;
            tracerEffectPrefab = FL.tracerEffectPrefab;

            _duration = baseDuration / attackSpeedStat;

            PlayLauncherAnimation();

            if(isAuthority)
            {
                Fire();
            }
        }

        private void Fire()
        {
            bool isCrit = RollCrit();
            Ray aimRay = GetAimRay();
            DamageTypeCombo damageTypeCombo = new DamageTypeCombo
            {
                damageSource = DamageSource.Secondary,
                damageType = DamageType.Generic,
                damageTypeExtended = DamageTypeExtended.Generic
            };

            foreach (string muzzleName in muzzleNameList)
            {
                BulletAttack bulletAttack = new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    damage = damageStat * damageCoefficient,
                    muzzleName = muzzleName,
                    hitEffectPrefab = hitEffectPrefab,
                    tracerEffectPrefab = tracerEffectPrefab,
                    falloffModel = BulletAttack.FalloffModel.None,
                    isCrit = isCrit,
                    maxDistance = 1024f,
                    radius = laserRadius,
                    smartCollision = true,
                    bulletCount = 1,
                    damageType = damageTypeCombo,
                    force = force
                };
                bulletAttack.AddModdedDamageType(RevenantSurvivor.jailingDamageType);
                bulletAttack.Fire();

                EffectManager.SimpleMuzzleFlash(muzzleEffectPrefab, gameObject, muzzleName, true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > _duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}