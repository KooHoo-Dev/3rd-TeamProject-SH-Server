using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class FinalResultState : GameTurnState
{

    public FinalResultState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        
        int maxScore = -999999;
        string winerId = "";
        for (int i = 0; i < gameManager.currentRoom.members.Count; i++)
        {

            if (gameManager.currentRoom.members[gameManager.users[i].Id].score > maxScore)
            {
                maxScore = gameManager.currentRoom.members[gameManager.users[i].Id].score;
                winerId = gameManager.users[i].Id;
            }
        }

        List<string> winerIds = new List<string>();
        
        for (int i = 0; i < gameManager.currentRoom.members.Count; i++)
        {

            if (gameManager.users[i].Id != winerId && gameManager.currentRoom.members[gameManager.users[i].Id].score == maxScore )
            {
                winerIds.Add(gameManager.users[i].Id);

            }
        }

        
        BroadcastAsync(TurnMessageFactory.FinalResult(MaxMsTime, gameManager.currentCycle, gameManager.currentRound,
            winerIds.ToArray()));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<FinalResultEndState>();
        }
    }
}