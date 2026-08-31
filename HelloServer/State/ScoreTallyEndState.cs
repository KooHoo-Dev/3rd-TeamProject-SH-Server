using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class ScoreTallyEndState : GameTurnState
{
    private ScoreTallyEndStateMessage scoreTallyEndStateMessage = new ScoreTallyEndStateMessage();
    public ScoreTallyEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        gameManager.LiarGuessKeyWord = "";
        gameManager.PressedLiarId = "";
        gameManager.LiarOutButtonQueue.Clear();
        scoreTallyEndStateMessage.CurrentCycle = gameManager.currentCycle;
        scoreTallyEndStateMessage.CurrentRound = gameManager.currentRound;
        scoreTallyEndStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(scoreTallyEndStateMessage);
    }

    public override string GetGameStateString()
    {
        return scoreTallyEndStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime && gameManager.currentRound >= gameManager.currentRoom.GameConfig.MaxRound)
        {
            stateMachine.ChangeState<FinalResultState>();
        }
        else if (currentMsTime > MaxMsTime && gameManager.currentRound < gameManager.currentRoom.GameConfig.MaxRound)
        {
            stateMachine.ChangeState<GenreAssignAndLiarSelectState>();
        }
    }
}