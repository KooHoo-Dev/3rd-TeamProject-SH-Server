using System.Collections.Concurrent;
using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public class PointAtSuspectEndState : GameTurnState
{
    private PointAtSuspectEndStateMessage pointAtSuspectEndStateMessage = new PointAtSuspectEndStateMessage();

    
    public PointAtSuspectEndState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }
    public override string GetGameStateString()
    {
        return pointAtSuspectEndStateMessage.Type;
    }

    public override void Enter()
    {
        base.Enter();
        pointAtSuspectEndStateMessage.CurrentCycle = gameManager.currentCycle;
        pointAtSuspectEndStateMessage.CurrentRound = gameManager.currentRound;
        pointAtSuspectEndStateMessage.TimerMs = MaxMsTime;
        ConcurrentDictionary<string, string> pointInfo = gameManager.PointInfo;
        List<string> list = pointInfo.Keys.ToList();
        // 가장 많이 등장한 문자열 찾기(중복 혹은 없으면 "" 반환)
        gameManager.MostFrequent = GetWinner(list);               
        pointAtSuspectEndStateMessage.CurrentOwnerID = gameManager.MostFrequent;
        BroadcastAsync(pointAtSuspectEndStateMessage);

    }

    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            if (string.IsNullOrEmpty(gameManager.MostFrequent) &&
                gameManager.currentCycle != gameManager.currentRoom.GameConfig.MaxCycle)
            {
                gameManager.currentCycle++;
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
    private string GetWinner(List<string> votes)
    {
        // 데이터가 없으면 빈 값 반환
        if (votes == null || votes.Count == 0) return "";

        // 득표수 기준으로 내림차순 정렬하여 상위 2개 그룹만 추출
        var topGroups = votes
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