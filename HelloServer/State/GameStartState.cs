using System.Timers;
using HelloServer.State;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class GameStartState : BaseGameTurnState
{
    protected override Type NextState => typeof(GenreAssignAndLiarSelectState);
    public GameStartState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        gameManager.currentRound = 0;
        gameManager.currentCycle = 0;
        
        BroadcastAsync(TurnMessageFactory.GameStart(MaxMsTime,0,0));
    }
}