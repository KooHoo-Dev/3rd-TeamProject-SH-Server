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
        scoreTallyStateMessage.LiarOutButtonInfo = new string[gameManager.LiarOutButtonQueue.Count];
        int count = gameManager.LiarOutButtonQueue.Count;
        for (int i = 0; i < count; i++)
        {
             gameManager.LiarOutButtonQueue.TryDequeue(out scoreTallyStateMessage.LiarOutButtonInfo[i]);
        }
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