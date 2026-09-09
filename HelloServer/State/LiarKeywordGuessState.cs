using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarKeywordGuessState : GameTurnState
{

    public LiarKeywordGuessState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.LiarKeywordGuess(MaxMsTime, gameManager.currentCycle, gameManager.currentRound));
    }


    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
        if (currentMsTime > MaxMsTime || string.IsNullOrEmpty(gameManager.LiarGuessKeyWord) == false)
        {
            stateMachine.ChangeState<LiarKeywordGuessEndState>();
        }
    }
}