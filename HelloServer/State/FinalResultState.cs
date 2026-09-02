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
        foreach (var VARIABLE in gameManager.UserGameInfos)
        {
            
            if (VARIABLE.Value.score > maxScore)
            {
                maxScore = VARIABLE.Value.score;
                winerId = VARIABLE.Key;
            }
        }

        List<string> winerIds = new List<string>();
        foreach (var VARIABLE in gameManager.UserGameInfos)
        {
            if (VARIABLE.Key != winerId && VARIABLE.Value.score == maxScore )
            {
                winerIds.Add(VARIABLE.Key);

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