using System.Timers;
using HelloServer.State;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class GameStartState : GameTurnState
{

    public GameStartState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        gameManager.currentRound = 0;
        gameManager.currentCycle = 0;
        
        BroadcastAsync(TurnMessageFactory.GameStart(MaxMsTime,0,0));
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