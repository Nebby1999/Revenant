using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityStates;
using Moonstorm.Loaders;
using R2API.ScriptableObjects;
using RoR2;

namespace RevenantMod
{
    public class RevenantContent : ContentLoader<RevenantContent>
    {
        public static class Survivors
        {
            public static SurvivorDef Revenant;
        }

        public static class BuffDefs
        {
            public static BuffDef bdAntiCoagulant;
        }

        public override string identifier => RevenantMain.GUID;
        public override R2APISerializableContentPack SerializableContentPack { get; protected set; } = RevenantAssets.LoadAsset<R2APISerializableContentPack>("RevenantContentPack");
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
                    new Modules.Projectiles().Initialize();
                },
                () =>
                {
                    RevLog.Info($"Loading Entity States");
                    SerializableContentPack.entityStateConfigurations = RevenantAssets.LoadAllAssetsOfType<EntityStateConfiguration>();
                    GetType().Assembly.GetTypes()
                                      .Where(type => typeof(EntityStates.EntityState).IsAssignableFrom(type) && !type.IsAbstract)
                                      .ToList()
                                      .ForEach(state => HG.ArrayUtils.ArrayAppend(ref SerializableContentPack.entityStateTypes, new EntityStates.SerializableEntityStateType(state)));
                    RevLog.Message($"Loading Finished.");
                }
            };

            PopulateFieldsDispatchers = new Action[]
            {
                () =>
                {
                    PopulateTypeFields(typeof(Survivors), ContentPack.survivorDefs);
                },
                () =>
                {
                    PopulateTypeFields(typeof(BuffDefs), ContentPack.buffDefs);
                }
            };
        }
    }
}
