using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class FinalResultEndState : GameTurnState
{
    FinalResultEndStateMessage finalResultEndStateMessage = new FinalResultEndStateMessage();
    public FinalResultEndState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        finalResultEndStateMessage.CurrentCycle = gameManager.currentCycle;
        finalResultEndStateMessage.CurrentRound = gameManager.currentRound;
        finalResultEndStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(finalResultEndStateMessage);
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
             gameManager.GameEnd();

        }
    }
}