using System.Timers;
using HelloServer.State;
using Jay.FSM;
namespace HelloServer.State;

public class GameStartState : GameTurnState
{
    public GameStartState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }
    public override string GetGameStateString()
    {
        return gameManager.allStateString.stateGameStart;
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