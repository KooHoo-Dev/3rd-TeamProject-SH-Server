using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class GenreAssignAndLiarSelectState: GameTurnState
{
    public GenreAssignAndLiarSelectState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }
    public override string GetGameStateString()
    {
        return gameManager.allStateString.stateGenreAssignAndLiarSelect;
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