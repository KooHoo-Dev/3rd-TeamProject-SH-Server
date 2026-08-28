using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class ShowItemAndSpeakState : GameTurnState
{
    ShowItemAndSpeakStateMessage showItemAndSpeakStateMessage = new ShowItemAndSpeakStateMessage();
    private int currentSpeakedCount = 0;
    private int maxSpeakedCount = 0;
    public ShowItemAndSpeakState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        maxSpeakedCount = gameManager.users.Length;
        showItemAndSpeakStateMessage.CurrentCycle = gameManager.currentCycle;
        showItemAndSpeakStateMessage.CurrentRound = gameManager.currentRound;
        showItemAndSpeakStateMessage.TimerMs = MaxMsTime;
        if (currentSpeakedCount == 0)
        {
            Random rnd = new Random();
            int rendIndex = rnd.Next(0, gameManager.users.Length);
            gameManager.focausUser = gameManager.users[rendIndex];
        }
        else
        {
            gameManager.focausUser = gameManager.users[currentSpeakedCount % (gameManager.users.Length)];
        }
        showItemAndSpeakStateMessage.CurrentOwnerID = gameManager.focausUser.Id;
        showItemAndSpeakStateMessage.CurrentCategory = gameManager.currentCategory.ToString();
        Console.WriteLine($"[발언 상태] {showItemAndSpeakStateMessage.ToString()}");
        if (currentSpeakedCount >= maxSpeakedCount)
        {
            currentSpeakedCount = 0;
            stateMachine.ChangeState<SpeechEndState>();
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
        if (currentMsTime > MaxMsTime || trigger)
        {
            gameManager.ChangeSpeakerTrigger = false;
            stateMachine.ChangeState<ShowItemAndSpeakState>();
            
        }
    }

    public override void Exit()
    {
        base.Exit();
        currentSpeakedCount++;
    }
}