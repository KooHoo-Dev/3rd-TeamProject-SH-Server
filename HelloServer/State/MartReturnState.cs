using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class MartReturnState : GameTurnState
{
    MartReturnStateMessage martMoveStateMessage;
    public MartReturnState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }
    public override string GetGameStateString()
    {
        return martMoveStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ShowItemAndSpeakState>();
        }
    }
}