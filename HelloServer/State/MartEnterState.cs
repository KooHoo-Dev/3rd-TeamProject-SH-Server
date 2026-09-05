using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartEnterState : GameTurnState
{
   
    public MartEnterState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        gameManager.MartItemsClear();
        Random random = new Random();
        int randomIndex = random.Next(gameManager.AllCategories.Length);
        
        Dictionary<CategoryType, List<string>> AllItemIds = new Dictionary<CategoryType, List<string>>();
        int itmeCounter = 0;
        foreach (var categoryType in gameManager.AllCategories)
        {

            
            List<ItemDef> ItemIist = DataManager.Instance.GetItemDefsByCategory(gameManager.AllCategories[(randomIndex + itmeCounter) % gameManager.AllCategories.Length]);
            int MaxItemCount = Math.Min(ItemIist.Count, gameManager.currentRoom.GameConfig.MaxCategoryItemCount);
            List<string> ResultItemList = new List<string>();
            for (int i = MaxItemCount - 1; i >= 0; i--)
            {

                int index = random.Next(ItemIist.Count);
                ResultItemList.Add(ItemIist[index].ItemId.ToString());
                gameManager.AllMartItems[categoryType].Enqueue(ItemIist[index].ItemId.ToString());
                
                ItemIist.RemoveAt(index);
            }
            AllItemIds.Add(gameManager.AllCategories[itmeCounter], ResultItemList);
            itmeCounter++;
        }
        Protocol.CategoryItemArray[] sendArrays = new Protocol.CategoryItemArray[AllItemIds.Count];
        int counter = 0;
        foreach (var VARIABLE in AllItemIds)
        {
            
            sendArrays[counter] = new Protocol.CategoryItemArray();
            sendArrays[counter].Category = VARIABLE.Key;
            sendArrays[counter].ItemIds = new string[VARIABLE.Value.Count];
            sendArrays[counter].ItemIds = VARIABLE.Value.ToArray();

            counter++;
        }

        
       foreach (var VARIABLE in gameManager.UserGameInfos.Values)
       {
           
           VARIABLE.ItemIds = new string[gameManager.AllCategories.Length];
            randomIndex = random.Next(gameManager.AllCategories.Length);
            List<string> list = AllItemIds[gameManager.AllCategories[randomIndex]];

           gameManager.QuestInfo[VARIABLE.user.Id] = list[random.Next(list.Count)];
            SendAsync(gameManager.currentRoom.members[VARIABLE.user.Id],TurnMessageFactory.MartEnter(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.QuestInfo[VARIABLE.user.Id],sendArrays));
           
       }
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartMoveState>();
        }
    }
}