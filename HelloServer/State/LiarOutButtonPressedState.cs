using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarOutButtonPressedState : BaseGameTurnState
{

    protected override Type NextState => typeof(LiarKeywordGuessState);
    public LiarOutButtonPressedState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        

        BroadcastAsync(TurnMessageFactory.LiarOutButtonPressed(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.PressedLiarId));
    }

}