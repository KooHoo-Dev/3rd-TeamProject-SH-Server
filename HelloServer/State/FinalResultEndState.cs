using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class FinalResultEndState : GameTurnState
{
    FinalResultEndStateMessage debateEndStateMessage = new FinalResultEndStateMessage();
    public FinalResultEndState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
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
             gameManager.GameEnd();
        }
    }
}