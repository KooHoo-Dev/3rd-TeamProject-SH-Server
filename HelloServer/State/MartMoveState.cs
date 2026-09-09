using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartMoveState : GameTurnState
{

    public MartMoveState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.MartMove(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }


    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartReturnState>();
        }
        
    }

}