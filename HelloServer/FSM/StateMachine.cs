using System;
using System.Collections.Generic;
using System.Linq;
using HelloServer;

namespace Jay.FSM
{
    public class StateMachine<TState> where TState : class, IState
    {
        private readonly List<TState> states = new List<TState>();
        public GameConfig gameConfig { get; set; } = new GameConfig();
        public TState CurrentState { get; private set; }

        
        public event Action<TState, TState> OnStateChanged;

        public StateMachine() { }

        public StateMachine(IEnumerable<TState> states)
        {
            AddRange(states);
        }

        public void Add(TState state)
        {
            if (state == null || states.Contains(state)) return;
            states.Add(state);
        }

        public void AddRange(IEnumerable<TState> range)
        {
            if (range == null) return;
            foreach (var s in range) Add(s);
        }

        public void ChangeState<T>() where T : TState
        {
            ChangeTo(states.FirstOrDefault(s => s is T), typeof(T));
            
        }

        public void ChangeState(Type stateType)
        {
            ChangeTo(states.FirstOrDefault(s => stateType.IsInstanceOfType(s)), stateType);
        }

        private void ChangeTo(TState next, Type requested)
        {
            if (next == null)
            {
                Console.WriteLine($"[StateMachine] 등록되지 않은 상태: {requested?.Name}");
                return;
            }

            if (ReferenceEquals(next, CurrentState)) return;
            
            var prev = CurrentState;

            // 보낼 메세지 조립 후 턴이 넘어갈 때마다 메세지 전송
                ChangeGameStateMessage NewChangeGameStateMessage = new ChangeGameStateMessage();


            prev?.Exit();
            CurrentState = next;
            next.Enter();
            NewChangeGameStateMessage.Type = next.GetGameStateString();
            // 밀리세컨드로 넘어갈 예정
            NewChangeGameStateMessage.Timer = next.MaxMsTime;
            NewChangeGameStateMessage.currentCategory = next.gameManager.currentCategory.ToString();
            NewChangeGameStateMessage.CurrentCycle = next.gameManager.currentCycle;
            NewChangeGameStateMessage.CurrentRound = next.gameManager.currentRound;
            NewChangeGameStateMessage.currentOwnerID = next.gameManager.focausUser?.Id;

            Console.WriteLine($"[상태 메세지] 바뀐 상태 : {next.GetType().Name} \n 보낸 메세지 : {NewChangeGameStateMessage.Type}");
           // next.gameManager.currentRoom.BroadcastAsync(NewChangeGameStateMessage);
            OnStateChanged?.Invoke(prev, next);
        }

        public bool IsCurrent<T>() where T : TState
        {
            return CurrentState is T;
        }

        public void Tick(float deltaTime)
        {
            if (CurrentState is IUpdatableState updatable)
            {
                updatable.Tick(deltaTime);
            }
        }
    }
}
