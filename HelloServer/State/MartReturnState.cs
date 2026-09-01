using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class MartReturnState : GameTurnState
{
    MartReturnStateMessage martReturnStateMessage = new MartReturnStateMessage();
    public MartReturnState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        martReturnStateMessage.CurrentCycle = gameManager.currentCycle;
        martReturnStateMessage.CurrentRound = gameManager.currentRound;
        martReturnStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(martReturnStateMessage);
    }

    public override string GetGameStateString()
    {
        return martReturnStateMessage.Type;
    }
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ShowItemAndSpeakState>();
        }
    }
}