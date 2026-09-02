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
            SendAsync(VARIABLE,TurnMessageFactory.MartReturn(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,IsSuccessQuest()));
            
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
    private bool IsSuccessQuest()
    {
            bool Sueccess = false;
        
        int counter = 0;
        foreach (var VARIABLE in gameManager.UserGameInfos)
        {

            for (int i = 0; i < gameManager.currentRoom.GameConfig.MaxCycle; i++)
            {
                if (VARIABLE.Value.ItemIds[i] == gameManager.QuestInfo[VARIABLE.Key])
                {
                    Sueccess = true;
                    VARIABLE.Value.IsQuestSuccess = true;
                    break;
                }
            }

            VARIABLE.Value.score += gameManager.currentRoom.GameConfig.QuestScoreChangeAmount;
            
            counter++;
        }
        return Sueccess;
        
    }
}