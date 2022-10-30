using BepInEx;
using R2API;
using R2API.ScriptableObjects;
using R2API.Utils;
using R2API.ContentManagement;
using UnityEngine;
using Moonstorm;
using System.Security.Permissions;
using System.Security;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618
[module: UnverifiableCode]

namespace Revenant
{
    [BepInDependency("com.TeamMoonstorm.MoonstormSharedUtils", BepInDependency.DependencyFlags.HardDependency)]
	[BepInPlugin(GUID, MODNAME, VERSION)]
	public class RevenantMain : BaseUnityPlugin
	{
		public const string GUID = "com.Nebby.Revenant";
		public const string MODNAME = "Revenant";
		public const string VERSION = "0.0.1";

		public static RevenantMain Instance { get; private set; }

		private void Awake()
		{
			Instance = this;
			new RevLog(Logger);

			new RevenantAssets().Init();
			new RevenantConfig();
			new RevenantLanguage().Init();
			new RevenantContent().Init();

			TokenModifierManager.AddToManager();
			ConfigurableFieldManager.AddMod(this);
		}	
	}
}