using System.Collections.Concurrent;
using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class PointAtSuspectState : BaseGameTurnState
{
    private int harf = 0;
    protected override Type NextState => typeof(PointAtSuspectEndState);
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
    public override void Tick()
    {
        base.Tick();

        if ((gameManager.SkipCount > harf))
        { 
            stateMachine.ChangeState<PointAtSuspectEndState>();
        }
    }
}