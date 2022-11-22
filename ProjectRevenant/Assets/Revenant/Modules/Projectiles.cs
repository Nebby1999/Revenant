using Moonstorm;
using R2API.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenantMod.Modules
{
    public class Projectiles : ProjectileModuleBase
    {
        public override R2APISerializableContentPack SerializableContentPack => RevenantContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            RevLog.Info($"Initializing Projectiles...");
            base.Initialize();
            foreach (ProjectileBase pb in base.GetProjectileBases())
            {
                AddProjectile(pb);
            }
        }
    }
}