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
        // 중복되는 모든 값 리스트에서 제거
        List<KeyWordDef> NewList = list.Concat(gameManager.OldKeyWords)
            .GroupBy(x => x)
            .Where(g => g.Count() == 1)
            .Select(g => g.Key)
            .ToList();
        gameManager.CurrentKeyWord = NewList[rnd.Next(NewList.Count)];
        Console.WriteLine($"[키워드 선정 로직] 선정된 키워드 : {gameManager.CurrentKeyWord.KeywordName}, 리스트 갯수 {NewList.Count}");
        
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
                gameManager.CurrentLiarKeyword = NewList[rnd.Next(NewList.Count)];
                gameManager.OldKeyWords.Add(gameManager.CurrentLiarKeyword);
                LiarMessage.CurrentCycle = gameManager.currentCycle;
                LiarMessage.CurrentRound = gameManager.currentRound;
                LiarMessage.TimerMs = MaxMsTime;

                LiarMessage.KeywordId = gameManager.CurrentLiarKeyword.KeywordId;
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
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartEnterState>();
        }
    }
}