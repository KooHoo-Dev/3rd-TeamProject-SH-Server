using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class DebateEndState: BaseGameTurnState
{
    protected override Type NextState => typeof(VoteState);
    public DebateEndState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.DebateEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }
}