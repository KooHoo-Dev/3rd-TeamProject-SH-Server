using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class LiarOutButtonPressedState : GameTurnState
{
    LiarOutButtonPressedStateMessage liarOutButtonPressedStateMessage = new LiarOutButtonPressedStateMessage();

    public LiarOutButtonPressedState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        liarOutButtonPressedStateMessage.ID = gameManager.LiarButtonPressedUserId;
        gameManager.LiarButtonPressedUserId = "";
        liarOutButtonPressedStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(liarOutButtonPressedStateMessage);
    }

    public override string GetGameStateString()
    {
        return liarOutButtonPressedStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<LiarKeywordGuessState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
  
    }
}