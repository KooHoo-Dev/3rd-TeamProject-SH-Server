using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class DebateTimeState : GameTurnState
{
    DebateEndStateMessage ebateEndStateMessage = new DebateEndStateMessage();
    public DebateTimeState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
    }
    
    public override string GetGameStateString()
    {
        return ebateEndStateMessage.Type;
    }
    
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<VoteState>();
        }
    }
}