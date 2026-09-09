using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarConfirmedState : GameTurnState
{
    public LiarConfirmedState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        
        BroadcastAsync(TurnMessageFactory.LiarConfirmed(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.LiarId));
    }

    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<LiarKeywordGuessState>();
        }
    }
}