using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarOutButtonPressedState : GameTurnState
{


    public LiarOutButtonPressedState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        
        Console.WriteLine($"[라밍아웃 초기화] 변수 내용 :{gameManager.PressedLiarId}");

        BroadcastAsync(TurnMessageFactory.LiarOutButtonPressed(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.PressedLiarId));
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