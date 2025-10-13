using HG;
using RoR2.Skills;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace EntityStates.RevenantMod.Weapon
{
    /// <summary>
    /// Takes care of populating <see cref="muzzleTransformList"/> with the transforms from which we need to fire our attacks.
    /// </summary>
    public abstract class BaseLauncherState : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public static string leftMuzzleName;
        public static string rightMuzzleName;

        /// <summary>
        /// The names of the child locators from which we're firing.
        /// </summary>
        protected List<string> muzzleNameList;
        protected int step;
        public void SetStep(int i)
        {
            step = i;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            muzzleNameList = ListPool<string>.RentCollection();
            PopulateLists();
        }

        public override void OnExit()
        {
            base.OnExit();
            ListPool<string>.ReturnCollection(muzzleNameList);
        }

        private void PopulateLists()
        {
            if (muzzleNameList.Count > 0)
            {
                return;
            }

            switch (step)
            {
                case 0:
                    muzzleNameList.Add(leftMuzzleName);
                    break;
                case 1:
                    muzzleNameList.Add(rightMuzzleName);
                    break;
                case 2:
                    muzzleNameList.Add(leftMuzzleName);
                    muzzleNameList.Add(rightMuzzleName);
                    break;
            }
        }


        /// <summary>
        /// Plays the animations on the launchers depending on the step we're in.
        /// </summary>
        protected void PlayLauncherAnimation()
        {

        }
    }
}