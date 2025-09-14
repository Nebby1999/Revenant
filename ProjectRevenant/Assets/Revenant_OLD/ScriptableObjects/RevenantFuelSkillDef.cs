using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RevenantMod.Components;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RevenantMod
{
    [CreateAssetMenu(menuName = "RoR2/SkillDef/RevenantFuelSkillDef")]
    public class RevenantFuelSkillDef : SkillDef
    {
        public class InstanceData : BaseSkillInstanceData
        {
            public float BaseCost { get; }
            public float EffectiveCost { get; internal set; }
            public RevenantController JetpackController { get; }

            public InstanceData(float fuelCost, GenericSkill genericSkill)
            {
                BaseCost = fuelCost;
                EffectiveCost = BaseCost;
                JetpackController = genericSkill.GetComponent<RevenantController>();
            }
        }
        [Tooltip("The base fuel cost for this skill")]
        public float baseFuelCost;
        [Tooltip("Minimum amount of fuel cost, the cost reduction from the fields below will never reduce the fuel cost past this limit.")]
        public float minFuelCost;
        [Tooltip("How much cost is reduced with extra skill stocks")]
        public float costReductionPerExtraStock;
        [Tooltip("How much cost is reduced or increased with cooldowns, this is applied as a multiplier to the final cost")]
        public float costCoefficientFromCooldowns;
        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            var instanceData = new InstanceData(baseFuelCost, skillSlot);
            return instanceData;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            var instanceData = (InstanceData)skillSlot.skillInstanceData;
            if(!instanceData.JetpackController)
            {
                RevLog.Warning($"{skillSlot} has been assigned a FuelSkillDef ({skillName}) but the GenericSkill's game object lacks a jetpack controller!");
                return base.CanExecute(skillSlot);
            }
            if(HasRequiredStockAndDelay(skillSlot) && IsReady(skillSlot) && skillSlot.stateMachine && !skillSlot.stateMachine.HasPendingState() && skillSlot.stateMachine.CanInterruptState(interruptPriority))
            {
                return instanceData.JetpackController.CurrentFuel >= instanceData.EffectiveCost;
            }
            return false;
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            bool baseVal = base.IsReady(skillSlot);
            var instanceData = (InstanceData)skillSlot.skillInstanceData;
            if (!instanceData.JetpackController)
            {
                RevLog.Warning($"{skillSlot} has been assigned a FuelSkillDef ({skillName}) but the GenericSkill's game object lacks a jetpack controller!");
                return base.CanExecute(skillSlot);
            }
            return baseVal && instanceData.JetpackController.CurrentFuel > instanceData.EffectiveCost;
        }
        public override void OnExecute([NotNull] GenericSkill skillSlot)
        {
            base.OnExecute(skillSlot);
            var instanceData = (InstanceData)skillSlot.skillInstanceData;
            if (!instanceData.JetpackController)
            {
                RevLog.Warning($"{skillSlot} has been assigned a FuelSkillDef ({skillName}) but the GenericSkill's game object lacks a jetpack controller!");
                return;
            }
            instanceData.JetpackController.AddFuel(-instanceData.EffectiveCost);
        }
    }
}