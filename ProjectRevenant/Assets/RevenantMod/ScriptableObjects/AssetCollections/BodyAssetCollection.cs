using RoR2;
using UnityEngine;
namespace RevenantMod
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "BodyAssetCollection", menuName = "Revenant/AssetCollections/BodyAssetCollection")]
#endif
    public class BodyAssetCollection : ExtendedAssetCollection
    {
        public GameObject bodyPrefab;
        public GameObject masterPrefab;
    }
}
