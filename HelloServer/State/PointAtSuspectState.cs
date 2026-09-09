using System.Collections.Concurrent;
using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class PointAtSuspectState : GameTurnState
{
    private int harf = 0;
    public PointAtSuspectState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        BroadcastAsync(TurnMessageFactory.PointAtSuspect(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));

        foreach ((string key, string value) in gameManager.PointInfo)
        {
            gameManager.PointInfo[key] = "";
        }
        harf = (int)(gameManager.UserGameInfos.Count / 2);
        
    }
    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);


        if (currentMsTime > MaxMsTime || (gameManager.SkipCount > harf))
        { 
            stateMachine.ChangeState<PointAtSuspectEndState>();
        }
    }
}