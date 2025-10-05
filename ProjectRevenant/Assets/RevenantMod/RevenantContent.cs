using EntityStates;
using HG.Coroutines;
using MSU;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RevenantMod
{
    public class RevenantContent : IContentPackProvider
    {
        public string identifier => RevenantMain.GUID;

        public static ReadOnlyContentPack readOnlyContentPack => new ReadOnlyContentPack(revenantContentPack);

        internal static ContentPack revenantContentPack { get; } = new ContentPack();


        //This ParallelMultiStartCoroutine can be used to load assets BEFORE actual mod content intialization is made. 
        internal static MSU.ParallelCoroutine _parallelPreLoadDispatchers = new MSU.ParallelCoroutine();

        private static Func<IEnumerator>[] _loadDispatchers;

        internal static MSU.ParallelCoroutine _parallelPostLoadDispatchers = new MSU.ParallelCoroutine();

        private static Action[] _fieldAssignDispatchers;
        private bool _initialized;

        IEnumerator IContentPackProvider.LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            if (_initialized)
                yield break;

            _initialized = true;
            var enumerator = RevenantAssets.Initialize();
            while (enumerator.MoveNext())
                yield return null;

            while (!_parallelPreLoadDispatchers.isDone) yield return null;

            for (int i = 0; i < _loadDispatchers.Length; i++)
            {
                args.ReportProgress(Util.Remap(i + 1, 0f, _loadDispatchers.Length, 0.1f, 0.2f));
                enumerator = _loadDispatchers[i]();

                while (enumerator?.MoveNext() ?? false) yield return null;
            }

            while (!_parallelPostLoadDispatchers.isDone) yield return null;

            for (int i = 0; i < _fieldAssignDispatchers.Length; i++)
            {
                args.ReportProgress(Util.Remap(i + 1, 0f, _fieldAssignDispatchers.Length, 0.95f, 0.99f));
                _fieldAssignDispatchers[i]();
            }
        }

        IEnumerator IContentPackProvider.GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(revenantContentPack, args.output);
            args.ReportProgress(1f);
            yield return null;
        }

        IEnumerator IContentPackProvider.FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }

        private void AddSelf(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        private static IEnumerator LoadFromAssetBundles()
        {
            yield break;
        }

        private IEnumerator CallAsyncAssetLoadAttributes()
        {
            var routine = AsyncAssetLoadAttribute.CreateParallelCoroutineForMod(RevenantMain.instance);
            while (!routine.isDone)
                yield return null;
        }

        internal RevenantContent()
        {
            ContentManager.collectContentPackProviders += AddSelf;
            _parallelPreLoadDispatchers.Add(CallAsyncAssetLoadAttributes());
        }

        static RevenantContent()
        {
            RevenantMain main = RevenantMain.instance;
            _loadDispatchers = new Func<IEnumerator>[]
            {
                () => 
                {
                    CharacterModule.AddProvider(main, ContentUtil.CreateGameObjectGenericContentPieceProvider<CharacterBody>(main, revenantContentPack));
                    
                    return CharacterModule.InitializeCharacters(main);
                },
                LoadFromAssetBundles,
            };

            _fieldAssignDispatchers = new Action[]
            {
                () => ContentUtil.PopulateTypeFields(typeof(Survivors), revenantContentPack.survivorDefs),
            };
        }

        public static class Survivors
        {
            public static SurvivorDef Revenant;
        }
    }
}