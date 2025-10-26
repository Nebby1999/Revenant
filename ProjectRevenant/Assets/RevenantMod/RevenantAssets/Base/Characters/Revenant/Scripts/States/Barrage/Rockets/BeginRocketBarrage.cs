namespace EntityStates.RevenantMod
{
    public class BeginRocketBarrage : BaseBeginBarrageState
    {
        protected override BaseReadyBarrageState GetReadyBarrageState()
        {
            return new ReadyRocketBarrage();
        }
    }
}