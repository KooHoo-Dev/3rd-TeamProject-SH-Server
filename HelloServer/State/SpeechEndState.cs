using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class SpeechEndState : GameTurnState
{
    SpeechEndStateMessage scoreTallyEndStateMessage = new SpeechEndStateMessage();
    public SpeechEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }
    public override string GetGameStateString()
    {
        return scoreTallyEndStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<PointAtSuspectState>();
        }
    }
}