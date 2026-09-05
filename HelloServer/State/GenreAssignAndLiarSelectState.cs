using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class GenreAssignAndLiarSelectState: GameTurnState
{
 
    public GenreAssignAndLiarSelectState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        gameManager.currentRound++;
        gameManager.currentCycle = 0;
        Random rnd = new Random();
        var dm = DataManager.Instance;
      
        Console.WriteLine($"[장르 랜덤 숫자] 랜덤 테스트 : {rnd.Next(0, DataManager.Instance.Genres.Count)}");
        
        GenreDef genreDef = DataManager.Instance.Genres.Get(rnd.Next(DataManager.Instance.Genres.Count));
        gameManager.CurrentGanre = genreDef;
        Console.WriteLine($"[장르 선정 로직] 현재 장르 : {genreDef?.GenreName}");

        gameManager.LiarId = "";
        foreach (var VARIABLE in gameManager.currentRoom.members.Values)
        {
            VARIABLE.playerState.IsLiar = false;
            gameManager.UserGameInfos[VARIABLE.User.Id].IsLiar =false;
        }

        int rendIndex = rnd.Next(0, gameManager.UserGameInfos.Count);
        Console.WriteLine($"[라이어 유저 랜덤 인덱스] 인덱스 : {rendIndex}, 전체 게임 유저 수 {gameManager.UserGameInfos.Count}");
        GameManager.UserInfo Liar = new GameManager.UserInfo();
        int counter = 0;
        foreach (var VARIABLE in gameManager.UserGameInfos)
        {
            if (counter == rendIndex)
            {
                VARIABLE.Value.IsLiar = true;
                
                gameManager.currentRoom.members[VARIABLE.Value.user.Id].playerState.IsLiar = true;
                Liar = gameManager.UserGameInfos[VARIABLE.Value.user.Id];
            }
            
            counter++;
        }

        Console.WriteLine($"[라이어 선정 로직] 라이어 유저 : {Liar?.user.Id}");
  
        gameManager.LiarId = Liar.user.Id;

        
        BroadcastAsync(TurnMessageFactory.GenreAssignAndLiarSelect(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.CurrentGanre.GenreId,gameManager.LiarId ));
    }
 
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<KeywordDistributeState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}