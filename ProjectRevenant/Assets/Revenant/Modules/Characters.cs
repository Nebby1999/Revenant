using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm;
using R2API.ScriptableObjects;

namespace RevenantMod.Modules
{
    public class Characters : CharacterModuleBase
    {
        public override R2APISerializableContentPack SerializableContentPack => RevenantContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            base.Initialize();
            RevLog.Info($"Initializing Characters.");
            foreach (var cb in GetCharacterBases())
            {
                AddCharacter(cb);
            }
        }
    }
}
