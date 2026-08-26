using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public abstract class GameTurnState : IState
{
    public StateMachine<IState> stateMachine;
    public GameManager gameManager { get; set; }
    private System.Timers.Timer timer;
    public int IntarvelMs = 200;
    public int currentMsTime;
    public float MaxMsTime{get;set;}

    public GameState GameStateStrings = new GameState();
    public GameTurnState(StateMachine<IState> stateMachine,GameManager gameManager, float MaxMsTime)
    {
        this.stateMachine = stateMachine;
        this.gameManager = gameManager;
        this.MaxMsTime = MaxMsTime;
    }



    public virtual void Enter()
    {
        timer = new System.Timers.Timer(IntarvelMs);
     timer.AutoReset = false;
     currentMsTime = (int)MaxMsTime;
     timer.Elapsed += OnTimedEvent;
    }
    public virtual string GetGameStateString()
    {
        return null;
    }

    public virtual void Exit()
    {
     timer.Elapsed -= OnTimedEvent;
     timer.Stop(); 
     timer.Dispose();
    }
    protected virtual void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        currentMsTime += IntarvelMs;
        

    }
}