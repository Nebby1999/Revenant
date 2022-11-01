using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonstorm;
using UnityEngine;

namespace EntityStates.Revenant.Weapon
{
    public class PunchState : BasicMeleeAttack
    {
		private const string tkn = "REV_REVENANT_SECONDARY_PUNCH_DESC";
		public static float baseDurationBeforeInterruptable;
		private float durationBeforeInterruptable;

		//For use in the token modifier, not used in the state
        [TokenModifier(tkn, StatTypes.Percentage, 0)]
		[HideInInspector] public static float dmgCoef;
        public override void OnEnter()
        {
			dmgCoef = damageCoefficient;
            base.OnEnter();
			durationBeforeInterruptable = baseDurationBeforeInterruptable / attackSpeedStat;
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
			overlapAttack.damageType = DamageType.Stun1s;
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
