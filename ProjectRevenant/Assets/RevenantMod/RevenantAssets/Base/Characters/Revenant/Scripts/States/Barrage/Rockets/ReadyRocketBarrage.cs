namespace EntityStates.RevenantMod
{
    public class ReadyRocketBarrage : BaseReadyBarrageState
    {
        protected override BaseFireBarrageState GetFiringState()
        {
            return new FireRocketBarrage();
        }
    }
}