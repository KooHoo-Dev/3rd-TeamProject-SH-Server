using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarKeywordGuessState : GameTurnState
{

    public LiarKeywordGuessState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.LiarKeywordGuess(MaxMsTime, gameManager.currentCycle, gameManager.currentRound));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime || string.IsNullOrEmpty(gameManager.LiarGuessKeyWord) == false)
        {
            stateMachine.ChangeState<LiarKeywordGuessEndState>();
        }
    }
}