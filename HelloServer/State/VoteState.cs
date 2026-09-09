using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class VoteState : GameTurnState
{

    public VoteState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        gameManager.VoteQueue.Clear();
        BroadcastAsync(TurnMessageFactory.Vote(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }


    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
        if (currentMsTime > MaxMsTime || gameManager.VoteQueue.Count == (gameManager.UserGameInfos.Count - 1))
        {
            stateMachine.ChangeState<VoteEndState>();
        }
    }
}