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

            List<GameManager.UserInfo> userList = gameManager.UserGameInfos.Values.ToList().OrderBy(info => info.user.Id).ToList();
            firstIndex = rnd.Next(userList.Count);
            for (int i = 0; i < userList.Count; i++)
            {
                if (i == firstIndex)
                {
                    gameManager.focausUser = userList[i].user;
                    
                }

            }
   
        }
        else
        {

            List<GameManager.UserInfo> userList = gameManager.UserGameInfos.Values.ToList().OrderBy(info => info.user.Id).ToList();
            for(int i = 0; i < userList.Count; i++)
            {
                if (i == (gameManager.currentSpeakedCount + firstIndex) % (gameManager.UserGameInfos.Count))
                {
                    gameManager.focausUser = userList[i].user;
                    
                }

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