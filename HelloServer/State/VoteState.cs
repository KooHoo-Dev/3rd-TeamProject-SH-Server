using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class VoteState : GameTurnState
{

    public VoteState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.Vote(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime || gameManager.VoteQueue.Count == (gameManager.users.Length - 1))
        {
            stateMachine.ChangeState<VoteEndState>();
        }
    }
}