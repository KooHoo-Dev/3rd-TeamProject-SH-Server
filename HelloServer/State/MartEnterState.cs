using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartEnterState : GameTurnState
{
   
    public MartEnterState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.MartEnter(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,Protocol.QuestType.ItemPickUp.ToString()));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartMoveState>();
        }
    }
}