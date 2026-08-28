using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class DebateTimeState : GameTurnState
{
    DebateTimeStateMessage debateTimeStateMessage = new DebateTimeStateMessage();
    public DebateTimeState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        debateTimeStateMessage.CurrentCycle = gameManager.currentCycle;
        debateTimeStateMessage.CurrentRound = gameManager.currentRound;
        debateTimeStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(debateTimeStateMessage);
    }

    public override string GetGameStateString()
    {
        return debateTimeStateMessage.Type;
    }
    
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<VoteState>();
        }
    }
}