using RevenantMod.Survivors;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;
using AR = EntityStates.Huntress.ArrowRain;

namespace EntityStates.RevenantMod
{
    public class FireRocketBarrage : BaseFireBarrageState
    {
        public static float rocketArrivalTime;
        public static float rocketSpeed;
        public static float explosionRadius;
        public static BlastAttack.FalloffModel falloffModel;
        public static BlastAttack.LoSType LoSType;
        protected override void FireRound()
        {
            if (!NetworkServer.active)
                return;

            var aimRay = GetAimRay();
            string muzzle = (Random.value > 0.5) ? leftMuzzleString : rightMuzzleString;
            Transform muzzleTransform = FindModelChild(muzzle);
            RocketOrb rocketOrb = new RocketOrb()
            {
                arrivalTime = rocketArrivalTime,
                attacker = gameObject,
                baseForce = force,
                damageValue = perRoundDamage,
                explosionRadius = explosionRadius,
                falloffModel = falloffModel,
                isCrit = isCrit,
                LoSType = LoSType,
                origin = muzzleTransform.position,
                procCoefficient = procCoefficient,
                speed = rocketSpeed,
                teamIndex = GetTeam(),
            };

            AssignTarget(rocketOrb, aimRay);
            OrbManager.instance.AddOrb(rocketOrb);
        }

        //I want the orbs to have spread, so i copied this from bullet attack, lol. It'll also assign a target if it finds one!
        private void AssignTarget(RocketOrb rocketOrb, Ray aimRay)
        {
            Vector3 axis2 = Vector3.Cross(Vector3.up, aimRay.direction);

            float x3 = UnityEngine.Random.Range(minSpread, maxSpread);
            float z3 = UnityEngine.Random.Range(0f, 360f);
            Vector3 vector3 = Quaternion.Euler(0, 0, z3) * (Quaternion.Euler(x3, 0, 0) * Vector3.forward);
            float y3 = vector3.y;
            vector3.y = 0f; 
            float angle5 = (Mathf.Atan2(vector3.z, vector3.x) * 57.29578f - 90f) * 1;
            float angle6 = Mathf.Atan2(y3, vector3.magnitude) * 57.29578f * 1;

            aimRay.direction = Quaternion.AngleAxis(angle5, Vector3.up) * (Quaternion.AngleAxis(angle6, axis2) * aimRay.direction);

            if(Util.CharacterRaycast(gameObject, aimRay, out RaycastHit hitInfo, float.PositiveInfinity, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.Ignore))
            {
                if(hitInfo.collider && hitInfo.collider.TryGetComponent<HurtBox>(out var hurtbox))
                {
                    rocketOrb.target = hurtbox;
                }
                else
                {
                    rocketOrb.targetPosition = hitInfo.point;
                }
            }
            else
            {
                rocketOrb.targetPosition = aimRay.GetPoint(2048);
            }
        }
    }
}