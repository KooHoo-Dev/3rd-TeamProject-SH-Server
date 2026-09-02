using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class MartReturnState : GameTurnState
{

    public MartReturnState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Protocol.UserScoreInfo[] scoreInfos = new Protocol.UserScoreInfo[gameManager.UserGameInfos.Count];
        foreach (var VARIABLE in gameManager.currentRoom.members.Values)
        {
            SendAsync(VARIABLE,TurnMessageFactory.MartReturn(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,IsSuccessQuest(VARIABLE.User.Id)));
            
        }
    }
    
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ShowItemAndSpeakState>();
        }
    }

    // 아직 미 구현
    private bool IsSuccessQuest(string UserId)
    {
            bool Sueccess = false;

            for (int i = 0; i < gameManager.currentRoom.GameConfig.MaxCycle; i++)
            {
                if (gameManager.UserGameInfos[UserId].ItemIds[i] == gameManager.QuestInfo[UserId])
                {
                    Sueccess = true;
                    gameManager.UserGameInfos[UserId].IsQuestSuccess = true;
                    break;
                }
            }
        
        return Sueccess;
        
    }
}