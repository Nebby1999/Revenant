using RoR2;
using UnityEngine;

namespace RevenantMod
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "SurvivorAssetCollection", menuName = "Revenant/AssetCollections/SurvivorAssetCollection")]
#endif
    public class SurvivorAssetCollection : BodyAssetCollection
    {
        public SurvivorDef survivorDef;
    }
}
