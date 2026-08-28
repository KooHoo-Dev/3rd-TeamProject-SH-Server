using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class FinalResultState : GameTurnState
{
    FinalResultStateMessage finalResultStateMessage = new FinalResultStateMessage();
    public FinalResultState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        finalResultStateMessage.CurrentCycle = gameManager.currentCycle;
        finalResultStateMessage.CurrentRound = gameManager.currentRound;
        finalResultStateMessage.TimerMs = MaxMsTime;
        int maxScore = -999999;
        string winerId = "";
        for (int i = 0; i < gameManager.currentRoom.members.Count; i++)
        {

            if (gameManager.currentRoom.members[gameManager.users[i].Id].playerState.score > maxScore)
            {
                maxScore = gameManager.currentRoom.members[gameManager.users[i].Id].playerState.score;
                winerId = gameManager.users[i].Id;
            }
        }

        finalResultStateMessage.CurrentOwnerID = winerId;
        BroadcastAsync(finalResultStateMessage);
    }

    public override string GetGameStateString()
    {
        return finalResultStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<FinalResultEndState>();
        }
    }
}