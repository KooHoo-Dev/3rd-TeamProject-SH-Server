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
        GenreDef genreDef = DataManager.Instance.Genres.Get(rnd.Next(DataManager.Instance.Genres.Count));
        gameManager.CurrentGanre = genreDef;
        
        
        User Liar = gameManager.users[rnd.Next(0, gameManager.users.Length)];
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