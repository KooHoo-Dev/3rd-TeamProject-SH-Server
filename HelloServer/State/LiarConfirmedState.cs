using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class LiarConfirmedState : GameTurnState
{
    private LiarConfirmedStateMessage liarConfirmedStateMessage = new LiarConfirmedStateMessage();  
    public LiarConfirmedState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        liarConfirmedStateMessage.CurrentCycle = gameManager.currentCycle;
        liarConfirmedStateMessage.CurrentRound = gameManager.currentRound;
        liarConfirmedStateMessage.CurrentOwnerID = gameManager.MostFrequent;
        liarConfirmedStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(liarConfirmedStateMessage);
    }

    public override string GetGameStateString()
    {
        return liarConfirmedStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<LiarKeywordGuessState>();
        }
    }
}