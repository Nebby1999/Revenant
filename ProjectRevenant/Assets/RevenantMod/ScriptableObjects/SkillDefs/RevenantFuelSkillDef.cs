using EntityStates;
using JetBrains.Annotations;
using RevenantMod.Survivors;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace RevenantMod
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Revenant/Skills/RevenantFuelSkillDef")]
#endif
    public class RevenantFuelSkillDef : SkillDef
    {
        public class InstanceData : BaseSkillInstanceData
        {
            public RevenantFuelController fuelController { get; private set; }
            public int step;
            public float stepResetTimer;
            public InstanceData(GenericSkill genericSkill)
            {
                fuelController = genericSkill.GetComponent<RevenantFuelController>();
            }
        }
        [Tooltip("If true, Revenant can utilize this skill even if it means entering his penalty.")]
        public bool allowEnteringPenalty;
        [Tooltip("The amount of fuel that costs firing this skill if you have no stocks left.")]
        public float overuseFuelCost;

        [Header("Stepped Skilldef impl")]
        public int stepCount = -1;
        public float stepGraceDuration = 0.1f;

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            InstanceData instanceData = new InstanceData(skillSlot);
            return instanceData;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            var instanceData = (InstanceData)skillSlot.skillInstanceData;
            if(!instanceData.fuelController)
            {
                return base.CanExecute(skillSlot);
            }

            if (!skillSlot.stateMachine)
                return false;

            if (skillSlot.stateMachine.HasPendingState())
                return false;

            if(HasRequiredStockOrFuel(skillSlot, instanceData))
            {
                return skillSlot.stateMachine.CanInterruptState(interruptPriority);
            }
            return false;
        }

        public override EntityState InstantiateNextState([NotNull] GenericSkill skillSlot)
        {
            EntityState entityState = base.InstantiateNextState(skillSlot);
            InstanceData instanceData = (InstanceData)skillSlot.skillInstanceData;
            if(entityState is SteppedSkillDef.IStepSetter stepSetter)
            {
                stepSetter.SetStep(instanceData.step);
            }

            return entityState;
        }

        private bool HasRequiredStockOrFuel(GenericSkill genericSkill, InstanceData instanceData)
        {
            bool hasStocks = genericSkill.stock >= requiredStock;
            if(hasStocks)
            {
                return true;
            }

            //If we're in penalty, we cant use fuel instead.
            if(instanceData.fuelController.isInPenalty)
            {
                return false;
            }

            //If we dont have stocks, calculate how much fuel it'd cost to execute anyways.
            float requiredFuelForOveruse = CalculateFuelCost(genericSkill.stock);
            float fuelAfterBeingSpent = instanceData.fuelController.currentFuel - requiredFuelForOveruse;

            //If we can afford the fuel spend, return true, if we cannot, only return true if we're allowing entering the penalty.
            if(fuelAfterBeingSpent >= Mathf.Epsilon || allowEnteringPenalty)
            {
                return true;
            }

            return false;
        }

        public override void OnExecute([NotNull] GenericSkill skillSlot)
        {
            //Store the prior amount of stocks, we'll use this to calculate if we've gone in debt
            int stocksPriorToExecution = skillSlot.stock;

            base.OnExecute(skillSlot);

            var instanceData = (InstanceData)skillSlot.skillInstanceData;

            if(stepCount > 0)
            {
                instanceData.step++;
                if(instanceData.step >= stepCount)
                {
                    instanceData.step = 0;
                }
            }

            //We've gone into debt, idk if the game ever intended this, so just set to 0 for good luck.
            if(skillSlot.stock < 0)
            {
                skillSlot.stock = 0;
                
                if(!instanceData.fuelController)
                {
                    return;
                }

                float fuelCost = CalculateFuelCost(stocksPriorToExecution);
                
                if(NetworkServer.active)
                {
                    instanceData.fuelController.SpendFuel(fuelCost);
                }
                else
                {
                    instanceData.fuelController.CmdSpendFuel(fuelCost);
                }
            }
        }

        public override void OnFixedUpdate([NotNull] GenericSkill skillSlot, float deltaTime)
        {
            base.OnFixedUpdate(skillSlot, deltaTime);
            InstanceData data = (InstanceData)skillSlot.skillInstanceData;
            if(skillSlot.CanExecute())
            {
                data.stepResetTimer += deltaTime;
            }
            else
            {
                data.stepResetTimer = 0f;
            }

            if(data.stepResetTimer > stepGraceDuration)
            {
                data.step = 0;
            }
        }

        private float CalculateFuelCost(int currentStocks)
        {
            int missingStocks = requiredStock - currentStocks;
            float requiredFuelForOveruse = missingStocks * overuseFuelCost;
            return requiredFuelForOveruse;
        }
    }
}