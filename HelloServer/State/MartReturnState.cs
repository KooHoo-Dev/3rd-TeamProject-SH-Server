using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartReturnState : GameTurnState
{

    public MartReturnState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
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

            userItemLists[counter] = SetRandomFillItemList(VARIABLE.user.Id);

            
            Protocol.UserQuestInfo questInfo = new Protocol.UserQuestInfo();
            questInfo.UserId = VARIABLE.user.Id;
            questInfo.IsSuccess = isSuccess;
            
            userQuestInfos[counter] = questInfo;
            Console.WriteLine($"[마트 리턴 메시지 보내기 {counter}번 째] {VARIABLE.user.Id}의 차례( userItemLists 성공 여부) :  {userItemLists[counter]?.ItemList != null},");
            
            counter++;
            
        }
        
        BroadcastAsync(TurnMessageFactory.MartReturn(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,userItemLists,userQuestInfos));
    }
    
    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ShowItemAndSpeakState>();
        }
    }

    public override void Exit()
    {
        base.Exit(); 

    }

    // 아직 미 구현
    private bool IsSuccessQuest(string UserId)
    {
            bool Sueccess = false;
            Console.WriteLine($"[퀘스트 판별 함수] 퀘스트Info의 개수 :  {gameManager.QuestInfo?.Count}, 유저 Id :{UserId}");

            for (int i = 0; i < gameManager.currentRoom.GameConfig.MaxCycle; i++)
            {
                
                if (string.IsNullOrEmpty(gameManager.UserGameInfos[UserId].ItemIds[i]) == false && gameManager.UserGameInfos[UserId].ItemIds[i] == gameManager.QuestInfo[UserId])
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
        itemList.UserId = UserId;
        itemList.ItemList = new string[gameManager.AllCategories.Length];
        int counter = 0;
        foreach (var VARIABLE in gameManager.AllCategories)
        {
            List<string> martCategoryItems = gameManager.AllMartItems[VARIABLE].ToList();
            // 이미 선택된 아이템들은 걸러주는 작업
            foreach (var userInfo in gameManager.UserGameInfos.Values)
            {
                for (int i = userInfo.ItemIds.Length - 1; i >= 0; i--)
                {
                    if(string.IsNullOrEmpty(userInfo.ItemIds[i] ?? "")) continue;
                    if (martCategoryItems.Contains(userInfo.ItemIds[i]))
                    {
                        martCategoryItems.Remove(userInfo.ItemIds[i]);
                    }
                }
            }
            int randomIndex = 0;
            
            // 유저의 소유 아이템을 꺼낸다
            itemList.ItemList[counter] = gameManager.UserGameInfos[UserId].ItemIds[counter] ?? "";
            Console.WriteLine($"[원래 유저의 {VARIABLE} 카테고리의 선택 아이템 : {itemList.ItemList[counter]}]");
            // 꺼낸 아이템이 비어있거나 null이라면 마트 아이템들 중에 랜덤으로 뽑아서 채운다.
            if ( string.IsNullOrEmpty(itemList.ItemList[counter]))
            {
                if (martCategoryItems.Count == 0)
                {
                    List<ItemDef> list = DataManager.Instance.GetItemDefsByCategory(VARIABLE);
                    itemList.ItemList[counter] = list[random.Next(list.Count)].ItemId.ToString();
                }
                else
                {
                    randomIndex = random.Next(martCategoryItems.Count);
                    itemList.ItemList[counter] = martCategoryItems[randomIndex];
             
                    martCategoryItems.RemoveAt(randomIndex);
                }

            }
            

            counter++;
        }
        // 원본 저장소에 할당해준다.
        gameManager.UserGameInfos[UserId].ItemIds = itemList.ItemList;
        Console.WriteLine($"[유저 {itemList.UserId}의 최종 아이템 리스트]");
        
        for (int i = 0; i < itemList.ItemList.Length; i++)
        {
            Console.WriteLine($"{itemList.ItemList[i]} ");
        }
        return  itemList;
    }
    
}