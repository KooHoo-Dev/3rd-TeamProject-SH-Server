using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartMoveState : GameTurnState
{

    public MartMoveState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.MartMove(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartReturnState>();
        }
        
    }
}