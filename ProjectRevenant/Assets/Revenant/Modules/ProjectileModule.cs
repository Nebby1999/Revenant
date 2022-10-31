using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonstorm;
using R2API.ScriptableObjects;

namespace Revenant.Modules
{
    public class ProjectileModule : ProjectileModuleBase
    {
        public override R2APISerializableContentPack SerializableContentPack => RevenantContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            base.Initialize();
            RevLog.Info($"Initializing Projectiles.");
            GetProjectileBases();
        }

        protected override IEnumerable<ProjectileBase> GetProjectileBases()
        {
            foreach(var pb in base.GetProjectileBases())
            {
                AddProjectile(pb);
            }
            return null;
        }
    }
}
