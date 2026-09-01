using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class ScoreTallyEndState : GameTurnState
{

    public ScoreTallyEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        gameManager.LiarGuessKeyWord = "";
        gameManager.PressedLiarId = "";
        gameManager.LiarOutButtonQueue.Clear();

        BroadcastAsync(TurnMessageFactory.ScoreTallyEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime && gameManager.currentRound >= gameManager.currentRoom.GameConfig.MaxRound)
        {
            stateMachine.ChangeState<FinalResultState>();
        }
        else if (currentMsTime > MaxMsTime && gameManager.currentRound < gameManager.currentRoom.GameConfig.MaxRound)
        {
            stateMachine.ChangeState<GenreAssignAndLiarSelectState>();
        }
    }
}