using MSU;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UObject = UnityEngine.Object;


namespace RevenantMod
{
    public enum RevenantBundle
    {
        Invalid,
        StreamedScene,
        All,
        Main
    }

    public static class RevenantAssets
    {
        private const string ASSET_BUNDLE_FOLDER_NAME = "assetbundles";
        private const string MAIN = "revmain";

        private static string assetBundleFolderPath => Path.Combine(Path.GetDirectoryName(RevenantMain.instance.Info.Location), ASSET_BUNDLE_FOLDER_NAME);

        private static Dictionary<RevenantBundle, AssetBundle> _assetBundles = new Dictionary<RevenantBundle, AssetBundle>();
        private static AssetBundle[] _streamedSceneBundles = Array.Empty<AssetBundle>();

        public static ResourceAvailability assetsAvailability;

        /// <summary>
        /// Returns the AssetBundle that's tied to the supplied enum value.
        /// </summary>
        /// <param name="bundle">The bundle to obtain</param>
        /// <returns>The tied assetbundle, null if the enum value is All, Invalid or Streamed Scene</returns>
        public static AssetBundle GetAssetBundle(RevenantBundle bundle)
        {
            if (IsEnumValueInvalidForGetAssetBundleOperation(bundle))
            {
                return null;
            }

            return _assetBundles[bundle];
        }

        private static bool IsEnumValueInvalidForGetAssetBundleOperation(RevenantBundle bundle)
        {
            return bundle == RevenantBundle.All || bundle == RevenantBundle.Invalid || bundle == RevenantBundle.StreamedScene;
        }

        private static bool IsEnumValueInvalidForAssetLoadingOperation(RevenantBundle bundle)
        {
            return bundle == RevenantBundle.Invalid || bundle == RevenantBundle.StreamedScene;
        }

        /// <summary>
        /// Loads an asset of type <typeparamref name="TAsset"/> and name <paramref name="name"/> from the asset bundle specified by <paramref name="bundle"/>.
        /// <para>See also <see cref="LoadAssetAsync{TAsset}(string, RevenantBundle)"/></para>
        /// </summary>
        /// <typeparam name="TAsset">The type of asset</typeparam>
        /// <param name="name">The name of the Asset</param>
        /// <param name="bundle">The bundle to load from. Accepts the value <see cref="RevenantBundle.All"/>, but it'll log a warning since using the value <see cref="RevenantBundle.All"/> creates unecesary calls.</param>
        /// <returns>The loaded asset if it exists, null otherwise.</returns>
        public static TAsset LoadAsset<TAsset>(string name, RevenantBundle bundle) where TAsset : UObject
        {
            if (IsEnumValueInvalidForAssetLoadingOperation(bundle))
            {
#if DEBUG
                RevLog.Warning("Cannot load asset from bundle enum of value " + bundle);
#endif
                return null;
            }

            TAsset asset = null;
            if (bundle == RevenantBundle.All)
            {
                return FindAsset<TAsset>(name);
            }

            asset = _assetBundles[bundle].LoadAsset<TAsset>(name);

#if DEBUG
            if (!asset)
            {
                RevLog.Warning($"The method \"{GetCallingMethod()}\" is calling \"LoadAsset<TAsset>(string, RevenantBundle)\" with the arguments \"{typeof(TAsset).Name}\", \"{name}\" and \"{bundle}\", however, the asset could not be found.\n" +
                    $"A complete search of all the bundles will be done and the correct bundle enum will be logged.");

                return LoadAsset<TAsset>(name, RevenantBundle.All);
            }
#endif
            return asset;
        }

        /// <summary>
        /// Creates an instance of <see cref="RevenantAssetRequest{TAsset}"/> which will contain the necesary metadata for loading an Asset asynchronously.
        /// <para>See also <see cref="LoadAsset{TAsset}(string, RevenantBundle)"/></para>
        /// </summary>
        /// <typeparam name="TAsset">The type of asset to load</typeparam>
        /// <param name="name">The name of the asset to load</param>
        /// <param name="bundle">The bundle to search thru, accepts the <see cref="RevenantBundle.All"/> value but it's not recommended as it creates unecesary calls.</param>
        /// <returns>The <see cref="RevenantAssetRequest{TAsset}"/> to use for asynchronous loading.</returns>
        public static RevenantAssetRequest<TAsset> LoadAssetAsync<TAsset>(string name, RevenantBundle bundle) where TAsset : UObject
        {
            if (IsEnumValueInvalidForAssetLoadingOperation(bundle))
            {
#if DEBUG
                RevLog.Warning("Cannot load asset asynchronously from bundle enum of value " + bundle);
#endif
                return null;
            }

            return new RevenantAssetRequest<TAsset>(name, bundle);
        }

