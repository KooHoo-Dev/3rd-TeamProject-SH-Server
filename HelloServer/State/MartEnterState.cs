using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartEnterState : BaseGameTurnState
{
    protected override Type NextState => typeof(MartMoveState);
    public MartEnterState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        gameManager.MartItemsClear();
        gameManager.itemOwnersDic.Clear();
        Random random = new Random();
        int randomIndex;
        
        Dictionary<CategoryType, List<string>> AllItemIds = new Dictionary<CategoryType, List<string>>();

        for (int i = 0; i <  gameManager.AllCategories.Length; i++)
        {

            CategoryType currentCategory =
                gameManager.AllCategories[(i) % gameManager.AllCategories.Length];

            List<ItemDef> ItemIist = new List<ItemDef>(DataManager.Instance.GetItemDefsByCategory(currentCategory));
            int MaxItemCount = Math.Min(ItemIist.Count, gameManager.currentRoom.GameConfig.MaxCategoryItemCount);
            List<string> ResultItemList = new List<string>();
            for (int j = MaxItemCount - 1; j >= 0; j--)
            {

                int index = random.Next(ItemIist.Count);
                ResultItemList.Add(ItemIist[index].ItemId.ToString());
                gameManager.AllMartItems[currentCategory].Enqueue(ItemIist[index].ItemId.ToString());
                
                ItemIist.RemoveAt(index);
            }
            AllItemIds.Add(currentCategory, ResultItemList);

        }
        Protocol.CategoryItemArray[] sendArrays = new Protocol.CategoryItemArray[AllItemIds.Count];
        int counter = 0;
        foreach (var allItemIdsDic in AllItemIds)
        {
            
            sendArrays[counter] = new Protocol.CategoryItemArray();
            sendArrays[counter].Category = allItemIdsDic.Key;
            sendArrays[counter].ItemIds = new string[allItemIdsDic.Value.Count];
            sendArrays[counter].ItemIds = allItemIdsDic.Value.ToArray();

            counter++;
        }

        
       foreach (var userInfo in gameManager.UserGameInfos.Values)
       {
           
           userInfo.ItemIds = new string[gameManager.AllCategories.Length];
            randomIndex = random.Next(gameManager.AllCategories.Length);
            List<string> list = AllItemIds[gameManager.AllCategories[randomIndex]];

           gameManager.QuestInfo[userInfo.user.Id] = list[random.Next(list.Count)];
            SendAsync(gameManager.currentRoom.members[userInfo.user.Id],TurnMessageFactory.MartEnter(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.QuestInfo[userInfo.user.Id],sendArrays));
           
       }
    }
    
}