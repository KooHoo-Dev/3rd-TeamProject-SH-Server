using System.Timers;
using Jay.FSM;
using Study.MiniDefence;

namespace HelloServer.State;

public class KeywordDistributeState : GameTurnState
{
    public KeywordDistributeState(StateMachine<IState> stateMachine, GameManager gameManager, float MaxMsTime) : base(stateMachine, gameManager, MaxMsTime)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Random rnd = new Random();
        GenreDef genreDef = DataManager.Instance.Genres.Get(rnd.Next(DataManager.Instance.Genres.Count));
        string ganre = genreDef.GenreName;
        List<KeyWordDef> list = DataManager.Instance.GetKeyWordDefsByGenre(ganre);
        KeyWordDef selectedKeyWordDef = list[rnd.Next(0, list.Count)];
        KeywordMessage keywordMessage = new KeywordMessage();
        keywordMessage.Ganre = genreDef;
        keywordMessage.Keyword = selectedKeyWordDef;
        gameManager.currentRoom.BroadcastAsync(keywordMessage);
    }

    public override string GetGameStateString()
    {
        return gameManager.allStateString.stateKeywordDistribute;
    }
    protected override void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        base.OnTimedEvent(sender, e);
        if (currentMsTime > MaxMsTime)
        {
            stateMachine.ChangeState<MartEnterState>();
        }
    }
}