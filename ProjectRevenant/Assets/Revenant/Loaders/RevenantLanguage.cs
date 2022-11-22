using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm.Loaders;

namespace RevenantMod
{
    public class RevenantLanguage : LanguageLoader<RevenantLanguage>
    {
        public override string LanguagesFolderName => "RevenantLanguage";
        public override string AssemblyDir => RevenantAssets.Instance.AssemblyDir;

        internal void Init()
        {
            LoadLanguages();
        }
    }
}