        /// <summary>
        /// Loads all assets of type <typeparamref name="TAsset"/> from the AssetBundle specified by <paramref name="bundle"/>
        /// <para>See also <see cref="LoadAllAssetsAsync{TAsset}(RevenantBundle)"/></para>
        /// </summary>
        /// <typeparam name="TAsset">The type of asset to load</typeparam>
        /// <param name="bundle">The AssetBundle to load from, accepts the <see cref="RevenantBundle.All"/> value</param>
        /// <returns>An array of <typeparamref name="TAsset"/> which contains all the loaded assets.</returns>
        public static TAsset[] LoadAllAssets<TAsset>(RevenantBundle bundle) where TAsset : UObject
        {
            if (IsEnumValueInvalidForAssetLoadingOperation(bundle))
            {
#if DEBUG
                RevLog.Warning("Cannot load assets from bundle enum of value " + bundle);
#endif
                return null;
            }

            TAsset[] loadedAssets = null;
            if (bundle == RevenantBundle.All)
            {
                return FindAssets<TAsset>();
            }
            loadedAssets = _assetBundles[bundle].LoadAllAssets<TAsset>();

#if DEBUG
            if (loadedAssets.Length == 0)
            {
                RevLog.Warning($"Could not find any asset of type {typeof(TAsset).Name} inside the bundle {bundle}");
            }
#endif
            return loadedAssets;
        }

        /// <summary>
        /// Creates an instance of <see cref="RevenantAssetRequest{TAsset}"/> which will contain the necesary metadata for loading an Asset asynchronously.
        /// <para>See also <see cref="LoadAllAssets{TAsset}(RevenantBundle)"/></para>
        /// </summary>
        /// <typeparam name="TAsset">The type of asset to load</typeparam>
        /// <param name="bundle">The AssetBundle to load from, accepts the <see cref="RevenantBundle.All"/> value</param>
        /// <returns>The <see cref="RevenantAssetRequest{TAsset}"/> to use for asynchronous loading.</returns>
        public static RevenantAssetRequest<TAsset> LoadAllAssetsAsync<TAsset>(RevenantBundle bundle) where TAsset : UObject
        {
            if (IsEnumValueInvalidForAssetLoadingOperation(bundle))
            {
#if DEBUG
                RevLog.Warning("Cannot load asset from bundle enum of value " + bundle);
#endif
                return null;
            }

            return new RevenantAssetRequest<TAsset>(bundle);
        }

        /// <summary>
        /// Initializes the mod's asset bundles asynchronously, should only be called once and during <see cref="RevenantContent.LoadStaticContentAsync(RoR2.ContentManagement.LoadStaticContentAsyncArgs)"/>
        /// </summary>
        /// <returns>A coroutine which can be awaited.</returns>
        internal static IEnumerator Initialize()
        {
            RevLog.Info($"Initializing Assets...");
            var loadRoutine = LoadAssetBundles();

            while(!loadRoutine.IsDone())
            {
                yield return null;
            }

            MSU.ParallelCoroutine parallelCoroutine = new MSU.ParallelCoroutine();
            parallelCoroutine.Add(SwapShaders());
            parallelCoroutine.Add(SwapAddressableShaders());

            while (!parallelCoroutine.isDone) yield return null;

            assetsAvailability.MakeAvailable();
            yield break;
        }

        //This is a method which is used to load the AssetBundles from the mod asynchronously, it is very complicated but this method should not be touched as if you properly add the new Enum and const string values, managing the new bundles will be easy.
        //look at the method "LoadFromPath", that one contains stuff you should be interested in modifying in the future.
        private static IEnumerator LoadAssetBundles()
        {
            ParallelCoroutine parallelCoroutine = new();

            List<(string path, RevenantBundle bundleEnum, AssetBundle loadedBundle)> pathsAndBundles = new List<(string path, RevenantBundle bundleEnum, AssetBundle loadedBundle)>();

            string[] paths = GetAssetBundlePaths();
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                parallelCoroutine.Add(LoadFromPath(pathsAndBundles, path, i, paths.Length));
            }

            while (!parallelCoroutine.IsDone())
                yield return null;

            foreach ((string path, RevenantBundle bundleEnum, AssetBundle assetBundle) in pathsAndBundles)
            {
                if (bundleEnum == RevenantBundle.StreamedScene)
                {
                    HG.ArrayUtils.ArrayAppend(ref _streamedSceneBundles, assetBundle);
                }
                else
                {
                    _assetBundles[bundleEnum] = assetBundle;
                }
            }
        }

