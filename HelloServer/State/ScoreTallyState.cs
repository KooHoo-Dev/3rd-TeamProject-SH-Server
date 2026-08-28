using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class ScoreTallyState : GameTurnState
{
    ScoreTallyStateMessage scoreTallyStateMessage = new ScoreTallyStateMessage();  
    public ScoreTallyState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        scoreTallyStateMessage.CurrentCycle = gameManager.currentCycle;
        scoreTallyStateMessage.CurrentRound = gameManager.currentRound;
        scoreTallyStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(scoreTallyStateMessage);
    }

    public override string GetGameStateString()
    {
        return  scoreTallyStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ScoreTallyEndState>();
        }
    }
}