using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartMoveState : BaseGameTurnState
{
    protected override Type NextState => typeof(MartReturnState);
    public MartMoveState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.MartMove(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }
    
}