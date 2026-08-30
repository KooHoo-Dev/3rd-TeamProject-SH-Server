using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class ShowItemAndSpeakState : GameTurnState
{
    ShowItemAndSpeakStateMessage showItemAndSpeakStateMessage = new ShowItemAndSpeakStateMessage();

    private int fristIndex = 0;
    public ShowItemAndSpeakState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if(gameManager.currentSpeakedCount == 0)
            gameManager.currentCycle++;

        
        showItemAndSpeakStateMessage.CurrentCycle = gameManager.currentCycle;
        showItemAndSpeakStateMessage.CurrentRound = gameManager.currentRound;
        showItemAndSpeakStateMessage.TimerMs = MaxMsTime;
        
        
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
        showItemAndSpeakStateMessage.CurrentOwnerID = gameManager.focausUser.Id;
        showItemAndSpeakStateMessage.CurrentCategory = gameManager.currentCategory.ToString();
        Console.WriteLine($"[발언 상태] {showItemAndSpeakStateMessage.ToString()}");
        if (gameManager.currentSpeakedCount >= gameManager.maxSpeakedCount)
        {
            gameManager.currentSpeakedCount = 0;
            stateMachine.ChangeState<PointAtSuspectState>();
        }
        else
        {
            BroadcastAsync(showItemAndSpeakStateMessage);
        }
    }

    public override string GetGameStateString()
    {
        return showItemAndSpeakStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        bool trigger = gameManager.ChangeSpeakerTrigger;
        if (string.IsNullOrEmpty(gameManager.LiarButtonPressedUserId) == false)
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