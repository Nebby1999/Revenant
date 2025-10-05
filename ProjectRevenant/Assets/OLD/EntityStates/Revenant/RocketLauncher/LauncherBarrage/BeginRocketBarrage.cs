using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStates.Revenant.RocketLauncher
{
    public class BeginRocketBarrage : BaseBeginRocketLauncherBarrage
    {
        protected override RevenantRocketLauncherBaseState InstantiateNextState()
        {
            return new RocketBarrage();
        }
    }
}
