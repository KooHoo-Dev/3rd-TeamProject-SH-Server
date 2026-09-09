using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarKeywordGuessState : BaseGameTurnState
{
    protected override Type NextState => typeof(LiarKeywordGuessEndState);
    public LiarKeywordGuessState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.LiarKeywordGuess(MaxMsTime, gameManager.currentCycle, gameManager.currentRound));
    }


    public override void Tick()
    {
        base.Tick();
        if (string.IsNullOrEmpty(gameManager.LiarGuessKeyWord) == false)
        {
            stateMachine.ChangeState<LiarKeywordGuessEndState>();
        }
    }
}