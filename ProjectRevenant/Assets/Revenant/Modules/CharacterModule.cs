using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm;
using R2API.ScriptableObjects;

namespace Revenant.Modules
{
    public class CharacterModule : CharacterModuleBase
    {
        public override R2APISerializableContentPack SerializableContentPack => RevenantContent.Instance.SerializableContentPack;

        public override void Initialize()
        {
            base.Initialize();
            RevLog.Info($"Initializing Characters.");
            GetCharacterBases();
        }

        protected override IEnumerable<CharacterBase> GetCharacterBases()
        {
            foreach(var cb in base.GetCharacterBases())
            {
                AddCharacter(cb);
            }
            return null;
        }
    }
}
