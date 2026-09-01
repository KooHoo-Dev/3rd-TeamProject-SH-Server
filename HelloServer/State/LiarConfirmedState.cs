using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarConfirmedState : GameTurnState
{
    public LiarConfirmedState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        
        BroadcastAsync(TurnMessageFactory.LiarConfirmed(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.LiarId));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<LiarKeywordGuessState>();
        }
    }
}