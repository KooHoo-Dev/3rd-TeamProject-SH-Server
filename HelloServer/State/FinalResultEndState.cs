using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class FinalResultEndState : GameTurnState
{
 
    public FinalResultEndState(StateMachine<IState> stateMachine, GameManager gameManager, float maxTime) : base(stateMachine, gameManager, maxTime)
    {
        
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.FinalResultEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }

 
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
             gameManager.GameEnd();

        }
    }
}