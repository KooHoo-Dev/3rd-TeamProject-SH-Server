using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class ShowItemAndSpeakState : GameTurnState
{


    private int fristIndex = 0;
    public ShowItemAndSpeakState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        
        if (gameManager.currentSpeakedCount == 0)
        {
            gameManager.ChangeCategory();
            gameManager.currentCycle++;
            gameManager.SkipCount = 0;
            Random rnd = new Random();
            fristIndex = rnd.Next(0, gameManager.UserGameInfos.Count);
            int counter = 0;
            foreach (var VARIABLE in gameManager.UserGameInfos)
            {
                if (counter == fristIndex)
                {
                    gameManager.focausUser = VARIABLE.Value.user;
                    
                }
                counter++;
            }
   
        }
        else
        {
            int counter = 0;
            
            foreach (var VARIABLE in gameManager.UserGameInfos)
            {
                if (counter == (gameManager.currentSpeakedCount + fristIndex) % (gameManager.UserGameInfos.Count))
                {
                    gameManager.focausUser = VARIABLE.Value.user;
                    
                }
                counter++;
            }
        }


            BroadcastAsync(TurnMessageFactory.ShowItemAndSpeak(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.focausUser.Id,gameManager.currentCategory.ToString()));

    }


    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);
        bool trigger = gameManager.ChangeSpeakerTrigger;
        if (string.IsNullOrEmpty(gameManager.PressedLiarId) == false)
        {
            gameManager.currentSpeakedCount = 0;
            
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