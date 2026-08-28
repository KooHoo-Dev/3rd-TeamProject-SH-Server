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

    public GameTurnState(StateMachine<IState> stateMachine,GameManager gameManager, float MaxTime)
    {
        this.stateMachine = stateMachine;
        this.gameManager = gameManager;
        this.MaxMsTime = MaxTime * 1000;
    }



    public virtual void Enter()
    {
        if (gameManager?.currentRoom == null)
        {
            Console.WriteLine($"[조기 종료됨]; 게임매니저 == null :{gameManager == null}, 현재 방 == null : {gameManager?.currentRoom == null}");
            return;
        }
        timer = gameManager.currentRoom.timer;
     timer.AutoReset = true;
     currentMsTime = 0;
     timer.Elapsed += OnTimedEvent;
     timer.Start();
        Console.WriteLine($"[스테이트 머신] 현재 Enter 상태: {stateMachine.CurrentState}, 제한시간(ms): {MaxMsTime}");
     
    }
    public virtual string GetGameStateString()
    {
        return null;
    }

    public virtual void Exit()
    {
     timer.Elapsed -= OnTimedEvent;
     timer.Stop(); 
     
    }
    protected virtual void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        currentMsTime += IntarvelMs;
        Console.WriteLine($"[스테이트 머신][{GetGameStateString()}] 현재 Timer 상태: {currentMsTime}");
        

        
    }

    protected Task BroadcastAsync(object message, string exceptId = null)
    {
       return gameManager.currentRoom.BroadcastAsync(message, exceptId);
    }
    protected Task SendAsync(Room.Member member, object message)
    {
        return gameManager.currentRoom.SendAsync(member, message);
    }
}