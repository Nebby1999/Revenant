using MSU;
using RoR2;
using RoR2.ContentManagement;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RevenantMod.Survivors
{
    public class RevenantSurvivor : ISurvivorContentPiece, IContentPackModifier
    {
        public SurvivorDef survivorDef { get; private set; }

        public NullableRef<GameObject> masterPrefab { get; private set; }

        public CharacterBody characterBody { get; private set; }

        public GameObject bodyPrefab { get; private set; }

        CharacterBody IGameObjectContentPiece<CharacterBody>.component => characterBody;

        GameObject IContentPiece<GameObject>.asset => bodyPrefab;

        private SurvivorAssetCollection _survivorAssetCollection;

        public void Initialize()
        {
            characterBody._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2_Base_UI.SimpleDotCrosshair_prefab).WaitForCompletion();
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
