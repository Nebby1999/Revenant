using RoR2;
using UnityEngine;
using System.Collections.Generic;
using MSU;

namespace RevenantMod
{
    [CreateAssetMenu(fileName = "EquipmentAssetCollection", menuName = "Revenant/AssetCollections/EquipmentAssetCollection")]
    public class EquipmentAssetCollection : ExtendedAssetCollection
    {
        public NullableRef<List<GameObject>> itemDisplayPrefabs;
        public EquipmentDef equipmentDef;
    }
}