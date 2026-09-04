using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class ScoreTallyState : GameTurnState
{

    public ScoreTallyState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        BroadcastAsync(TurnMessageFactory.ScoreTally(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.LiarOutButtonQueue?.ToArray(),CalculateScoreAndApply()));
    }


    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ScoreTallyEndState>();
        }
    }
    // 라이어 자진공개 버튼을 누른 여부에 따라 점수 분배
    private Protocol.UserScoreInfo[] CalculateScoreAndApply()
    {
        int liarButtonScoreChangeAmount = gameManager.currentRoom.GameConfig.LiarButtonScoreChangeAmount;

        Protocol.UserScoreInfo[] resultInfo = new Protocol.UserScoreInfo[gameManager.UserGameInfos.Count];
        List<string> pressedNormalUsers = new List<string>();
        pressedNormalUsers = gameManager.LiarOutButtonQueue?.ToList();
        int counter = 0;
        foreach (var VARIABLE in gameManager.UserGameInfos)
        {
            Console.WriteLine($"[라밍아웃 점수 계산 이전] 유저 아이디 : {VARIABLE.Key}, 유저 점수 {VARIABLE.Value.score}");
            resultInfo[counter].UserId = VARIABLE.Key;
            Protocol.UserScoreInfo scoreInfo = new Protocol.UserScoreInfo();
            scoreInfo.UserId = VARIABLE.Key;
            scoreInfo.UserScore = VARIABLE.Value.score;
            if (VARIABLE.Value.IsLiar)
            {
                if (string.IsNullOrEmpty(gameManager.PressedLiarId) == false
                    && gameManager.LiarGuessKeyWord == gameManager.CurrentLiarKeyword.KeywordName)
                {
                    scoreInfo.UserScore += liarButtonScoreChangeAmount;
                    
                }
                else if (string.IsNullOrEmpty(gameManager.PressedLiarId) == false
                         && gameManager.LiarGuessKeyWord != gameManager.CurrentLiarKeyword.KeywordName)
                {
                    scoreInfo.UserScore += -1;
                }


            }
            else
            {
                if(pressedNormalUsers.Count == 0) continue;
                bool isPressed = false;
                for (int i = 0; i < pressedNormalUsers.Count; i++)
                {
                    if (pressedNormalUsers[i] == VARIABLE.Key)
                    {
                        isPressed = true;
                        break;
                    }
                }

                if (isPressed)
                {
                    scoreInfo.UserScore += -(int)(liarButtonScoreChangeAmount/2) == 0 ? -1 : -(int)(liarButtonScoreChangeAmount/2);
                }
            }

            if(scoreInfo.UserScore < 0) scoreInfo.UserScore = 0;

            resultInfo[counter] = scoreInfo;
            counter++;
            Console.WriteLine($"[라밍아웃 점수 계산 이후] 유저 아이디 : {scoreInfo.UserId}, 유저 점수 {scoreInfo.UserScore}");
            
        }

        int counter2 = 0;
        foreach (var VARIABLE in gameManager.UserGameInfos)
        {
            Protocol.UserScoreInfo scoreInfo = new Protocol.UserScoreInfo();
            scoreInfo.UserId = VARIABLE.Key;
            scoreInfo.UserScore = VARIABLE.Value.score;
                if (VARIABLE.Value.IsQuestSuccess)
                {

                    scoreInfo.UserScore += gameManager.currentRoom.GameConfig.QuestScoreChangeAmount;
                    break;
                }
            Console.WriteLine($"[퀘스트 점수 계산 이후] 유저 아이디 : {scoreInfo.UserId}, 유저 점수 {scoreInfo.UserScore}");
            resultInfo[counter] =  scoreInfo;
        }
        
        return  resultInfo;
    }
}