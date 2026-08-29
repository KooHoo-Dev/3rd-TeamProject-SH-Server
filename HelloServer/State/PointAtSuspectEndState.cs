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
        List<string> list = pointInfo.Values.ToList();
        // 가장 많이 등장한 문자열 찾기(중복 혹은 없으면 "" 반환)
        gameManager.MostFrequent = GetWinner();  
        Console.WriteLine($"[{gameManager.currentRoom.code}][최종 당선자] : {gameManager.MostFrequent}");
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

        foreach ((string user, int count) in VoteCount)
            {
                if (count > harf)
                {
                    ElectedUser = user;
                    break;
                }
            }
            
        return ElectedUser;
    }
}