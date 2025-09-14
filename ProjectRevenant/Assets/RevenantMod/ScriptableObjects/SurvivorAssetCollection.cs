using RoR2;
using UnityEngine;

namespace RevenantMod
{
    [CreateAssetMenu(fileName = "SurvivorAssetCollection", menuName = "Revenant/AssetCollections/SurvivorAssetCollection")]
    public class SurvivorAssetCollection : BodyAssetCollection
    {
        public SurvivorDef survivorDef;
    }
}