        private static IEnumerator LoadFromPath(List<(string path, RevenantBundle bundleEnum, AssetBundle loadedBundle)> list, string path, int index, int totalPaths)
        {
            string fileName = Path.GetFileName(path);
            RevenantBundle? revenantBundle = null;
            switch (fileName)
            {
                case MAIN: revenantBundle = RevenantBundle.Main; break;
                default: revenantBundle = RevenantBundle.StreamedScene; break;
            }

            var request = AssetBundle.LoadFromFileAsync(path);
            while (!request.isDone)
            {
                yield return null;
            }

            AssetBundle bundle = request.assetBundle;

            if (!bundle)
            {
                throw new FileLoadException($"AssetBundle.LoadFromFile did not return an asset bundle. (Path={path})");
            }

            if (revenantBundle == RevenantBundle.StreamedScene)
            {
                if (!bundle.isStreamedSceneAssetBundle)
                {
                    throw new Exception($"AssetBundle in specified path is not a streamed scene bundle, but its file name was not found in the Switch statement. have you forgotten to setup the enum and file name in your assets class? (Path={path})");
                }
                else
                {
                    list.Add((path, RevenantBundle.StreamedScene, bundle));
                    yield break;
                }
            }

            list.Add((path, revenantBundle.Value, bundle));
            yield break;
        }

        private static string[] GetAssetBundlePaths()
        {
            return Directory.GetFiles(assetBundleFolderPath).Where(filePath => !filePath.EndsWith(".manifest")).ToArray();
        }

        private static IEnumerator SwapShaders()
        {
            return ShaderUtil.SwapStubbedShadersAsync(_assetBundles.Values.ToArray());
        }

        private static IEnumerator SwapAddressableShaders()
        {
            return ShaderUtil.LoadAddressableMaterialShadersAsync(_assetBundles.Values.ToArray());
        }

        private static TAsset FindAsset<TAsset>(string name) where TAsset : UnityEngine.Object
        {
            TAsset loadedAsset = null;
            RevenantBundle foundInBundle = RevenantBundle.Invalid;
            foreach ((var enumVal, var assetBundle) in _assetBundles)
            {
                loadedAsset = assetBundle.LoadAsset<TAsset>(name);

                if (loadedAsset)
                {
                    foundInBundle = enumVal;
                    break;
                }
            }

#if DEBUG
            if (loadedAsset)
                RevLog.Info($"Asset of type {typeof(TAsset).Name} with name {name} was found inside bundle {foundInBundle}, it is recommended that you load the asset directly.");
            else
                RevLog.Warning($"Could not find asset of type {typeof(TAsset).Name} with name {name} in any of the bundles.");
#endif

            return loadedAsset;
        }

        private static TAsset[] FindAssets<TAsset>() where TAsset : UnityEngine.Object
        {
            List<TAsset> assets = new List<TAsset>();
            foreach ((_, var bundles) in _assetBundles)
            {
                assets.AddRange(bundles.LoadAllAssets<TAsset>());
            }

#if DEBUG
            if (assets.Count == 0)
                RevLog.Warning($"Could not find any asset of type {typeof(TAsset).Name} in any of the bundles");
#endif

            return assets.ToArray();
        }

#if DEBUG
        private static string GetCallingMethod()
        {
            var stackTrace = new StackTrace();

            for (int stackFrameIndex = 0; stackFrameIndex < stackTrace.FrameCount; stackFrameIndex++)
            {
                var frame = stackTrace.GetFrame(stackFrameIndex);
                var method = frame.GetMethod();
                if (method == null)
                    continue;

                var declaringType = method.DeclaringType;
                if (declaringType.IsGenericType && declaringType.DeclaringType == typeof(RevenantAssets))
                    continue;

                if (declaringType == typeof(RevenantAssets))
                    continue;

                var fileName = frame.GetFileName();
                var fileLineNumber = frame.GetFileLineNumber();
                var fileColumnNumber = frame.GetFileColumnNumber();

                return $"{declaringType.FullName}.{method.Name}({GetMethodParams(method)}) (fileName={fileName}, Location=L{fileLineNumber} C{fileColumnNumber})";
            }
            return "[COULD NOT GET CALLING METHOD]";
        }

        private static string GetMethodParams(MethodBase methodBase)
        {
            var parameters = methodBase.GetParameters();
            if (parameters.Length == 0)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var parameter in parameters)
            {
                stringBuilder.Append(parameter.ToString() + ", ");
            }
            return stringBuilder.ToString();
        }
