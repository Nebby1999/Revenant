using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MSU;
using FL = EntityStates.GolemMonster.FireLaser;

namespace EntityStates.Revenant.RocketLauncher
{
    public class LaserBarrage : BaseRocketLauncherBarrage
    {
        private const string tkn = "REV_REVENANT_SPECIAL_LASERBARRAGE_DESC";

        [HideInInspector]
        public static GameObject hitEffectPrefab;
        [HideInInspector]
        public static GameObject tracerEffectPrefab;
        [HideInInspector]
        public static GameObject areaIndicatorPrefab;
        [FormatToken(tkn, 0)]
        public static int laserAmount;
        public static float firingDuration;
        public static float minSpread;
        public static float maxSpread;
        [FormatToken(tkn, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100, 1)]
        public static float damageCoef;
        public static float force;
        public static float laserRadius;

        private GameObject areaIndicatorInstance;
        private float damage;
        private bool crit;
        private int totalLasersFired;
        private float timeBetweenLasers;
        private float laserFireStopwatch;
        private MuzzleTransforms bothMuzzles;

        public override void OnEnter()
        {
            hitEffectPrefab = FL.hitEffectPrefab;
            tracerEffectPrefab = FL.tracerEffectPrefab;
            areaIndicatorInstance = Huntress.ArrowRain.areaIndicatorPrefab;
            base.OnEnter();
            timeBetweenLasers = firingDuration / laserAmount;
            crit = RollCrit();
            damage = damageStat * damageCoef;
            SetStep(2);
            bothMuzzles = GetMuzzle();
            if(areaIndicatorPrefab)
            {
                areaIndicatorInstance = Object.Instantiate(areaIndicatorPrefab);
            }
        }

        private void UpdateAreaIndicator()
        {
            if (areaIndicatorInstance)
            {
                float maxDistance = 1000f;
                if (Physics.Raycast(GetAimRay(), out var hitinfo, maxDistance, LayerIndex.world.mask))
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
            laserFireStopwatch += Time.fixedDeltaTime;
            if (laserFireStopwatch >= timeBetweenLasers && totalLasersFired < laserAmount)
            {
                laserFireStopwatch -= timeBetweenLasers;
                if (!Run.instance)
                    return;
                FireLaser();
            }
            if (fixedAge >= firingDuration && totalLasersFired == laserAmount && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireLaser()
        {
            aimRay = GetAimRay();
            SetStep(Run.instance.runRNG.RangeInt(0, 2));
            if(isAuthority)
            {
                BulletAttack ba = new BulletAttack();
                ba.owner = gameObject;
                ba.weapon = gameObject;
                ba.origin = aimRay.origin;
                ba.aimVector = aimRay.direction;
                ba.minSpread = minSpread;
                ba.maxSpread = maxSpread;
                ba.damage = damage;
                ba.force = force;
                ba.tracerEffectPrefab = tracerEffectPrefab;
                ba.muzzleName = CurrentMuzzle == Muzzle.Left ? bothMuzzles.leftMuzzleChildLocatorName : bothMuzzles.rightMuzzleChildLocatorName;
                ba.hitEffectPrefab = hitEffectPrefab;
                ba.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                ba.isCrit = crit;
                ba.radius = laserRadius;
                ba.smartCollision = true;
                ba.Fire();
            }
            totalLasersFired++;
        }
        public override void OnExit()
        {
            if (areaIndicatorInstance)
                Destroy(areaIndicatorInstance);
            base.OnExit();
        }
    }
}
