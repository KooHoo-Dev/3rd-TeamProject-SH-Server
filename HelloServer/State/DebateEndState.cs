using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class DebateEndState: GameTurnState
{
    private DebateEndStateMessage debateEndStateMessage = new DebateEndStateMessage();
    public DebateEndState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override string GetGameStateString()
    {
        return debateEndStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<DebateEndState>();
        }
    }
}