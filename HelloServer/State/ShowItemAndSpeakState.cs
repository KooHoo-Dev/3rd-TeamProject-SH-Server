using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class ShowItemAndSpeakState : GameTurnState
{


    private int fristIndex = 0;
    public ShowItemAndSpeakState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if(gameManager.currentSpeakedCount == 0)
            gameManager.currentCycle++;
        
        if (gameManager.currentSpeakedCount == 0)
        {
            Random rnd = new Random();
            fristIndex = rnd.Next(0, gameManager.users.Length);
            gameManager.focausUser = gameManager.users[fristIndex];
   
        }
        else
        {

            gameManager.focausUser = gameManager.users[ (gameManager.currentSpeakedCount + fristIndex) % (gameManager.users.Length)];
        }

        if (gameManager.currentSpeakedCount >= gameManager.maxSpeakedCount)
        {
            gameManager.currentSpeakedCount = 0;
            stateMachine.ChangeState<PointAtSuspectState>();
        }
        else
        {
            BroadcastAsync(TurnMessageFactory.ShowItemAndSpeak(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.focausUser.Id,gameManager.currentCategory.ToString()));
        }
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        bool trigger = gameManager.ChangeSpeakerTrigger;
        if (string.IsNullOrEmpty(gameManager.PressedLiarId) == false)
        {
            stateMachine.ChangeState<LiarOutButtonPressedState>();
        }
        else if (currentMsTime > MaxMsTime || trigger)
        {
            gameManager.ChangeSpeakerTrigger = false;
            stateMachine.ChangeState<SpeechEndState>();
            
        }
    }

    public override void Exit()
    {
        base.Exit();
        
    }
}