using RoR2;
using UnityEngine;
namespace RevenantMod
{
    [CreateAssetMenu(fileName = "BodyAssetCollection", menuName = "Revenant/AssetCollections/BodyAssetCollection")]
    public class BodyAssetCollection : ExtendedAssetCollection
    {
        public GameObject bodyPrefab;
        public GameObject masterPrefab;
    }
}
