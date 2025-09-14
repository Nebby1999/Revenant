using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityStates;
using MSU;
using R2API.ScriptableObjects;
using RoR2;

namespace RevenantMod
{
    public class RevenantContent //: ContentLoader<RevenantContent>
    {
        public static class Survivors
        {
            public static SurvivorDef Revenant;
        }

        public static class BuffDefs
        {
            public static BuffDef bdAntiCoagulant;
        }
        /*

        public override string identifier => RevenantMain.GUID;
        //FIXME
        public override R2APISerializableContentPack SerializableContentPack { get; protected set; } = null;//RevenantAssets.LoadAsset<R2APISerializableContentPack>("RevenantContentPack");
        public override Action[] LoadDispatchers { get; protected set; }
        public override Action[] PopulateFieldsDispatchers { get; protected set; }

        public override void Init()
        {
            base.Init();

            LoadDispatchers = new Action[]
            {
                () =>
                {
                    RevLog.Message($"Loading Started");
                    new Modules.Characters().Initialize();
                },
                () =>
                {
                    new Modules.Buffs().Initialize();
                },
                () =>
                {
                    new Modules.DamageTypes().Initialize();
                },
                () =>
                {
                    RevLog.Info($"Loading Entity States");
                    //FIXME
                    SerializableContentPack.entityStateConfigurations = null;//RevenantAssets.LoadAllAssetsOfType<EntityStateConfiguration>();
                    this.GetType().Assembly.GetTypes()
                                      .Where(type => typeof(EntityStates.EntityState).IsAssignableFrom(type) && !type.IsAbstract)
                                      .ToList()
                                      .ForEach(state => HG.ArrayUtils.ArrayAppend(ref SerializableContentPack.entityStateTypes, new EntityStates.SerializableEntityStateType(state)));
                    RevLog.Message($"Loading Finished.");
                }
            };

            PopulateFieldsDispatchers = new Action[]
            {
                //FIXME
                () =>
                {
                    //ContentUtil.PopulateTypeFields(typeof(Survivors), ContentPack.survivorDefs);
                },
                () =>
                {
                    //ContentUtil.PopulateTypeFields(typeof(BuffDef), ContentPack.buffDefs);
                }
            };
        }*/
    }
}
