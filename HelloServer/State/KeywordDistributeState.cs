using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class KeywordDistributeState : GameTurnState
{
    private KeywordDistributeStateMessage keywordDistributeStateMessage;   
    public KeywordDistributeState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
        keywordDistributeStateMessage = new KeywordDistributeStateMessage();
    }

    public override void Enter()
    {
        base.Enter();

        Random rnd = new Random();
        List<KeyWordDef> list = DataManager.Instance.GetKeyWordDefsByGenre(gameManager.CurrentGanre.GenreName);
        List<KeyWordDef> NewList = list.Union(gameManager.OldKeyWords).ToList();
        gameManager.CurrentKeyWord = NewList[rnd.Next(NewList.Count)];
        gameManager.OldKeyWords.Add(gameManager.CurrentKeyWord);
        
        keywordDistributeStateMessage.CurrentCycle = gameManager.currentCycle;
        keywordDistributeStateMessage.CurrentRound = gameManager.currentRound;
        keywordDistributeStateMessage.TimerMs = MaxMsTime;

        keywordDistributeStateMessage.KeywordId = gameManager.CurrentKeyWord.KeywordId;
        BroadcastAsync(keywordDistributeStateMessage);
    }

    public override string GetGameStateString()
    {
        return keywordDistributeStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartEnterState>();
        }
    }
}