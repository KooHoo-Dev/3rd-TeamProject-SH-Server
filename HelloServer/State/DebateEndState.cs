using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class DebateEndState: GameTurnState
{
    private DebateEndStateMessage debateEndStateMessage = new DebateEndStateMessage();
    public DebateEndState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        debateEndStateMessage.CurrentRound = gameManager.currentRound;
        debateEndStateMessage.TimerMs = MaxMsTime;
        debateEndStateMessage.CurrentCycle =  gameManager.currentCycle;
        BroadcastAsync(debateEndStateMessage);
    }

    public override string GetGameStateString()
    {
        return debateEndStateMessage.Type;
    }
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<VoteState>();
        }
    }
}