using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class LiarKeywordGuessState : GameTurnState
{
    private LiarKeywordGuessStateMessage liarKeywordGuessStateMessage = new LiarKeywordGuessStateMessage();
    public LiarKeywordGuessState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        liarKeywordGuessStateMessage.CurrentCycle = gameManager.currentCycle;
        liarKeywordGuessStateMessage.CurrentRound = gameManager.currentRound;
        liarKeywordGuessStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(liarKeywordGuessStateMessage);
    }

    public override string GetGameStateString()
    {
        return liarKeywordGuessStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime || string.IsNullOrEmpty(gameManager.LiarGuessKeyWord) == false)
        {
            stateMachine.ChangeState<LiarKeywordGuessEndState>();
        }
    }
}