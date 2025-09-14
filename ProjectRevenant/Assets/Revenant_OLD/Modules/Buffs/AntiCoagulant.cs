using MSU;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenantMod.Buffs
{
    public class AntiCoagulant //: BuffBase
    {
        /*public override BuffDef BuffDef => RevenantAssets.LoadAsset<BuffDef>("bdAntiCoagulant");

        private static float durationCoefficient = 0.1f;
        private static float damageCoefficient = 0.05f;
        private static bool increaseBleedEffectiveness = true;
        private static bool increaseHemorrhageEffectiveness = true;

        public override void Initialize()
        {
            base.Initialize();
            On.RoR2.DotController.InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1 += IncreaseBleedDuration;
        }

        private void IncreaseBleedDuration(On.RoR2.DotController.orig_InflictDot_GameObject_GameObject_DotIndex_float_float_Nullable1 orig, UnityEngine.GameObject victimObject, UnityEngine.GameObject attackerObject, DotController.DotIndex dotIndex, float duration, float damageMultiplier, uint? maxStacksFromAttacker)
        {
            var origDuration = duration;
            var origDamageMult = damageMultiplier;

            if(victimObject)
            {
                var victimBody = victimObject.GetComponent<CharacterBody>();
                if(victimBody && victimBody.GetBuffCount(RevenantContent.BuffDefs.bdAntiCoagulant) > 0)
                {
                    var count = victimBody.GetBuffCount(RevenantContent.BuffDefs.bdAntiCoagulant);
                    var effectiveDurationCoefficient = 1 + (durationCoefficient * count);
                    var effectiveDamageCoefficient = 1 + (damageCoefficient * count);
                    switch(dotIndex)
                    {
                        case DotController.DotIndex.Bleed:
                            duration *= increaseBleedEffectiveness ? effectiveDurationCoefficient : 1;
                            damageMultiplier *= increaseBleedEffectiveness ? effectiveDamageCoefficient : 1;
                            break;
                        case DotController.DotIndex.SuperBleed:
                            duration *= increaseHemorrhageEffectiveness ? effectiveDurationCoefficient : 1;
                            damageMultiplier *= increaseHemorrhageEffectiveness ? effectiveDamageCoefficient : 1f;
                            break;
                    }
                }
            }
            if(dotIndex == DotController.DotIndex.Bleed || dotIndex == DotController.DotIndex.SuperBleed)
            {
                RevLog.Info($"Old Values: Dur {origDuration}; Dmg {origDamageMult}\nNew Values: Dur {duration}; Dmg {damageMultiplier}");
            }
            orig(victimObject, attackerObject, dotIndex, duration, damageMultiplier, maxStacksFromAttacker);
        }*/
    }
}
