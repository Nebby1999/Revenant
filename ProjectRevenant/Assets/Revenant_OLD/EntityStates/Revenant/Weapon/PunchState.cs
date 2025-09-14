using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSU;
using R2API;
using UnityEngine;

namespace EntityStates.Revenant.Weapon
{
    public class PunchState : BasicMeleeAttack
    {
		private const string tkn = "REV_REVENANT_SECONDARY_PUNCH_DESC";
		public static float baseDurationBeforeInterruptable;
		private float durationBeforeInterruptable;

		//For use in the token modifier, not used in the state
        [FormatToken(tkn, FormatTokenAttribute.OperationTypeEnum.MultiplyByN, 100, 0)]
		public static float TokenModifier_DmgCoef => new PunchState().damageCoefficient;
        public override void OnEnter()
        {
            base.OnEnter();
			durationBeforeInterruptable = baseDurationBeforeInterruptable / attackSpeedStat;
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
			//FIXME
			//overlapAttack.AddModdedDamageType(RevenantMod.DamageTypes.AntiCoagulant.dtAntiCoagulant);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (!(base.fixedAge < durationBeforeInterruptable))
			{
				return InterruptPriority.Skill;
			}
			return InterruptPriority.Pain;
		}
	}
}
