using BepInEx;
using MSU;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RevenantMod
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class RevenantMain : BaseUnityPlugin
    {
        public const string GUID = "com.Nebby1999.RevenantMod";
        public const string VERSION = "0.0.1";
        public const string NAME = "Revenant Mod";

        //Singleton access pattern to our instance.
        internal static RevenantMain instance { get; private set; }

        private void Awake()
        {
            instance = this;

            new RevLog(Logger);
            new RevenantConfig(this);

            new RevenantContent();

            LanguageFileLoader.AddLanguageFilesFromMod(this, "RevenantLang");
        }
    }
}