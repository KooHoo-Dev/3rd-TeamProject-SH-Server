using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class VoteEndState : GameTurnState
{
    VoteEndStateMessage voteEndStateMessage = new VoteEndStateMessage();

    private string result = "";
    public VoteEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        voteEndStateMessage.CurrentCycle = gameManager.currentCycle;
        voteEndStateMessage.CurrentRound = gameManager.currentRound;
        voteEndStateMessage.TimerMs = MaxMsTime;
        BroadcastAsync(voteEndStateMessage);
        int count = gameManager.VoteQueue.Count;

        List<string> resultList = new List<string>();
        for (int i = 0; i < count; i++)
        {
            gameManager.VoteQueue.TryDequeue(out VoteMessage v);
            if (v == null)
            {
                Console.WriteLine($"[투표 저장 정보 꺼내기 실패]");
                return;
            }
            else
            {
                resultList.Add(v.selectNum);
            }

        }

        for (int i = gameManager.users.Length - count - 1; i >= 0; i--)
        {
            resultList.Add(HelloServer.SelectNum.DontKnow.ToString());
        }

        for (int i = 0; i < resultList.Count; i++)
        {
            Console.WriteLine($"[최종 투표 리스트] {i}번째 : {resultList[i]}");
        }
        result = GetWinner(resultList);
        
    }

    public override string GetGameStateString()
    {
        return voteEndStateMessage.Type;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {

            if (string.IsNullOrEmpty(result) &&
                voteEndStateMessage.CurrentCycle >= gameManager.currentRoom.GameConfig.MaxCycle)
            {
                stateMachine.ChangeState<PointAtSuspectState>();
            }
            else if(string.IsNullOrEmpty(result) &&
                    voteEndStateMessage.CurrentCycle < gameManager.currentRoom.GameConfig.MaxCycle)
            {
                stateMachine.ChangeState<ShowItemAndSpeakState>();
            }
            else if(string.IsNullOrEmpty(result) == false)
            {
                stateMachine.ChangeState<LiarConfirmedState>();
            }
        }
    }
    private string GetWinner(List<string> list)
    {
        // 데이터가 없으면 빈 값 반환
        if (list == null || list.Count == 0) return "";

        // 득표수 기준으로 내림차순 정렬하여 상위 2개 그룹만 추출
        var topGroups = list
            .GroupBy(id => id)
            .Select(g => new { ID = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(2)
            .ToList();

        // 투표자가 1명뿐이거나, 1위와 2위의 득표수가 다르면 1위 반환
        if (topGroups.Count == 1 || topGroups[0].Count != topGroups[1].Count)
        {
            return topGroups[0].ID;
        }

        // 동률인 경우 빈 값 반환
        return "";
    }
}