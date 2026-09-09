using System.Timers;
using Jay.FSM;

namespace HelloServer.State;

public abstract class GameTurnState : IUpdatableState
{
    public StateMachine<IUpdatableState> stateMachine;
    public GameManager gameManager { get; set; }


    private int deltaMsTime;
    public int currentMsTime;
    public float MaxMsTime{get;set;}

    public GameTurnState(StateMachine<IUpdatableState> stateMachine,GameManager gameManager, float MaxTime)
    {
        this.stateMachine = stateMachine;
        this.gameManager = gameManager;
        this.MaxMsTime = MaxTime * 1000;
        deltaMsTime = gameManager.currentRoom.DeltaMs;
    }



    public virtual void Enter()
    {
        if (gameManager?.currentRoom == null)
        {
            Console.WriteLine($"[조기 종료됨]; 게임매니저 == null :{gameManager == null}, 현재 방 == null : {gameManager?.currentRoom == null}");
            return;
        }
        
     currentMsTime = 0;

        Console.WriteLine($"[스테이트 머신] 현재 Enter 상태: {stateMachine.CurrentState} , 현재 사이클 {gameManager.currentCycle} , 현재 라운드 {gameManager.currentRound} , 제한시간(ms): {MaxMsTime}");
     
    }
    public string GetGameStateString()
    {
        return $"{stateMachine.CurrentState?.ToString() ?? "null"}";
    }

    public virtual void Exit()
    {
        currentMsTime = 0;


    }
    public virtual void Tick(int deltaMs)
    {
        currentMsTime += deltaMsTime;
        if(currentMsTime % 5000 == 0)
            Console.WriteLine($"[{gameManager?.currentRoom?.code}][스테이트 머신][{GetGameStateString()?? "null"}] 현재 Timer 상태: {currentMsTime}");
        if (string.IsNullOrEmpty(gameManager?.currentRoom?.code))
        {
            Console.WriteLine($"[*현재 비 정상적으로 종료가 안된 상태*]");

        }

        
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