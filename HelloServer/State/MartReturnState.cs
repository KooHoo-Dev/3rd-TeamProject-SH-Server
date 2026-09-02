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

    
        foreach (var VARIABLE in gameManager.currentRoom.members.Values)
        {
            bool isSuccess = IsSuccessQuest(VARIABLE.User.Id);
            Console.WriteLine($"[마트 엔터 메시지 보내기] {VARIABLE.User.Id}의 차례(성공 여부) :  {isSuccess}");
            SendAsync(VARIABLE,TurnMessageFactory.MartReturn(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,isSuccess));
            
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
            Console.WriteLine($"[퀘스트 판별 함수] 퀘스트Info의 개수 :  {gameManager.QuestInfo?.Count}, 유저 Id :{UserId}");

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