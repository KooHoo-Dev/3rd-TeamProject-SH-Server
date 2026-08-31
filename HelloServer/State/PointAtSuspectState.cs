using System.Collections.Concurrent;
using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class PointAtSuspectState : GameTurnState
{
    private PointAtSuspectStateMessage pointAtSuspectStateMessage = new PointAtSuspectStateMessage();    
    public PointAtSuspectState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }
    public override string GetGameStateString()
    {
        return pointAtSuspectStateMessage.Type;
    }

    public override void Enter()
    {
        base.Enter();
        pointAtSuspectStateMessage.CurrentCycle = gameManager.currentCycle;
        pointAtSuspectStateMessage.CurrentRound = gameManager.currentRound;
        pointAtSuspectStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(pointAtSuspectStateMessage);

        foreach ((string key, string value) in gameManager.PointInfo)
        {
            gameManager.PointInfo[key] = "";
        }
    }

    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        int harf = (int)(gameManager.users.Length / 2);


        if (currentMsTime > MaxMsTime || gameManager.SkipCount >= harf)
        { 
            stateMachine.ChangeState<PointAtSuspectEndState>();
        }
    }
}