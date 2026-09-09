using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class FinalResultEndState : BaseGameTurnState
{
 
    public FinalResultEndState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.FinalResultEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }

 
    public override void Tick()
    {
        base.Tick();
        if (currentMsTime > MaxMsTime)
        {
             gameManager.GameEnd();

        }
    }
}