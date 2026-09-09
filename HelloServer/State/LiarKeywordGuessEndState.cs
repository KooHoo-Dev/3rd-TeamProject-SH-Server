using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarKeywordGuessEndState : BaseGameTurnState
{
    protected override Type NextState => typeof(ScoreTallyState);
    public LiarKeywordGuessEndState(StateMachine<IUpdatableState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();

        var msg = TurnMessageFactory.LiarKeywordGuessEnd(MaxMsTime, gameManager.currentCycle, gameManager.currentRound
            , gameManager.CurrentLiarKeyword.KeywordName, gameManager.CurrentKeyWord.KeywordName,
            gameManager.CurrentKeyWord.KeywordName == gameManager.LiarGuessKeyWord,
            CalculateScoreAndApply());
        BroadcastAsync(msg);
        
    }

    // 시민이 라이어를 맞춘 여부와, 라이어가 키워드를 맞춘 여부에 따라 점수 분배
    private Protocol.UserScoreInfo[] CalculateScoreAndApply()
    {
        int voteScoreChangeAmount = gameManager.currentRoom.GameConfig.VoteScoreChangeAmount;
        int keywordGuessScoreChangeAmount = gameManager.currentRoom.GameConfig.KeywordGuessScoreChangeAmount;
        Protocol.UserScoreInfo[] resultInfo = new Protocol.UserScoreInfo[gameManager.UserGameInfos.Count];


        int MaxVoteCount = gameManager.Votes.Count;
        Dictionary<string, Protocol.VoteMessage> voteDic = new Dictionary<string, Protocol.VoteMessage>();
        foreach (var voteMessage in gameManager.Votes.Values)
        {
            gameManager.Votes.TryGetValue(voteMessage.UserID, out Protocol.VoteMessage vote);
            voteDic.Add(vote.UserID,vote);
        }
        
        int counter = 0;
        foreach (var userGameInfoDic in gameManager.UserGameInfos)
        {
            Protocol.UserScoreInfo scoreInfo = new Protocol.UserScoreInfo();
            scoreInfo.UserId = userGameInfoDic.Key;
            scoreInfo.UserScore = userGameInfoDic.Value.score;
            if (userGameInfoDic.Value.IsLiar)
            {
                scoreInfo.UserScore +=
                    (gameManager.CurrentKeyWord.KeywordName == gameManager.LiarGuessKeyWord)
                        ? keywordGuessScoreChangeAmount
                        : 0;
                Console.WriteLine($"[투표 및 키워드 점수 계산 이전] 일반 유저 아이디 : {userGameInfoDic.Key}, 유저 점수 {userGameInfoDic.Value.score}");
                
                Console.WriteLine($"[라이어 키워드 맞춤 여부] 맟췄는가? :{gameManager.CurrentKeyWord.KeywordName == gameManager.LiarGuessKeyWord}");
                // 라밍아웃 버튼으로 투표가 스킵된 경우, 투표 점수 집계 안함
                if(string.IsNullOrEmpty(gameManager.PressedLiarId) == false)
                {
                    
                    if(scoreInfo.UserScore < 0) scoreInfo.UserScore = 0;
            Console.WriteLine($"[투표 및 키워드 점수 계산 이후] 유저 아이디 : {scoreInfo.UserId}, 유저 점수 {scoreInfo.UserScore}");
             
                    resultInfo[counter] = scoreInfo;
                    counter++;
                    continue;
                }
                scoreInfo.UserScore +=
                    (gameManager.MostFrequent == gameManager.LiarId)
                        ? 0
                        : voteScoreChangeAmount;
                
            }
            else
            {
                Console.WriteLine($"[투표 및 키워드 점수 계산 이전] 일반 유저 아이디 : {userGameInfoDic.Key}, 유저 점수 {userGameInfoDic.Value.score}");
                
                scoreInfo.UserScore +=
                    (gameManager.CurrentKeyWord.KeywordName == gameManager.LiarGuessKeyWord)
                        ? 0
                        : (int)(keywordGuessScoreChangeAmount / 2)  == 0 ? 1 : (int)(keywordGuessScoreChangeAmount / 2) ;
                // 라밍아웃 버튼으로 투표가 스킵된 경우, 투표 점수 집계 안함
                if(string.IsNullOrEmpty(gameManager.PressedLiarId) == false)
                {
                    
                    if(scoreInfo.UserScore < 0) scoreInfo.UserScore = 0;
            Console.WriteLine($"[투표 및 키워드 점수 계산 이후] 유저 아이디 : {scoreInfo.UserId}, 유저 점수 {scoreInfo.UserScore}");
                    
                    resultInfo[counter] = scoreInfo;
                    counter++;
                    continue;
                }

                int scoreAmount = 0;
                Protocol.SelectNum num;
                if (voteDic.TryGetValue(userGameInfoDic.Key, out Protocol.VoteMessage vote) == false ||
                    Enum.TryParse(vote.selectNum, out num) == false)
                {
                    // 안 냈거나 규약에 없는 값이면 '모르겠다'로 본다.
                    num = Protocol.SelectNum.DontKnow;
                }
                
                if (gameManager.MostFrequent == gameManager.LiarId)
                {
                    scoreAmount = (num) switch
                    {
                        Protocol.SelectNum.Liar => voteScoreChangeAmount,
                        Protocol.SelectNum.DontKnow => 0,
                        Protocol.SelectNum.NotLiar => -(int)(voteScoreChangeAmount / 2) == 0
                            ? -1
                            : -(int)(voteScoreChangeAmount / 2),
                        _=> 0
                    };
                }
                else
                {
                    scoreAmount = (num) switch
                    {
                        Protocol.SelectNum.Liar => -(int)(voteScoreChangeAmount / 2) == 0
                            ? -1
                            : -(int)(voteScoreChangeAmount / 2),
                        Protocol.SelectNum.DontKnow => 0,
                        Protocol.SelectNum.NotLiar => voteScoreChangeAmount,
                        _=> 0
                    };
                }

                if (num != Protocol.SelectNum.DontKnow && scoreAmount == 0)
                {
                    Console.WriteLine($"[투표 범위가 아닌 투표 값 에러] num의 값 : {num}");
                }
                scoreInfo.UserScore += scoreAmount;
                Console.WriteLine($"[ 키워드쪽 점수 계산 중] 유저: {userGameInfoDic.Key}, 선택한 종류: {num}, 적용된 점수 : {scoreAmount}");
            }


            if(scoreInfo.UserScore < 0) scoreInfo.UserScore = 0;

            resultInfo[counter] = scoreInfo;
            
            counter++;
            Console.WriteLine($"[투표 및 키워드 점수 계산 이후] 유저 아이디 : {scoreInfo.UserId}, 유저 점수 {scoreInfo.UserScore}");
        }

        for (int i = 0; i < resultInfo.Length; i++)
        {
            gameManager.UserGameInfos[resultInfo[i].UserId].score = resultInfo[i].UserScore;
        }

        return  resultInfo;
    }
}