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
        BroadcastAsync(TurnMessageFactory.MartReturn(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,CalculateQuest()));
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
    private Protocol.UserScoreInfo[] CalculateQuest()
    {
        Protocol.UserScoreInfo[]  scoreInfos = new Protocol.UserScoreInfo[gameManager.UserGameInfos.Count];
        int counter = 0;
        foreach (var VARIABLE in gameManager.UserGameInfos)
        {
            Protocol.UserScoreInfo scoreInfo = new Protocol.UserScoreInfo();

            bool Sueccess = false;
            for (int i = 0; i < gameManager.currentRoom.GameConfig.MaxCycle; i++)
            {
                if (VARIABLE.Value.ItemIds[i] == gameManager.QuestInfo[VARIABLE.Key])
                {
                    Sueccess = true;
                    break;
                }
            }

            VARIABLE.Value.score += gameManager.currentRoom.GameConfig.QuestScoreChangeAmount;

            scoreInfos[counter] = scoreInfo;
            counter++;
        }
        return scoreInfos;
        
    }
}