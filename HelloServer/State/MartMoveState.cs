using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class MartMoveState : GameTurnState
{
    MartMoveStateMessage martMoveStateMessage = new MartMoveStateMessage();
    public MartMoveState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        martMoveStateMessage.CurrentCycle = gameManager.currentCycle;
        martMoveStateMessage.CurrentRound = gameManager.currentRound;
        martMoveStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(martMoveStateMessage);
    }

    public override string GetGameStateString()
    {
        return martMoveStateMessage.Type;
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