using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class VoteState : BaseGameTurnState
{
    protected override Type NextState => typeof(VoteEndState);
    public VoteState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        gameManager.Votes.Clear();
        BroadcastAsync(TurnMessageFactory.Vote(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }


    public override void Tick()
    {
        base.Tick();
        if (gameManager.Votes.Count >= gameManager.UserGameInfos.Count - 1)
        {
            stateMachine.ChangeState<VoteEndState>();
        }
    }
}