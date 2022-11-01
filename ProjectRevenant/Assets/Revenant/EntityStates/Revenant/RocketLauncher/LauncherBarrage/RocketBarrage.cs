using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Moonstorm;

namespace EntityStates.Revenant.RocketLauncher
{
    public class RocketBarrage : BaseRocketLauncherBarrage
    {
        private const string tkn = "REV_REVENANT_SPECIAL_ROCKETBARRAGE_DESC";
        [HideInInspector]
        public static GameObject areaIndicatorPrefab = EntityStates.Huntress.ArrowRain.areaIndicatorPrefab;
        public static GameObject rocketPrefab;
        public static GameObject guidedRocketPrefab;
        [TokenModifier(tkn, StatTypes.Default, 3)]
        public static float guidedChance;
        [TokenModifier(tkn, StatTypes.Default, 0)]
        public static int rocketAmount;
        public static float firingDuration;
        [TokenModifier(tkn, StatTypes.Default, 1)]
        public static float damageCoef;
        [TokenModifier(tkn, StatTypes.Default, 2)]
        [HideInInspector] public static float explosionDamageCoef = damageCoef * 3;
        public static float force;
        
        private GameObject areaIndicatorInstance;
        private float damage;
        private bool crit;
        private int totalRocketsFired;
        private float timeBetweenRockets;
        private float rocketFireStopwatch;
        private MuzzleTransforms bothMuzzles;
        private FireProjectileInfo cachedProjectileInfo;
        public override void OnEnter()
        {
            areaIndicatorInstance = Huntress.ArrowRain.areaIndicatorPrefab;
            base.OnEnter();
            timeBetweenRockets = firingDuration / rocketAmount;
            crit = RollCrit();
            damage = damageStat * damageCoef;
            SetStep(2);
            bothMuzzles = GetMuzzle();
            cachedProjectileInfo = CreateProjectileInfo();
            if(areaIndicatorPrefab)
            {
                areaIndicatorInstance = Object.Instantiate(areaIndicatorPrefab);
            }
        }

        private void UpdateAreaIndicator()
        {
            if(areaIndicatorInstance)
            {
                float maxDistance = 1000f;
                if(Physics.Raycast(GetAimRay(), out var hitinfo, maxDistance, LayerIndex.world.mask))
                {
                    areaIndicatorInstance.transform.position = hitinfo.point;
                    areaIndicatorInstance.transform.up = hitinfo.normal;
                }
            }
        }

        public override void Update()
        {
            base.Update();
            UpdateAreaIndicator();
        }

        protected override void DoFire()
        {
            rocketFireStopwatch += Time.fixedDeltaTime;
            if(rocketFireStopwatch >= timeBetweenRockets && totalRocketsFired < rocketAmount)
            {
                rocketFireStopwatch -= timeBetweenRockets;
                if (!Run.instance)
                    return;
                FireProjectile();
            }
            if(fixedAge >= firingDuration && totalRocketsFired == rocketAmount && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireProjectile()
        {
            if(isAuthority)
            {
                aimRay = GetAimRay();
                SetStep(Run.instance.runRNG.RangeInt(0, 2));
                bool homing = CheckRollHoming();
                cachedProjectileInfo.projectilePrefab = homing ? guidedRocketPrefab : rocketPrefab;
                cachedProjectileInfo.position = aimRay.origin;
                cachedProjectileInfo.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
                switch (CurrentMuzzle)
                {
                    case Muzzle.Left:
                        cachedProjectileInfo.position = bothMuzzles.leftMuzzle.position;
                        break;
                    case Muzzle.Right:
                        cachedProjectileInfo.position = bothMuzzles.rightMuzzle.position;
                        break;
                }
                ProjectileManager.instance.FireProjectile(cachedProjectileInfo);
            }
            totalRocketsFired++;
        }

        public override void OnExit()
        {
            if(areaIndicatorInstance)
                Destroy(areaIndicatorInstance);
            base.OnExit();
        }

        private FireProjectileInfo CreateProjectileInfo()
        {
            return new FireProjectileInfo
            {
                owner = gameObject,
                damage = damage,
                force = force,
                crit = crit,
                damageColorIndex = DamageColorIndex.Default,
            };
        }
        private bool CheckRollHoming()
        {
            if(characterBody && characterBody.master)
            {
                return Util.CheckRoll(guidedChance, characterBody.master);
            }
            return false;
        }
    }
}
