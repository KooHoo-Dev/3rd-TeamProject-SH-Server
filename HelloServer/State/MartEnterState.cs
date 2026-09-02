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
        Random random = new Random();
        int randomIndex = random.Next(gameManager.AllCategories.Length);
        
        Dictionary<CategoryType, List<string>> AllItemIds = new Dictionary<CategoryType, List<string>>();
  
        for (int i = 0; i < gameManager.AllCategories.Length; i++)
        {
            List<ItemDef> ItemIist = DataManager.Instance.GetItemDefsByCategory(gameManager.AllCategories[randomIndex]);
            int MaxItemCount = Math.Min(ItemIist.Count, gameManager.currentRoom.GameConfig.MaxCategoryItemCount);
            List<string> ResultItemList = new List<string>();
            for (int j = 0; j < MaxItemCount; j++)
            {
                int index = random.Next(ItemIist.Count);
                ResultItemList.Add(ItemIist[index].ItemId.ToString());
                ItemIist.RemoveAt(index);
            }
            AllItemIds.Add(gameManager.AllCategories[i], ResultItemList);
            
        }

        Protocol.CategoryItemArray[] sendArrays = new Protocol.CategoryItemArray[AllItemIds.Count];
        int counter = 0;
        foreach (var VARIABLE in AllItemIds)
        {
            sendArrays[counter] = new Protocol.CategoryItemArray();
            sendArrays[counter].Category = VARIABLE.Key;
            sendArrays[counter].ItemIds = VARIABLE.Value.ToArray();

            counter++;
        }

        
       foreach (var VARIABLE in gameManager.UserGameInfos.Values)
       {
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