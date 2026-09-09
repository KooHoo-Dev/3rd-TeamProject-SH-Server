using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarConfirmedState : BaseGameTurnState
{
    
    protected override Type NextState => typeof(LiarKeywordGuessState);
    public LiarConfirmedState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        
        BroadcastAsync(TurnMessageFactory.LiarConfirmed(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.LiarId));
    }
}