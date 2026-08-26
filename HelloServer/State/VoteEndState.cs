using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class VoteEndState : GameTurnState
{
    public VoteEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }
    public override string GetGameStateString()
    {
        return gameManager.allStateString.stateVote;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<LiarConfirmedState>();
        }
    }
}