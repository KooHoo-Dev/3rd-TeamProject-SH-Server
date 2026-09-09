using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class FinalResultState : GameTurnState
{

    public FinalResultState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        List<string> winerIds = new List<string>();
        
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
        winerIds.Add(winerId);
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


    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<FinalResultEndState>();
        }
    }
}