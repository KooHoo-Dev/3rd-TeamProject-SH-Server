using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class ShowItemAndSpeakState : BaseGameTurnState
{


    private int firstIndex = 0;
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
            gameManager.SkipUserDicClear();
            Random rnd = new Random();
            firstIndex = rnd.Next(0, gameManager.UserGameInfos.Count);
            int counter = 0;
            foreach (var VARIABLE in gameManager.UserGameInfos)
            {
                if (counter == firstIndex)
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
                if (counter == (gameManager.currentSpeakedCount + firstIndex) % (gameManager.UserGameInfos.Count))
                {
                    gameManager.focausUser = VARIABLE.Value.user;
                    
                }
                counter++;
            }
        }


            BroadcastAsync(TurnMessageFactory.ShowItemAndSpeak(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.focausUser.Id,gameManager.currentCategory.ToString()));

    }


    public override void Tick()
    {
        base.Tick();
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
    
 
}