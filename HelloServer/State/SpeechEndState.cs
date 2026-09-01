using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class SpeechEndState : GameTurnState
{

    public SpeechEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
     

        BroadcastAsync(TurnMessageFactory.SpeechEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ShowItemAndSpeakState>();
        }
    }

    public override void Exit()
    {
        base.Exit();
            gameManager.currentSpeakedCount++;
    }
}