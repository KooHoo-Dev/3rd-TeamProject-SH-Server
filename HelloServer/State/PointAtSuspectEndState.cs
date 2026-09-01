using System.Collections.Concurrent;
using System.Timers;
using Jay.FSM;
using NetworkManager;

namespace HelloServer.State;

public class PointAtSuspectEndState : GameTurnState
{
    
    public PointAtSuspectEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }


    public override void Enter()
    {
        base.Enter();

        ConcurrentDictionary<string, string> pointInfo = gameManager.PointInfo;
        List<string> list = pointInfo.Values.ToList();
        // 가장 많이 등장한 문자열 찾기(중복 혹은 없으면 "" 반환)
        gameManager.MostFrequent = GetWinner();  
        Console.WriteLine($"[{gameManager.currentRoom.code}][최종 당선자] : {gameManager.MostFrequent}");
        BroadcastAsync(TurnMessageFactory.PointAtSuspectEnd(MaxMsTime,gameManager.currentCycle,gameManager.currentRound,gameManager.MostFrequent));

    }

    protected override void Tick(object sender, ElapsedEventArgs e)
    {
        base.Tick(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            if (string.IsNullOrEmpty(gameManager.MostFrequent) &&
                gameManager.currentCycle != gameManager.currentRoom.GameConfig.MaxCycle)
            {
                
                stateMachine.ChangeState<ShowItemAndSpeakState>();
            }
            else if (string.IsNullOrEmpty(gameManager.MostFrequent) &&
                     gameManager.currentCycle >= gameManager.currentRoom.GameConfig.MaxCycle)
            {
                
                stateMachine.ChangeState<PointAtSuspectState>();
            }
            else if (string.IsNullOrEmpty(gameManager.MostFrequent) == false)
            {
                stateMachine.ChangeState<DebateTimeState>();
            }
        }
    }
    private string GetWinner()
    {

        int harf = (int)(gameManager.users.Length / 2);

        string ElectedUser = "";
        ConcurrentDictionary<string, string> pointInfo = gameManager.PointInfo;
        Dictionary<string, int> VoteCount = new Dictionary<string, int>();

        foreach ((string pointer, string seleted) in pointInfo)
        {
            if (string.IsNullOrEmpty(seleted)) continue;
            if (VoteCount.ContainsKey(seleted) == false)
            {
                VoteCount.Add(seleted, 1);
            }
            else
            {
                VoteCount[seleted]++;
            }
        }

        int TiedCount = 0;
        foreach ((string user, int count) in VoteCount)
            {
                Console.WriteLine($"[지목 결정된 유저 계산 함수] 유저 : {user}, 투표 받은 수 {count}");
                
                if (count > harf)
                {
                    ElectedUser = user;
                    break;
                }
                else if (count == harf)
                {
                    ElectedUser = user;
                    TiedCount++;
                }
                
            }

        // 반반 투표가 나오면 무효
        if (TiedCount > 1)
        {
            ElectedUser = "";
        }
            Console.WriteLine($"[지목 결정된 유저 계산 함수] 최종 리턴 : {ElectedUser}");
        return ElectedUser;
    }
}