using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class DebateEndState: GameTurnState
{
    public DebateEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override string GetGameStateString()
    {
        return gameManager.allStateString.stateDebateEnd;
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