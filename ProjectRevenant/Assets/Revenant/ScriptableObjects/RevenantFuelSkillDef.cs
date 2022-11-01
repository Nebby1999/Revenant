using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Revenant.Components;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace Revenant
{
    [CreateAssetMenu(menuName = "RoR2/SkillDef/RevenantFuelSkillDef")]
    public class RevenantFuelSkillDef : SkillDef
    {
        public float fuelCost;

        private RevenantJetpackController jetpackController;
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            var baseVal = base.OnAssigned(skillSlot);
            jetpackController = skillSlot.GetComponent<RevenantJetpackController>();
            return baseVal;
        }
        public override void OnUnassigned([NotNull] GenericSkill skillSlot)
        {
            base.OnUnassigned(skillSlot);
            jetpackController = null;
        }
        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            if(!jetpackController)
            {
                RevLog.Warning($"{skillSlot} has been assigned a FuelSkillDef ({skillName}) but the GenericSkill's game object lacks a jetpack controller!");
                return base.CanExecute(skillSlot);
            }
            if(HasRequiredStockAndDelay(skillSlot) && IsReady(skillSlot) && skillSlot.stateMachine && !skillSlot.stateMachine.HasPendingState() && skillSlot.stateMachine.CanInterruptState(interruptPriority))
            {
                return jetpackController.CurrentFuel >= fuelCost;
            }
            return false;
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            bool baseVal = base.IsReady(skillSlot);
            if (!jetpackController)
            {
                RevLog.Warning($"{skillSlot} has been assigned a FuelSkillDef ({skillName}) but the GenericSkill's game object lacks a jetpack controller!");
                return baseVal;
            }
            return baseVal && jetpackController.CurrentFuel > fuelCost;
        }
        public override void OnExecute([NotNull] GenericSkill skillSlot)
        {
            base.OnExecute(skillSlot);
            if(!jetpackController)
            {
                RevLog.Warning($"{skillSlot} has been assigned a FuelSkillDef ({skillName}) but the GenericSkill's game object lacks a jetpack controller!");
                return;
            }
            jetpackController.AddFuel(-fuelCost);
        }
    }
}