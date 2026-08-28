using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class LiarKeywordGuessEndState : GameTurnState
{
    LiarKeywordGuessEndStateMessage liarKeywordGuessEndStateMessage = new LiarKeywordGuessEndStateMessage();
    public LiarKeywordGuessEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        liarKeywordGuessEndStateMessage.CurrentRound = gameManager.currentRound;
        liarKeywordGuessEndStateMessage.CurrentCycle = gameManager.currentCycle;
        liarKeywordGuessEndStateMessage.TimerMs = MaxMsTime;
        liarKeywordGuessEndStateMessage.IsRightAnswer = gameManager.CurrentKeyWord.KeywordName  == gameManager.LiarGuessKeyWord;
        liarKeywordGuessEndStateMessage.liarKeyword = gameManager.CurrentLiarKeyword.KeywordName;
        liarKeywordGuessEndStateMessage.nomalKeyword = gameManager.CurrentKeyWord.KeywordName;
        BroadcastAsync(liarKeywordGuessEndStateMessage);
        gameManager.LiarGuessKeyWord = "";
    }

    public override string GetGameStateString()
    {
        return liarKeywordGuessEndStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime && gameManager.currentRound >= gameManager.currentRoom.GameConfig.MaxRound)
        {
            stateMachine.ChangeState<ScoreTallyState>();
        }
        else if (currentMsTime > MaxMsTime && gameManager.currentRound < gameManager.currentRoom.GameConfig.MaxRound)
        {
            stateMachine.ChangeState<KeywordDistributeState>();
        }
    }
}