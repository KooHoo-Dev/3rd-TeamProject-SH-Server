using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class FinalResultState : GameTurnState
{
    FinalResultEndStateMessage finalResultEndStateMessage = new FinalResultEndStateMessage();
    public FinalResultState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }
    public override string GetGameStateString()
    {
        return finalResultEndStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<FinalResultState>();
        }
    }
}