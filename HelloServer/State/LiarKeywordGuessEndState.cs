using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class LiarKeywordGuessEndState : GameTurnState
{
    LiarKeywordGuessEndStateMessage liarKeywordGuessEndStateMessage = new LiarKeywordGuessEndStateMessage();
    public LiarKeywordGuessEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        liarKeywordGuessEndStateMessage.CurrentRound = gameManager.currentRound;
        liarKeywordGuessEndStateMessage.CurrentCycle = gameManager.currentCycle;
        liarKeywordGuessEndStateMessage.TimerMs = MaxMsTime;
        liarKeywordGuessEndStateMessage.IsRightAnswer = gameManager.CurrentKeyWord.KeywordName  == gameManager.LiarGuessKeyWord;
        liarKeywordGuessEndStateMessage.liarKeyword = gameManager.CurrentLiarKeyword.KeywordName;
        liarKeywordGuessEndStateMessage.nomalKeyword = gameManager.CurrentKeyWord.KeywordName;
        
        liarKeywordGuessEndStateMessage.userScoreInfo = CalculateScoreAndApply();
        
        BroadcastAsync(liarKeywordGuessEndStateMessage);
        
    }

    public override string GetGameStateString()
    {
        return liarKeywordGuessEndStateMessage.Type;
    }
    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);


        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ScoreTallyState>();
        }
    }

    // 시민이 라이어를 맞춘 여부와, 라이어가 키워드를 맞춘 여부에 따라 점수 분배
    private UserScoreInfo[] CalculateScoreAndApply()
    {
        int voteScoreChangeAmount = gameManager.currentRoom.GameConfig.VoteScoreChangeAmount;
        int keywordGuessScoreChangeAmount = gameManager.currentRoom.GameConfig.KeywordGuessScoreChangeAmount;
        UserScoreInfo[] resultInfo = new UserScoreInfo[gameManager.users.Length];
        int counter = 0;
        foreach (var VARIABLE in gameManager.currentRoom.members.Values)
        {
            UserScoreInfo scoreInfo = new UserScoreInfo();
            if (VARIABLE.playerState.IsLiar)
            {
                VARIABLE.score +=
                    (gameManager.CurrentKeyWord.KeywordName == gameManager.LiarGuessKeyWord)
                        ? keywordGuessScoreChangeAmount
                        : 0;
                Console.WriteLine($"[투표 및 키워드 점수 계산 이전] 일반 유저 아이디 : {VARIABLE.User.Id}, 유저 점수 {VARIABLE.score}");
                
                Console.WriteLine($"[라이어 키워드 맞춤 여부] 맟췄는가? :{gameManager.CurrentKeyWord.KeywordName == gameManager.LiarGuessKeyWord}");
                // 라밍아웃 버튼으로 투표가 스킵된 경우, 투표 점수 집계 안함
                if(string.IsNullOrEmpty(gameManager.PressedLiarId) == false)
                {
                    
                    if(VARIABLE.score < 0) VARIABLE.score = 0;
                    scoreInfo.UserId = VARIABLE.User.Id;
                    scoreInfo.UserScore = VARIABLE.score;
                    resultInfo[counter] = scoreInfo;
                    counter++;
                    continue;
                }
                VARIABLE.score +=
                    (gameManager.MostFrequent == VARIABLE.User.Id)
                        ? 0
                        : voteScoreChangeAmount;
            }
            else
            {
            Console.WriteLine($"[투표 및 키워드 점수 계산 이전] 일반 유저 아이디 : {VARIABLE.User.Id}, 유저 점수 {VARIABLE.score}");
                
                VARIABLE.score +=
                    (gameManager.CurrentKeyWord.KeywordName == gameManager.LiarGuessKeyWord)
                        ? 0
                        : (int)(keywordGuessScoreChangeAmount / 2)  == 0 ? 1 : (int)(keywordGuessScoreChangeAmount / 2) ;
                // 라밍아웃 버튼으로 투표가 스킵된 경우, 투표 점수 집계 안함
                if(string.IsNullOrEmpty(gameManager.PressedLiarId) == false)
                {
                    
                    if(VARIABLE.score < 0) VARIABLE.score = 0;
                    scoreInfo.UserId = VARIABLE.User.Id;
                    scoreInfo.UserScore = VARIABLE.score;
                    resultInfo[counter] = scoreInfo;
                    counter++;
                    continue;
                }
                VARIABLE.score +=
                    (gameManager.MostFrequent == VARIABLE.User.Id)
                        ? voteScoreChangeAmount
                        : -(int)(voteScoreChangeAmount / 2)  == 0 ? -1 : -(int)(voteScoreChangeAmount / 2) ;;
            }

            if(VARIABLE.score < 0) VARIABLE.score = 0;
            scoreInfo.UserId = VARIABLE.User.Id;
            scoreInfo.UserScore = VARIABLE.score;
            resultInfo[counter] = scoreInfo;
            counter++;
            Console.WriteLine($"[투표 및 키워드 점수 계산 이후] 유저 아이디 : {scoreInfo.UserId}, 유저 점수 {scoreInfo.UserScore}");
            
        }
        return  resultInfo;
    }
}