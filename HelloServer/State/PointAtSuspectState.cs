using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class PointAtSuspectState : GameTurnState
{
    private PointAtSuspectStateMessage pointAtSuspectStateMessage = new PointAtSuspectStateMessage();    
    public PointAtSuspectState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }
    public override string GetGameStateString()
    {
        return pointAtSuspectStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        { 
            stateMachine.ChangeState<PointAtSuspectEndState>();
        }
    }
}