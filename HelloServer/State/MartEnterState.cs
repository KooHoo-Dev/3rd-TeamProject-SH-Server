using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class MartEnterState : GameTurnState
{
    private MartEnterStateMessage martEnterStateMessage = new MartEnterStateMessage();     
    public MartEnterState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }
    public override string GetGameStateString()
    {
        return martEnterStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartMoveState>();
        }
    }
}