using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class VoteState : GameTurnState
{
    VoteStateMessage voteStateMessage = new VoteStateMessage();
    public VoteState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        voteStateMessage.CurrentRound = gameManager.currentRound;
        voteStateMessage.TimerMs = MaxMsTime;
        voteStateMessage.CurrentCycle = gameManager.currentCycle;
        BroadcastAsync(voteStateMessage);
    }

    public override string GetGameStateString()
    {
        return voteStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime || gameManager.VoteQueue.Count == (gameManager.users.Length - 1))
        {
            stateMachine.ChangeState<VoteEndState>();
        }
    }
}