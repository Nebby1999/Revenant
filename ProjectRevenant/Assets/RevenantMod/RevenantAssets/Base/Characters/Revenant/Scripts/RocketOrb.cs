using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace RevenantMod.Survivors
{
    public class RocketOrb : GenericDamageOrb
    {
        public float explosionRadius;
        public float baseForce;
        public BlastAttack.FalloffModel falloffModel;
        public BlastAttack.LoSType LoSType;
        public Vector3 targetPosition;

        public Vector3 explosionPosition => target ? target.transform.position : targetPosition;

        public override void Begin()
        {
            duration = Vector3.Distance(explosionPosition, origin) / speed;
            GameObject orbEffect = GetOrbEffect();
            if(orbEffect)
            {
                EffectData effectData = new EffectData { scale = scale, origin = origin, genericFloat = duration };
                if(target)
                {
                    effectData.SetHurtBoxReference(target);
                }
                else
                {
                    effectData.start = targetPosition;
                }
                EffectManager.SpawnEffect(orbEffect, effectData, true);
            }
        }

        public override GameObject GetOrbEffect()
        {
            return RevenantSurvivor.rocketOrbEffect;
        }

        public override void OnArrival()
        {
            BlastAttack blastAttack = new BlastAttack
            {
                attacker = attacker,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                baseDamage = damageValue,
                baseForce = baseForce,
                canRejectForce = false,
                crit = isCrit,
                damageType = damageType,
                falloffModel = falloffModel,
                inflictor = attacker,
                losType = LoSType,
                position = explosionPosition,
                procCoefficient = procCoefficient,
                radius = explosionRadius,
                teamIndex = teamIndex
            };
            blastAttack.Fire();
        }
    }
}