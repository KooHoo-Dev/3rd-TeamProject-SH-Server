using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class FinalResultEndState : GameTurnState
{
 
    public FinalResultEndState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.FinalResultEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }

 
    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
        if (currentMsTime > MaxMsTime)
        {
             gameManager.GameEnd();

        }
    }
}