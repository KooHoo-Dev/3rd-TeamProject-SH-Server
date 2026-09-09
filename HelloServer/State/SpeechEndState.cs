using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class SpeechEndState : BaseGameTurnState
{

    public SpeechEndState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
     
        gameManager.currentSpeakedCount++;

        BroadcastAsync(TurnMessageFactory.SpeechEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound));
        
    }


    public override void Tick()
    {
        base.Tick();
        if (currentMsTime > MaxMsTime)
        {
            if (gameManager.currentSpeakedCount >= gameManager.maxSpeakedCount)
            {
                gameManager.currentSpeakedCount = 0;
                stateMachine.ChangeState<PointAtSuspectState>();
            }
            else
            {
                stateMachine.ChangeState<ShowItemAndSpeakState>();
                
            }
        }
    }


}