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

        Protocol.UserScoreInfo[] scoreInfos = new Protocol.UserScoreInfo[gameManager.users.Length];
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
        Protocol.UserScoreInfo[]  scoreInfos = new Protocol.UserScoreInfo[gameManager.users.Length];
        int counter = 0;
        foreach (var VARIABLE in gameManager.currentRoom.members.Values)
        {
            Protocol.UserScoreInfo scoreInfo = new Protocol.UserScoreInfo();

            bool Sueccess = false;
            for (int i = 0; i < gameManager.currentRoom.GameConfig.MaxCycle; i++)
            {
                if (VARIABLE.playerState.ItemIds[i] == gameManager.QuestInfo[VARIABLE.User.Id])
                {
                    Sueccess = true;
                    break;
                }
            }

            VARIABLE.score += gameManager.currentRoom.GameConfig.QuestScoreChangeAmount;
            scoreInfo.UserId = VARIABLE.User.Id;
            scoreInfo.UserScore = VARIABLE.score;
            scoreInfos[counter] = scoreInfo;
            counter++;
        }
        return scoreInfos;
        
    }
}