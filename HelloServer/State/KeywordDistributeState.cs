using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class KeywordDistributeState : GameTurnState
{

    public KeywordDistributeState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {

    }

    public override void Enter()
    {
        base.Enter();

        
        Random rnd = new Random();
        List<KeyWordDef> list = DataManager.Instance.GetKeyWordDefsByGenre(gameManager.CurrentGanre.GenreName);
        // 중복되는 모든 값 리스트에서 제거
        // OldKeyWords의 ID 목록을 해시셋(HashSet)으로 만들어 검색 속도를 높입니다.
        var oldIds = new HashSet<int>(gameManager.OldKeyWords.Select(x => x.KeywordId));

        // list에서 OldKeyWords에 포함된 항목을 제외합니다.
        List<KeyWordDef> NewList = list.Except(gameManager.OldKeyWords).ToList();
        gameManager.CurrentKeyWord = NewList[rnd.Next(NewList.Count)];
        gameManager.OldKeyWords.Add(gameManager.CurrentKeyWord);
        
        Console.WriteLine($"[키워드 선정 로직] 선정된 키워드 : {gameManager.CurrentKeyWord.KeywordName}, 리스트 갯수 {NewList.Count}");
        for (int i = 0; i < NewList.Count; i++)
        {
            Console.WriteLine($"[키워드 선정 로직] 갱신된 {i}번째 리스트 : {NewList[i].KeywordName}");
        }
        for (int i = 0; i < gameManager.OldKeyWords.Count; i++)
        {
            Console.WriteLine($"[키워드 선정 로직] 올드 {i}번째 리스트 : {gameManager.OldKeyWords[i].KeywordName}");
        }

        if (NewList.Contains(gameManager.CurrentKeyWord))
        {
            Console.WriteLine($"[키워드 선정 로직] 삭제 로직 이전");
            NewList.Remove(gameManager.CurrentKeyWord);
            
        }

        Protocol.TurnMessage msg = TurnMessageFactory.KeywordDistribute(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.CurrentKeyWord.KeywordId);


        foreach (var VARIABLE in gameManager.UserGameInfos)
        {
            
            if (VARIABLE.Value.IsLiar)
            {
                Console.WriteLine($"[라이어 키워드 선정 로직] 선정된 키워드 : 1, 리스트 갯수 {NewList.Count}");
                

                gameManager.CurrentLiarKeyword = NewList[rnd.Next(NewList.Count)];
                
                gameManager.OldKeyWords.Add(gameManager.CurrentLiarKeyword);

                Protocol.TurnMessage liarMsg = TurnMessageFactory.KeywordDistribute(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.CurrentKeyWord.KeywordId);
                SendAsync(gameManager.currentRoom.members[VARIABLE.Key], liarMsg);
            }
            else
            {
                SendAsync(gameManager.currentRoom.members[VARIABLE.Key], msg);
            }
        }
        
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