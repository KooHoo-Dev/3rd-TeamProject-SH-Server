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
        // OldKeyWords의 ID 목록을 해시셋(HashSet)으로 만들어 검색 속도를 높입니다.
        var oldIds = new HashSet<int>(gameManager.OldKeyWords.Select(x => x.KeywordId));

        // list 중에서 oldIds에 포함되지 않은 ID를 가진 객체만 필터링합니다.
        List<KeyWordDef> NewList = list.Where(x => !oldIds.Contains(x.KeywordId)).ToList();
        gameManager.CurrentKeyWord = NewList[rnd.Next(NewList.Count)];
        gameManager.OldKeyWords.Add(gameManager.CurrentKeyWord);
        
        Console.WriteLine($"[키워드 선정 로직] 선정된 키워드 : {gameManager.CurrentKeyWord.KeywordName}, 리스트 갯수 {NewList.Count}");
        for (int i = 0; i < NewList.Count; i++)
        {
            Console.WriteLine($"[키워드 선정 로직] 갱신된 {i}번째 리스트 : {NewList[i].KeywordName}");
        }
        for (int i = 0; i < NewList.Count; i++)
        {
            Console.WriteLine($"[키워드 선정 로직] 올드 {i}번째 리스트 : {gameManager.OldKeyWords[i].KeywordName}");
        }
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