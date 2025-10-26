using UnityEngine;
using FL = EntityStates.GolemMonster.FireLaser;
using AR = EntityStates.Huntress.ArrowRain;
using RoR2;
using UnityEngine.Networking;

namespace EntityStates.RevenantMod
{
    public class FireLaserBarrage : BaseFireBarrageState
    {
        [HideInInspector] public static GameObject hitEffectPrefab;
        [HideInInspector] public static GameObject tracerEffectPrefab;

        public static float laserRadius;

        public override void OnEnter()
        {
            hitEffectPrefab = FL.hitEffectPrefab;
            tracerEffectPrefab = FL.tracerEffectPrefab;
            base.OnEnter();
        }
        protected override void FireRound()
        {
            if (!isAuthority)
                return;

            var aimRay = GetAimRay();
            string muzzle = (Random.value > 0.5) ? leftMuzzleString : rightMuzzleString;

            BulletAttack ba = new BulletAttack()
            {
                owner = gameObject,
                weapon = gameObject,
                origin = aimRay.origin,
                aimVector = aimRay.direction,
                minSpread = minSpread,
                maxSpread = maxSpread,
                damage = perRoundDamage,
                force = force,
                tracerEffectPrefab = tracerEffectPrefab,
                muzzleName = muzzle,
                hitEffectPrefab = hitEffectPrefab,
                falloffModel = BulletAttack.FalloffModel.None,
                isCrit = isCrit,
                radius = laserRadius,
                smartCollision = true,
                procCoefficient = procCoefficient,
            };
            ba.Fire();
        }
    }
}