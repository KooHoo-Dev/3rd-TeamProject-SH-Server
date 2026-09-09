using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class SpeechEndState : GameTurnState
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


    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
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

    public override void Exit()
    {
        base.Exit();
    }
}