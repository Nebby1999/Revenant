using Moonstorm;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevenantMod.Projectiles
{
    public class GuidedRocket : ProjectileBase
    {
        public override GameObject ProjectilePrefab => RevenantAssets.LoadAsset<GameObject>("RevenantGuidedRocketProjectile");

        public override void Initialize()
        {
            base.Initialize();
            var mdth = ProjectilePrefab.AddComponent<R2API.DamageAPI.ModdedDamageTypeHolderComponent>();
            mdth.Add(DamageTypes.AntiCoagulant.dtAntiCoagulant);
        }
    }
}