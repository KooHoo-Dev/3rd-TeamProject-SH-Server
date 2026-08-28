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

        gameManager.currentRound++;
        Random rnd = new Random();
        List<KeyWordDef> list = DataManager.Instance.GetKeyWordDefsByGenre(gameManager.CurrentGanre.GenreName);
        List<KeyWordDef> NewList = list.Union(gameManager.OldKeyWords).ToList();
        gameManager.CurrentKeyWord = NewList[rnd.Next(NewList.Count)];
        gameManager.OldKeyWords.Add(gameManager.CurrentKeyWord);
        NewList.Remove(gameManager.CurrentKeyWord);
        keywordDistributeStateMessage.CurrentCycle = gameManager.currentCycle;
        keywordDistributeStateMessage.CurrentRound = gameManager.currentRound;
        keywordDistributeStateMessage.TimerMs = MaxMsTime;

        keywordDistributeStateMessage.KeywordId = gameManager.CurrentKeyWord.KeywordId;
        
        for (int i = 0; i < gameManager.users.Length; i++)
        {
            
            if (gameManager.currentRoom.members[gameManager.users[i].Id].playerState.IsLiar)
            {
                KeywordDistributeStateMessage LiarMessage = new  KeywordDistributeStateMessage();
                List<KeyWordDef> Liarlist = DataManager.Instance.GetKeyWordDefsByGenre(gameManager.CurrentGanre.GenreName);
                List<KeyWordDef> LiarNewList = Liarlist.Union(gameManager.OldKeyWords).ToList();
                gameManager.CurrentLiarKeyword = LiarNewList[rnd.Next(LiarNewList.Count)];
                gameManager.OldKeyWords.Add(gameManager.CurrentLiarKeyword);
                LiarNewList.Remove(gameManager.CurrentKeyWord);
                LiarMessage.CurrentCycle = gameManager.currentCycle;
                LiarMessage.CurrentRound = gameManager.currentRound;
                LiarMessage.TimerMs = MaxMsTime;

                LiarMessage.KeywordId = gameManager.CurrentKeyWord.KeywordId;
                SendAsync(gameManager.currentRoom.members[gameManager.users[i].Id], LiarMessage);
            }
            else
            {
                SendAsync(gameManager.currentRoom.members[gameManager.users[i].Id], keywordDistributeStateMessage);
            }
        }
        
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