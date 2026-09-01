using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class MartEnterState : GameTurnState
{
    private MartEnterStateMessage martEnterStateMessage = new MartEnterStateMessage();     
    public MartEnterState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        martEnterStateMessage.CurrentCycle = gameManager.currentCycle;
        martEnterStateMessage.CurrentRound = gameManager.currentRound;
        martEnterStateMessage.TimerMs = MaxMsTime;
        
        BroadcastAsync(martEnterStateMessage);
    }

    public override string GetGameStateString()
    {
        return martEnterStateMessage.Type;
    }
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartMoveState>();
        }
    }
}