#endif
    }

    /// <summary>
    /// A class that represents a request for loading Assets asynchronously.
    /// <br>You're strongly advised to use and check out <see cref="RevenantAssetRequest{TAsset}"/> instead.</br>
    /// </summary>
    public abstract class RevenantAssetRequest : IEnumerator
    {
        /// <summary>
        /// The loaded asset, boxed as a Unity Object.
        /// </summary>
        public abstract UObject boxedAsset { get; }
        /// <summary>
        /// The loaded assets, boxed as an Enumerable of Unity Object
        /// </summary>
        public abstract IEnumerable<UObject> boxedAssets { get; }

        /// <summary>
        /// The AssetBundle to load from.
        /// </summary>
        public RevenantBundle targetBundle => _targetBundle;
        private RevenantBundle _targetBundle;

        /// <summary>
        /// The name of the asset to load. Can be null in the scenario this request loads multiple assets.
        /// </summary>
        public NullableRef<string> assetName => _assetName;
        private NullableRef<string> _assetName;

        /// <summary>
        /// Wether this request is loading a single asset, or multiple assets.
        /// </summary>
        public bool singleAssetLoad { get; private set; }
        /// <summary>
        /// Checks if the asynchronous loading operation has completed.
        /// </summary>
        public bool isComplete
        {
            get 
            {
                if (internalCoroutine == null)
                    StartLoad();
                
                return !internalCoroutine.MoveNext();
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (internalCoroutine == null)
                    StartLoad();

                return internalCoroutine.Current;
            }
        }

        /// <summary>
        /// The coroutine that's loading the assets
        /// </summary>
        protected IEnumerator internalCoroutine;
        
        /// <summary>
        /// The AssetType's Name
        /// </summary>
        protected string assetTypeName;

        /// <summary>
        /// Starts the loading coroutine from this AssetRequest.
        /// </summary>
        public void StartLoad()
        {
            if (singleAssetLoad)
            {
                internalCoroutine = LoadSingleAsset();
            }
            else
            {
                internalCoroutine = LoadMultipleAsset();
            }
        }

        /// <summary>
        /// Implement the method that loads a Single asset asynchronously.
        /// </summary>
        /// <returns>A coroutine</returns>
        protected abstract IEnumerator LoadSingleAsset();

        /// <summary>
        /// Implement the method that loads multiple assets asynchronously.
        /// </summary>
        /// <returns>A coroutine</returns>
        protected abstract IEnumerator LoadMultipleAsset();

        bool IEnumerator.MoveNext()
        {
            if (internalCoroutine == null)
                StartLoad();
            return internalCoroutine.MoveNext();
        }

        void IEnumerator.Reset()
        {
            if(internalCoroutine != null)
                internalCoroutine.Reset();
        }

        /// <summary>
        /// Constructor for an MSUTAssetRequest that'll load a single asset
        /// </summary>
        /// <param name="assetName">The name of the asset</param>
        /// <param name="bundleEnum">The AssetBundle to load from, accepts the value <see cref="RevenantBundle.All"/>, but it shouldn't be used as it generates unecesary overhead</param>
        public RevenantAssetRequest(string assetName, RevenantBundle bundleEnum)
        {
            _assetName = assetName;
            _targetBundle = bundleEnum;
            singleAssetLoad = true;
            assetTypeName = "UnityEngine.Object";
        }

        /// <summary>
        /// Constructor for an MSUTAssetRequest that'll load multiple assets
        /// </summary>
        /// <param name="bundleEnum">The AssetBundle to load from, accepts the value <see cref="RevenantBundle.All"/></param>
        public RevenantAssetRequest(RevenantBundle bundleEnum)
        {
            _assetName = string.Empty;
            _targetBundle = bundleEnum;
            singleAssetLoad = false;
            assetTypeName = "UnityEngine.Object";
        }
    }

    /// <summary>
    /// A class that represents a request for loading assets of type <typeparamref name="TAsset"/> asynchronously
    /// </summary>
    /// <typeparam name="TAsset">The type of asset to load</typeparam>
    public class RevenantAssetRequest<TAsset> : RevenantAssetRequest where TAsset : UObject
    {
        public override UObject boxedAsset => _asset;
        public TAsset asset => _asset;
        private TAsset _asset;

        public override IEnumerable<UObject> boxedAssets => _assets;
        public IEnumerable<TAsset> assets => _assets;
        private List<TAsset> _assets;

        protected override IEnumerator LoadSingleAsset()
        {
            AssetBundleRequest request = null;

            request = RevenantAssets.GetAssetBundle(targetBundle).LoadAssetAsync<TAsset>(assetName); ;
            while (!request.isDone)
                yield return null;

            _asset = (TAsset)request.asset;

#if DEBUG
            //Asset found, dont try to find it.
            if (_asset)
                yield break;

            RevLog.Warning($"The method \"{GetCallingMethod()}\" is calling a MSUTAssetRequest.StartLoad() while the class has the values \"{assetTypeName}\", \"{assetName}\" and \"{targetBundle}\", however, the asset could not be found.\n" +
    $"A complete search of all the bundles will be done and the correct bundle enum will be logged.");

            RevenantBundle foundInBundle = RevenantBundle.Invalid;
            foreach (RevenantBundle bundleEnum in Enum.GetValues(typeof(RevenantBundle)))
            {
                if (bundleEnum == RevenantBundle.All || bundleEnum == RevenantBundle.Invalid || bundleEnum == RevenantBundle.StreamedScene)
                    continue;

                request = RevenantAssets.GetAssetBundle(bundleEnum).LoadAssetAsync<TAsset>(assetName);
                while (!request.isDone)
                {
                    yield return null;
                }

                if (request.asset)
                {
                    _asset = (TAsset)request.asset;
                    foundInBundle = bundleEnum;
                    break;
                }
            }

            if (_asset)
            {
                RevLog.Info($"Asset of type {assetTypeName} and name {assetName} was found inside bundle {foundInBundle}. It is recommended to load the asset directly.");
            }
            else
            {
                RevLog.Fatal($"Could not find asset of type {assetTypeName} and name {assetName} In any of the bundles, exceptions may occur.");
            }
#endif
            yield break;
        }

        protected override IEnumerator LoadMultipleAsset()
        {
            _assets.Clear();

            AssetBundleRequest request = null;
            if (targetBundle == RevenantBundle.All)
            {
                foreach (RevenantBundle enumVal in Enum.GetValues(typeof(RevenantBundle)))
                {
                    if (enumVal == RevenantBundle.All || enumVal == RevenantBundle.Invalid || enumVal == RevenantBundle.StreamedScene)
                        continue;

                    request = RevenantAssets.GetAssetBundle(targetBundle).LoadAllAssetsAsync<TAsset>();
                    while (!request.isDone)
                        yield return null;

                    _assets.AddRange(request.allAssets.OfType<TAsset>());
                }

#if DEBUG
                if (_assets.Count == 0)
                {
                    RevLog.Warning($"Could not find any asset of type {assetTypeName} in any of the bundles");
                }
#endif
                yield break;
            }

            request = RevenantAssets.GetAssetBundle(targetBundle).LoadAllAssetsAsync<TAsset>();
            while (!request.isDone) yield return null;

            _assets.AddRange(request.allAssets.OfType<TAsset>());

#if DEBUG
            if (_assets.Count == 0)
            {
                RevLog.Warning($"Could not find any asset of type {assetTypeName} inside the bundle {targetBundle}");
            }
#endif

            yield break;
        }

#if DEBUG
        private static string GetCallingMethod()
        {
            var stackTrace = new StackTrace();

            for (int stackFrameIndex = 0; stackFrameIndex < stackTrace.FrameCount; stackFrameIndex++)
            {
                var frame = stackTrace.GetFrame(stackFrameIndex);
                var method = frame.GetMethod();
                if (method == null)
                    continue;

                var declaringType = method.DeclaringType;
                if (declaringType.IsGenericType && declaringType.DeclaringType == typeof(RevenantAssets))
                    continue;

                if (declaringType == typeof(RevenantAssets))
                    continue;

                var fileName = frame.GetFileName();
                var fileLineNumber = frame.GetFileLineNumber();
                var fileColumnNumber = frame.GetFileColumnNumber();

                return $"{declaringType.FullName}.{method.Name}({GetMethodParams(method)}) (fileName={fileName}, Location=L{fileLineNumber} C{fileColumnNumber})";
            }
            return "[COULD NOT GET CALLING METHOD]";
        }

        private static string GetMethodParams(MethodBase methodBase)
        {
            var parameters = methodBase.GetParameters();
            if (parameters.Length == 0)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var parameter in parameters)
            {
                stringBuilder.Append(parameter.ToString() + ", ");
            }
            return stringBuilder.ToString();
        }
#endif

        internal RevenantAssetRequest(string name, RevenantBundle bundle) : base(name, bundle)
        {
            assetTypeName = typeof(TAsset).Name;
        }

        internal RevenantAssetRequest(RevenantBundle bundle) : base(bundle)
        {
            _assets = new List<TAsset>();
            assetTypeName = typeof(TAsset).Name;
        }
    }
}