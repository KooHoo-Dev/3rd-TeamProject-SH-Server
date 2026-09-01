using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class DebateEndState: GameTurnState
{

    public DebateEndState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.DebateEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
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