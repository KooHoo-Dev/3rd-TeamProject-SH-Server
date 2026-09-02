using System.Collections.Concurrent;
using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class PointAtSuspectState : GameTurnState
{

    public PointAtSuspectState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
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
    }

    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        int harf = (int)(gameManager.UserGameInfos.Count / 2);


        if (currentMsTime > MaxMsTime || gameManager.SkipCount >= harf)
        { 
            stateMachine.ChangeState<PointAtSuspectEndState>();
        }
    }
}