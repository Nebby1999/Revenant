using EntityStates.RevenantMod.Weapon;

namespace EntityStates.RevenantMod
{
    public class BeginLaserBarrage : BaseBeginBarrageState
    {
        protected override BaseReadyBarrageState GetReadyBarrageState()
        {
            return new ReadyLaserBarrage();
        }
    }
}