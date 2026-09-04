using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartReturnState : GameTurnState
{

    public MartReturnState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        gameManager.RemovePlayerSelectedItemFromBag();
        
        Protocol.UserItemList[] userItemLists = new Protocol.UserItemList[gameManager.UserGameInfos.Count];
        Protocol.UserQuestInfo[] userQuestInfos = new Protocol.UserQuestInfo[gameManager.UserGameInfos.Count];
        int counter = 0;
        foreach (var VARIABLE in gameManager.UserGameInfos.Values)
        {
            
            
            
            
            bool isSuccess = IsSuccessQuest(VARIABLE.user.Id);
            Console.WriteLine($"[마트 리턴 메시지 보내기] {VARIABLE.user.Id}의 차례(성공 여부) :  {isSuccess}");
            Protocol.UserQuestInfo questInfo = new Protocol.UserQuestInfo();
            questInfo.UserId = VARIABLE.user.Id;
            questInfo.IsSuccess = isSuccess;
            
            userQuestInfos[counter] = questInfo;
            
            
            Protocol.UserItemList itemList = new Protocol.UserItemList();
            
            
            counter++;
        }
    }
    
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ShowItemAndSpeakState>();
        }
    }

    public override void Exit()
    {
        base.Exit(); 
        gameManager.MartItemsClear();
    }

    // 아직 미 구현
    private bool IsSuccessQuest(string UserId)
    {
            bool Sueccess = false;
            Console.WriteLine($"[퀘스트 판별 함수] 퀘스트Info의 개수 :  {gameManager.QuestInfo?.Count}, 유저 Id :{UserId}");

            for (int i = 0; i < gameManager.currentRoom.GameConfig.MaxCycle; i++)
            {
                
                if (gameManager.UserGameInfos[UserId].ItemIds[i] != null && gameManager.UserGameInfos[UserId].ItemIds[i] == gameManager.QuestInfo[UserId])
                {
                    Sueccess = true;
                    gameManager.UserGameInfos[UserId].IsQuestSuccess = true;
                    break;
                }
            }
        
        return Sueccess;
        
    }

    private Protocol.UserItemList SetRandomFillItemList(string UserId)
    {
        Random random = new Random();
        Protocol.UserItemList itemList = new Protocol.UserItemList();
        foreach (var VARIABLE in gameManager.AllCategories)
        {

        List<string> martCategoryItems = gameManager.AllMartItems[VARIABLE].ToList();
        itemList.UserId = UserId;
        itemList.ItemList = new string[gameManager.AllCategories.Length];

        

        int randomIndex = 0;
        for (int i = 0; i < gameManager.AllCategories.Length; i++)
        {
            itemList.ItemList[i] = gameManager.UserGameInfos[UserId].ItemIds[i] ?? "";
            if (i <= martCategoryItems.Count && string.IsNullOrEmpty(itemList.ItemList[i]))
            {
             randomIndex = random.Next(martCategoryItems.Count);
             itemList.ItemList[i] = martCategoryItems[randomIndex];
             
             martCategoryItems.RemoveAt(randomIndex);
            }
        }
        
        gameManager.UserGameInfos[UserId].ItemIds = itemList.ItemList;
                    
        }
        return  itemList;
    }
    
}