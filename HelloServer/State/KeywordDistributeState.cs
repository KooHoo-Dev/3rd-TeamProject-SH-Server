using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class KeywordDistributeState : BaseGameTurnState
{
    protected override Type NextState => typeof(MartEnterState);
    public KeywordDistributeState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {

    }

    public override void Enter()
    {
        base.Enter();

        
        Random rnd = new Random();
        List<KeyWordDef> list = DataManager.Instance.GetKeyWordDefsByGenre(gameManager.CurrentGanre.GenreName);
 
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

            NewList.Remove(gameManager.CurrentKeyWord);
            
        }

        Protocol.TurnMessage msg = TurnMessageFactory.KeywordDistribute(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.CurrentKeyWord.KeywordId);


        foreach (var userGameInfoDic in gameManager.UserGameInfos)
        {
            
            if (userGameInfoDic.Value.IsLiar)
            {
                NewList = NewList.Except(gameManager.OldKeyWords).ToList();

                
                gameManager.CurrentLiarKeyword = NewList[rnd.Next(NewList.Count)];
                
                gameManager.OldKeyWords.Add(gameManager.CurrentLiarKeyword);

                Protocol.TurnMessage liarMsg = TurnMessageFactory.KeywordDistribute(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.CurrentLiarKeyword.KeywordId);
                SendAsync(gameManager.currentRoom.members[userGameInfoDic.Key], liarMsg);
            }
            else
            {
                SendAsync(gameManager.currentRoom.members[userGameInfoDic.Key], msg);
            }
        }
        
    }
}