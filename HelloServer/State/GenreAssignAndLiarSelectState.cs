using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class GenreAssignAndLiarSelectState: GameTurnState
{
    private GenreAssignAndLiarSelectStateMessage genreAssignAndLiarSelectStateMessage = new GenreAssignAndLiarSelectStateMessage();
    public GenreAssignAndLiarSelectState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        
        Random rnd = new Random();
        var dm = DataManager.Instance;
        Console.WriteLine($"[진단] DataManager 인스턴스 해시: {dm.GetHashCode()}, IsReady: {dm.IsReady}, Genres.Count: {dm.Genres.Count}");
        Console.WriteLine($"[장르 랜덤 숫자] 랜덤 테스트 : {rnd.Next(0, DataManager.Instance.Genres.Count)}");
        
        GenreDef genreDef = DataManager.Instance.Genres.Get(rnd.Next(DataManager.Instance.Genres.Count));
        gameManager.CurrentGanre = genreDef;
        Console.WriteLine($"[장르 선정 로직] 현재 장르 : {genreDef?.GenreName}");
        
        
        User Liar = gameManager.users[rnd.Next(0, gameManager.users.Length)];
        Console.WriteLine($"[라이어 선정 로직] 라이어 유저 : {Liar?.Id}");
        gameManager.currentRoom.members[Liar.Id].playerState.IsLiar = true;
        genreAssignAndLiarSelectStateMessage.CurrentOwnerID = Liar.Id;
        
        genreAssignAndLiarSelectStateMessage.TimerMs = MaxMsTime;
        genreAssignAndLiarSelectStateMessage.CurrentCycle = gameManager.currentCycle;
        genreAssignAndLiarSelectStateMessage.CurrentRound = gameManager.currentRound;
        genreAssignAndLiarSelectStateMessage.GenreId = gameManager.CurrentGanre.GenreId;
        
        BroadcastAsync(genreAssignAndLiarSelectStateMessage);
    }
    public override string GetGameStateString()
    {
        return genreAssignAndLiarSelectStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
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