using RoR2;
using UnityEngine;
using System.Collections.Generic;
using MSU;

namespace RevenantMod
{
#if UNITY_EDITOR
    [CreateAssetMenu(fileName = "EquipmentAssetCollection", menuName = "Revenant/AssetCollections/EquipmentAssetCollection")]
#endif
    public class EquipmentAssetCollection : ExtendedAssetCollection
    {
        public NullableRef<List<GameObject>> itemDisplayPrefabs;
        public EquipmentDef equipmentDef;
    }
}