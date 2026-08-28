using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class SpeechEndState : GameTurnState
{
    SpeechEndStateMessage speechEndStateMessage = new SpeechEndStateMessage();

    public SpeechEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
     
        speechEndStateMessage.CurrentCycle = gameManager.currentCycle;
        speechEndStateMessage.CurrentRound = gameManager.currentRound;
        speechEndStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(speechEndStateMessage);
    }

    public override string GetGameStateString()
    {
        return speechEndStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ShowItemAndSpeakState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
            gameManager.currentSpeakedCount++;
    }
}