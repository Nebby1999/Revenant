using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm.Loaders;
using Moonstorm;
using UnityEngine;
using System.IO;

namespace Revenant
{
    public class RevenantAssets : AssetsLoader<RevenantAssets>
    {
        public override AssetBundle MainAssetBundle => _mainBundle;
        private AssetBundle _mainBundle;
        public string AssemblyDir => Path.GetDirectoryName(RevenantMain.Instance.Info.Location);

        private const string assetBundleFolderName = "assetbundles";
        private const string mainAssetBundleName = "revenantassets";

        internal void Init()
        {
            _mainBundle = AssetBundle.LoadFromFile(Path.Combine(AssemblyDir, assetBundleFolderName, mainAssetBundleName));
        }

        internal void SwapMaterialShaders()
        {
            SwapShadersFromMaterials(MainAssetBundle.LoadAllAssets<Material>().Where(m => m.shader.name.StartsWith("Stubbed")));
        }
    }
}
