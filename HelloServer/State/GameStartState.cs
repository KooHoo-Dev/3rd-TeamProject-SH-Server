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
        gameManager.currentRound = 0;
        gameManager.currentCycle = 0;
        gameStartStateMessage.CurrentCycle = 0;
        gameStartStateMessage.CurrentRound = 0;
        gameStartStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(gameStartStateMessage);
    }
    public override string GetGameStateString()
    {
        return gameStartStateMessage.Type;
    }
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
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