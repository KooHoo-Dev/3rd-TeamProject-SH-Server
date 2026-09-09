using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class ScoreTallyEndState : BaseGameTurnState
{

    public ScoreTallyEndState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 모든 점수가 게산되었으니 관련 변수 초기화
        gameManager.LiarGuessKeyWord = "";
        gameManager.PressedLiarId = "";
        gameManager.LiarOutButtonQueue.Clear();
        foreach (var info in gameManager.UserGameInfos.Values)
            info.IsQuestSuccess = false;
        
        BroadcastAsync(TurnMessageFactory.ScoreTallyEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }


    public override void Tick()
    {
        base.Tick();
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