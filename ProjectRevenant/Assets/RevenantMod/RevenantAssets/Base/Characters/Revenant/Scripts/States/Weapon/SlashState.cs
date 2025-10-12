using IL.RoR2;
using MSU;
using RevenantMod;
using RoR2.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStates.RevenantMod.Weapon
{
    public class SlashState : BasicMeleeAttack, SteppedSkillDef.IStepSetter
    {
        public static float baseDurationBeforeInterruptable;


        private int _step;
        private float _durationBeforeInterruptable;

        public override void OnEnter()
        {
            base.OnEnter();
            _durationBeforeInterruptable = baseDurationBeforeInterruptable / attackSpeedStat;
        }

        public override void AuthorityModifyOverlapAttack(RoR2.OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            RevLog.Error(overlapAttack);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (!(base.fixedAge < _durationBeforeInterruptable))
            {
                return InterruptPriority.Skill;
            }
            return InterruptPriority.Pain;
        }

        public void SetStep(int i)
        {
            _step = i;
        }
    }
}
