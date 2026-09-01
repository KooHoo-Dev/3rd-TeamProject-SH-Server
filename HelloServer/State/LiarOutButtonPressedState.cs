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

        liarOutButtonPressedStateMessage.ID = gameManager.PressedLiarId;
        
        Console.WriteLine($"[라밍아웃 초기화] 변수 내용 :{gameManager.PressedLiarId}");
        liarOutButtonPressedStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(liarOutButtonPressedStateMessage);
    }

    public override string GetGameStateString()
    {
        return liarOutButtonPressedStateMessage.Type;
    }
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
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