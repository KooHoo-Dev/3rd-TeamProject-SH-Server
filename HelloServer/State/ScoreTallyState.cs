using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class ScoreTallyState : GameTurnState
{
    ScoreTallyStateMessage scoreTallyStateMessage = new ScoreTallyStateMessage();  
    public ScoreTallyState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        scoreTallyStateMessage.CurrentCycle = gameManager.currentCycle;
        scoreTallyStateMessage.CurrentRound = gameManager.currentRound;
        scoreTallyStateMessage.TimerMs = MaxMsTime;
        scoreTallyStateMessage.LiarOutButtonInfo = gameManager.LiarOutButtonQueue.ToArray();
        
        scoreTallyStateMessage.userScoreInfo = CalculateScoreAndApply();
        BroadcastAsync(scoreTallyStateMessage);
    }

    public override string GetGameStateString()
    {
        return  scoreTallyStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<ScoreTallyEndState>();
        }
    }
    // 라이어 자진공개 버튼을 누른 여부에 따라 점수 분배
    private UserScoreInfo[] CalculateScoreAndApply()
    {
        int liarButtonScoreChangeAmount = gameManager.currentRoom.GameConfig.LiarButtonScoreChangeAmount;

        UserScoreInfo[] resultInfo = new UserScoreInfo[gameManager.users.Length];
        string[] pressedNormalUsers = gameManager.LiarOutButtonQueue.ToArray();
        int counter = 0;
        foreach (var VARIABLE in gameManager.currentRoom.members.Values)
        {
            Console.WriteLine($"[라밍아웃 점수 계산 이전] 유저 아이디 : {VARIABLE.User.Id}, 유저 점수 {VARIABLE.score}");
            
            UserScoreInfo scoreInfo = new UserScoreInfo();
            if (VARIABLE.playerState.IsLiar)
            {
                if (string.IsNullOrEmpty(gameManager.PressedLiarId) == false
                    && gameManager.LiarGuessKeyWord == gameManager.CurrentLiarKeyword.KeywordName)
                {
                    VARIABLE.score += liarButtonScoreChangeAmount;
                    
                }
                else if (string.IsNullOrEmpty(gameManager.PressedLiarId) == false
                         && gameManager.LiarGuessKeyWord != gameManager.CurrentLiarKeyword.KeywordName)
                {
                    VARIABLE.score += -1;
                }


            }
            else
            {
                bool isPressed = false;
                for (int i = 0; i < pressedNormalUsers.Length; i++)
                {
                    if (pressedNormalUsers[i] == VARIABLE.User.Id)
                    {
                        isPressed = true;
                        break;
                    }
                }

                if (isPressed)
                {
                    VARIABLE.score += -(int)(liarButtonScoreChangeAmount/2) == 0 ? -1 : -(int)(liarButtonScoreChangeAmount/2);
                }
            }

            if(VARIABLE.score < 0) VARIABLE.score = 0;
            scoreInfo.UserId = VARIABLE.User.Id;
            scoreInfo.UserScore = VARIABLE.score;
            resultInfo[counter] = scoreInfo;
            counter++;
            Console.WriteLine($"[라밍아웃 점수 계산 이후] 유저 아이디 : {scoreInfo.UserId}, 유저 점수 {scoreInfo.UserScore}");
        }
        return  resultInfo;
    }
}