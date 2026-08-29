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

        // 자기 자신 제외 ( -1 더해줌)
        for (int i = gameManager.users.Length - count - 1 - 1; i >= 0; i--)
        {

            resultList.Add(HelloServer.SelectNum.DontKnow.ToString());
        }

        for (int i = 0; i < resultList.Count; i++)
        {
            Console.WriteLine($"[최종 투표 리스트] {i}번째 : {resultList[i]}");
        }
        result = GetWinner(resultList);
        Console.WriteLine($"[최종 투표 result 타입] : {result}");
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

            if (result != SelectNum.Liar.ToString() &&
                voteEndStateMessage.CurrentCycle >= gameManager.currentRoom.GameConfig.MaxCycle)
            {
                stateMachine.ChangeState<PointAtSuspectState>();
            }
            else if(result != SelectNum.Liar.ToString() &&
                    voteEndStateMessage.CurrentCycle < gameManager.currentRoom.GameConfig.MaxCycle)
            {
                stateMachine.ChangeState<ShowItemAndSpeakState>();
            }
            else if(result == SelectNum.Liar.ToString())
            {
                stateMachine.ChangeState<LiarConfirmedState>();
            }
        }
    }
    private string GetWinner(List<string> list)
    {
        string result = "";
        // 데이터가 없으면 빈 값 반환
        if (list == null || list.Count == 0) return "";

        Dictionary<string, int> dict = new Dictionary<string, int>();
        foreach (var VARIABLE in list)
        {
            if (dict.ContainsKey(VARIABLE) == false)
            {
                dict.Add(VARIABLE, 1);
            }
            else
            {
                dict[VARIABLE]++;
            }
        }

        
        List<KeyValuePair<string, int>> orderedList = dict.OrderByDescending(x => x.Value).ToList();
        
        // 만약 라이어와 
        if (orderedList.Count > 1 && orderedList[0].Value == orderedList[1].Value
                                  && ((orderedList[0].Key == SelectNum.DontKnow.ToString() && orderedList[1].Key == SelectNum.Liar.ToString())
                                      || orderedList[1].Key == SelectNum.DontKnow.ToString() && orderedList[0].Key == SelectNum.Liar.ToString()))
        {
            result = SelectNum.Liar.ToString();
        }
        else if (orderedList.Count > 1 && orderedList[0].Value == orderedList[1].Value)
        {
            result = "";
        }
        else if (orderedList[0].Key == SelectNum.NotLiar.ToString())
        {
            result = SelectNum.NotLiar.ToString();
        }
        else if (orderedList[0].Key == SelectNum.DontKnow.ToString())
        {
            result = SelectNum.DontKnow.ToString();
        }
        else if (orderedList[0].Key == SelectNum.Liar.ToString())
        {
            result = SelectNum.Liar.ToString();
        }
        // 동률인 경우 빈 값 반환
        return result;
    }
}