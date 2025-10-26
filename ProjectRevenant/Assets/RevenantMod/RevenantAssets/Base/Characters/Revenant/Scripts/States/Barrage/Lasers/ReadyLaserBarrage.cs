namespace EntityStates.RevenantMod
{
    public class ReadyLaserBarrage : BaseReadyBarrageState
    {
        protected override BaseFireBarrageState GetFiringState()
        {
            return new FireLaserBarrage();
        }
    }
}