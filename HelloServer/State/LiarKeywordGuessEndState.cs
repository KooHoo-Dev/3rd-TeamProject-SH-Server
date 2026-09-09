using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class LiarKeywordGuessEndState : GameTurnState
{

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


    public override void Tick(int deltaMs)
    {
        base.Tick(deltaMs);


        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ScoreTallyState>();
        }
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
        foreach (var VARIABLE in gameManager.UserGameInfos)
        {
            Protocol.UserScoreInfo scoreInfo = new Protocol.UserScoreInfo();
            scoreInfo.UserId = VARIABLE.Key;
            scoreInfo.UserScore = VARIABLE.Value.score;
            if (VARIABLE.Value.IsLiar)
            {
                scoreInfo.UserScore +=
                    (gameManager.CurrentKeyWord.KeywordName == gameManager.LiarGuessKeyWord)
                        ? keywordGuessScoreChangeAmount
                        : 0;
                Console.WriteLine($"[투표 및 키워드 점수 계산 이전] 일반 유저 아이디 : {VARIABLE.Key}, 유저 점수 {VARIABLE.Value.score}");
                
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
                Console.WriteLine($"[투표 및 키워드 점수 계산 이전] 일반 유저 아이디 : {VARIABLE.Key}, 유저 점수 {VARIABLE.Value.score}");
                
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
                if (voteDic.ContainsKey(VARIABLE.Key) == false)
                {
                    num = Protocol.SelectNum.DontKnow;
                }
                else
                {
                    Protocol.SelectNum.TryParse(voteDic[VARIABLE.Key].selectNum, out num);
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
                        _=> 111111
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
                        _=> 2222222
                    };
                }
                scoreInfo.UserScore += scoreAmount;
                Console.WriteLine($"[ 키워드쪽 점수 계산 중] 유저: {VARIABLE.Key}, 선택한 종류: {num}, 적용된 점수 : {scoreAmount}");
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