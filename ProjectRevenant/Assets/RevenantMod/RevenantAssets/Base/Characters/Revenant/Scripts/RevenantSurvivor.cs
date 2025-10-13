using MSU;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RevenantMod.Survivors
{
    public class RevenantSurvivor : ISurvivorContentPiece, IContentPackModifier
    {
        /// <summary>
        /// If the victim has the snaring debuff, consume all stacks and grant 10 * procCoef * snareBuffCount fuel to revenant.
        /// </summary>
        public static R2API.DamageAPI.ModdedDamageType jailingDamageType { get; private set; }
        /// <summary>
        /// Snares an enemy by reducing their movement speed.
        /// </summary>
        public static R2API.DamageAPI.ModdedDamageType snaringDamageType { get; private set; }

        public SurvivorDef survivorDef { get; private set; }

        public NullableRef<GameObject> masterPrefab { get; private set; }

        public CharacterBody characterBody { get; private set; }

        public GameObject bodyPrefab { get; private set; }

        CharacterBody IGameObjectContentPiece<CharacterBody>.component => characterBody;

        GameObject IContentPiece<GameObject>.asset => bodyPrefab;

        private SurvivorAssetCollection _survivorAssetCollection;
        private BuffDef _bdRevenantSnaring;
        public void Initialize()
        {
            characterBody._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_UI.SimpleDotCrosshair_prefab).WaitForCompletion();
            snaringDamageType = R2API.DamageAPI.ReserveDamageType();
            jailingDamageType = R2API.DamageAPI.ReserveDamageType();
            _bdRevenantSnaring = _survivorAssetCollection.FindAsset<BuffDef>("bdRevenantSnaring");

            var projectile = _survivorAssetCollection.FindAsset<GameObject>("RevenantRocket");
            projectile.GetComponent<ProjectileDamage>().damageType.AddModdedDamageType(jailingDamageType);
            projectile = _survivorAssetCollection.FindAsset<GameObject>("RevenantRocketHoming");
            projectile.GetComponent<ProjectileDamage>().damageType.AddModdedDamageType(jailingDamageType);

            R2API.RecalculateStatsAPI.GetStatCoefficients += HandleRevenantSnaring;
            GlobalEventManager.onServerDamageDealt += HandleDamageTypes;
        }

        private void HandleRevenantSnaring(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int count = sender.GetBuffCount(_bdRevenantSnaring);
            if(count > 0)
            {
                args.moveSpeedReductionMultAdd += 0.1f * count;
            }
        }

        private void HandleDamageTypes(DamageReport obj)
        {
            var victim = obj.victimBody;
            if (!victim)
                return;

            if (obj.damageInfo == null)
                return;

            if (obj.damageInfo.HasModdedDamageType(snaringDamageType))
            {
                victim.AddTimedBuff(_bdRevenantSnaring, 5);
                for (int i = 0; i < victim.timedBuffs.Count; i++)
                {
                    CharacterBody.TimedBuff timedBuff = victim.timedBuffs[i];
                    if (timedBuff.buffIndex == _bdRevenantSnaring.buffIndex)
                    {
                        if (timedBuff.timer < 5)
                        {
                            timedBuff.timer = 5;
                            timedBuff.totalDuration = 5;
                        }
                    }
                }
            }

            if (obj.damageInfo.HasModdedDamageType(jailingDamageType))
            {
                int count = victim.GetBuffCount(_bdRevenantSnaring);
                victim.ClearTimedBuffs(_bdRevenantSnaring);

                if(obj.damageInfo.attacker && obj.damageInfo.attacker.TryGetComponent<RevenantFuelController>(out var controller))
                {
                    controller.AddFuel(10 * obj.damageInfo.procCoefficient * count);
                }
            }
        }

        public bool IsAvailable(ContentPack contentPack)
        {
            return true;
        }

        public IEnumerator LoadContentAsync()
        {
            RevenantAssetRequest<SurvivorAssetCollection> request = RevenantAssets.LoadAssetAsync<SurvivorAssetCollection>("acRevenant", RevenantBundle.Main);

            request.StartLoad();
            while (!request.isComplete)
                yield return null;

            _survivorAssetCollection = request.asset;

            survivorDef = _survivorAssetCollection.survivorDef;
            masterPrefab = _survivorAssetCollection.masterPrefab;
            bodyPrefab = _survivorAssetCollection.bodyPrefab;
            characterBody = _survivorAssetCollection.bodyPrefab.GetComponent<CharacterBody>();
        }

        public void ModifyContentPack(ContentPack contentPack)
        {
            contentPack.AddContentFromAssetCollection(_survivorAssetCollection);
        }
    }
}
