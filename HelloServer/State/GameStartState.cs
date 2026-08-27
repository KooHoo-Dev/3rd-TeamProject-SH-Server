using System.Timers;
using HelloServer.State;
using Jay.FSM;
namespace HelloServer.State;

public class GameStartState : GameTurnState
{
    private GameStartStateMessage gameStartStateMessage = new GameStartStateMessage();
    public GameStartState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        gameStartStateMessage.CurrentCycle = 1;
        gameStartStateMessage.CurrentRound = 1;
        gameStartStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(gameStartStateMessage);
    }
    public override string GetGameStateString()
    {
        return gameStartStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<GenreAssignAndLiarSelectState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}