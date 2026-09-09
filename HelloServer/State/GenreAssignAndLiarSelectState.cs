using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class GenreAssignAndLiarSelectState: BaseGameTurnState
{
    protected override Type NextState => typeof(KeywordDistributeState);
    public GenreAssignAndLiarSelectState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        gameManager.currentRound++;
        gameManager.currentCycle = 0;
        Random rnd = new Random();
        var dm = DataManager.Instance;
      

        GenreDef genreDef = DataManager.Instance.Genres.Get(rnd.Next(DataManager.Instance.Genres.Count));
        gameManager.CurrentGanre = genreDef;
        Console.WriteLine($"[장르 선정 로직] 현재 장르 : {genreDef?.GenreName}");

        gameManager.LiarId = "";
        foreach (var member in gameManager.currentRoom.members.Values)
        {
            member.playerState.IsLiar = false;
            gameManager.UserGameInfos[member.User.Id].IsLiar =false;
        }

        int rendIndex = rnd.Next(0, gameManager.UserGameInfos.Count);

        GameManager.UserInfo Liar = new GameManager.UserInfo();
        int counter = 0;
        foreach (var userGameInfoDic in gameManager.UserGameInfos)
        {
            if (counter == rendIndex)
            {
                userGameInfoDic.Value.IsLiar = true;
                
                gameManager.currentRoom.members[userGameInfoDic.Value.user.Id].playerState.IsLiar = true;
                Liar = gameManager.UserGameInfos[userGameInfoDic.Value.user.Id];
            }
            
            counter++;
        }

        Console.WriteLine($"[라이어 선정 로직] 라이어 유저 : {Liar?.user.Id}");
  
        gameManager.LiarId = Liar.user.Id;

        
        BroadcastAsync(TurnMessageFactory.GenreAssignAndLiarSelect(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.CurrentGanre.GenreId,gameManager.LiarId ));
    }
}