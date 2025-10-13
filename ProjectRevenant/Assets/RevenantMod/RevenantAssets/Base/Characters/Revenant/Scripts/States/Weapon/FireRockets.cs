using RoR2;
using RoR2.ConVar;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.RevenantMod.Weapon
{
    public class FireRockets : BaseLauncherState
    {
        public static BoolConVar modifyOriginConvar = new BoolConVar("revenantmod_modify_rocket_direction", ConVarFlags.None, "1", "Penis");
        public static GameObject rocketPrefab;
        public static GameObject homingRocketPrefab;
        public static float homingChance;
        public static float baseDuration;
        public static float targetAnticipationRadius;
        public static float targetAnticipationDistance;

        public static float damageCoef;
        public static float force;

        private static float _duration;

        public override void OnEnter()
        {
            base.OnEnter();
            _duration = baseDuration / attackSpeedStat;

            PlayLauncherAnimation();

            if(isAuthority)
            {
                Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(isAuthority && fixedAge > _duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void Fire()
        {
            bool isCrit = RollCrit();
            Ray aimRay = GetAimRay();

            GameObject target = AnticipateTarget(out RaycastHit? hitInfo);
            GameObject chosenProjectilePrefab = ChooseProjectile();

            foreach (string muzzleName in muzzleNameList)
            {
                Vector3 direction = aimRay.direction;
                Vector3 origin = aimRay.origin + Vector3.Cross(transform.up, direction) * muzzleName switch
                {
                    "LeftMuzzle" => 1 * -1f,
                    "RightMuzzle" => 1,
                    _ => 0f
                };
                if (modifyOriginConvar.value && hitInfo.HasValue)
                {
                    Vector3 approximatePoint = hitInfo.Value.point;
                    Vector3 newDirection = (approximatePoint - origin).normalized;
                    direction = newDirection;
                }

                FireProjectileInfo info = new FireProjectileInfo
                {
                    owner = gameObject,
                    crit = isCrit,
                    damage = damageStat * damageCoef,
                    force = force,
                    position = origin,
                    projectilePrefab = chosenProjectilePrefab,
                    rotation = Util.QuaternionSafeLookRotation(direction),
                    target = target,
                };

                ProjectileManager.instance.FireProjectile(info);
            }
        }

        private GameObject AnticipateTarget(out RaycastHit? raycastHit)
        {
            if(!Util.CharacterSpherecast(gameObject, GetAimRay(), targetAnticipationRadius, out var hitInfo, targetAnticipationDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal))
            {
                raycastHit = null;
                return null;
            }
            raycastHit = hitInfo;

            if (!hitInfo.collider)
                return null;

            if(!hitInfo.collider.TryGetComponent<HurtBox>(out var hb))
            {
                return null;
            }

            HealthComponent hc = hb.healthComponent;
            if(!hc)
            {
                return null;
            }

            if(hc.body.teamComponent.teamIndex == teamComponent.teamIndex || FriendlyFireManager.friendlyFireMode != FriendlyFireManager.FriendlyFireMode.Off)
            {
                return null;
            }

            return hc.gameObject;
        }

        private GameObject ChooseProjectile()
        {
            return Util.CheckRoll(homingChance, characterBody.master) ? homingRocketPrefab : rocketPrefab;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}