using BepInEx;
using BepInEx.Configuration;
using MSU.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenantMod
{
    public class RevenantConfig
    {
        public const string PREFIX = "RevenantMod.";
        public const string ID_MAIN = PREFIX + "Main";

        internal static ConfigFactory configFactory { get; private set; }

        public static ConfigFile configMain { get; private set; }
        public static ConfigFile configItems { get; private set; }
        public static ConfigFile configEquipments { get; private set; }

        internal static IEnumerator RegisterToModSettingsManager()
        {
            yield break;
        }

        internal RevenantConfig(BaseUnityPlugin bup)
        {
            configFactory = new ConfigFactory(bup, true);
            configMain = configFactory.CreateConfigFile(ID_MAIN, true);
        }
    }
}