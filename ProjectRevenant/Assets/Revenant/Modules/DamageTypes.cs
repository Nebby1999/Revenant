using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonstorm;
using R2API.ScriptableObjects;

namespace RevenantMod.Modules
{
    public class DamageTypes : DamageTypeModuleBase
    {
        public override void Initialize()
        {
            base.Initialize();
            RevLog.Info($"Initializing Damage Types.");
            foreach (DamageTypeBase dtb in GetDamageTypeBases())
            {
                AddDamageType(dtb);
            }
        }
    }
}
