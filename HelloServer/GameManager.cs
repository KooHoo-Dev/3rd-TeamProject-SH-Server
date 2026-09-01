using System.Collections.Concurrent;
using HelloServer.State;
using Jay.FSM;

namespace HelloServer;

public class GameManager
{
    
    StateMachine<IState> stateMachine;

    public IState currentTurnState => stateMachine.CurrentState;

    public bool IsGameRunning = false;

    public Room currentRoom;

    
    public GameStartState startState;
    public GenreAssignAndLiarSelectState genreAssignAndLiarSelectState;
    public KeywordDistributeState keywordDistributeState;
    public MartEnterState martEnterState;
    public MartMoveState martMoveState;
    public MartReturnState martReturnState;
    public ShowItemAndSpeakState showItemAndSpeakState;
    public SpeechEndState speechEndState;
    public PointAtSuspectState pointAtSuspectState;
    public PointAtSuspectEndState pointAtSuspectEndState;
    public DebateTimeState debateTimeState;
    public DebateEndState debateEndState;
    public VoteState voteState;
    public VoteEndState voteEndState;
    public LiarConfirmedState liarConfirmedState;
    public LiarKeywordGuessState liarKeywordGuessState;
    public LiarKeywordGuessEndState liarKeywordGuessEndState;
    public ScoreTallyState scoreTallyState;
    public ScoreTallyEndState scoreTallyEndState;
    public FinalResultState finalResultState;
    public FinalResultEndState finalResultEndState;
    public LiarOutButtonPressedState  liarOutButtonPressedState;
    
    public CategoryType[] AllCategories;
    public CategoryType currentCategory;
    public int currentCycle = 0;
    public int currentRound = 0;
    public int currentSpeakedCount = 0;
    public int maxSpeakedCount = 0;
    // 라이어가 라이어 버튼을 눌렀을 경우 추가되는 필드
    private string liarButtonPressedUserId;
    
    public User[] users;
    public User focausUser;



    // 지목 유저의 변경은 오로지 지목 턴에만 변경 가능하도록
    private string mostFrequent;
    public string MostFrequent
    {
        get
        {
            lock (gameLock)
            {
                return mostFrequent;
            }
        }
        set
        {
            lock (gameLock)
            {
                mostFrequent = value;
                
            }
        }
    }

    public GenreDef CurrentGanre = new GenreDef();
    public KeyWordDef CurrentKeyWord = new KeyWordDef();
    public KeyWordDef CurrentLiarKeyword = new KeyWordDef();
    public List<KeyWordDef> OldKeyWords = new List<KeyWordDef>();
   

    public readonly SemaphoreSlim gameLock 
        = new SemaphoreSlim(1, 1);

    #region 비동기 함수에서 보내는 정보들
    public readonly ConcurrentQueue<VoteMessage> VoteQueue = new ConcurrentQueue<VoteMessage>();
    
    // 라이어 버튼을 누른 '일반 유저ID'가 담기는 버튼
    public readonly ConcurrentQueue<string> LiarOutButtonQueue = new ConcurrentQueue<string>();
    



    private string liarGuessKeyWord;

    public string LiarGuessKeyWord
    {

        get
        {
            lock (gameLock)
            {
                return liarGuessKeyWord;
            }
        }
        set
        {
            lock (gameLock)
            {
                liarGuessKeyWord = value;
            }
        }
    }


    private bool changeSpeakerTrigger = false;
    public bool ChangeSpeakerTrigger
    {
        get
        {
            lock (gameLock)
            {
                return changeSpeakerTrigger;
            }
        }
        set
        {
            lock (gameLock)
            {
                changeSpeakerTrigger = value;
            }
        }
    }

    public string PressedLiarId
    {
        get
        {
            lock (gameLock)
            {
                return liarButtonPressedUserId;
            }

        }
        set{
            lock (gameLock)
            {

                    liarButtonPressedUserId = value;
                
            }
        }

    }
    public int skipCount = 0;
    public int SkipCount
    {
        get
        {
            lock (gameLock)
            {
                return skipCount;
            }
        }
        set
        {
            lock (gameLock)
            {
                skipCount = value;
            }
        }
    }

    // Key :한 유저, Value : 지목을 받은 유저 (만약 없다면 빈 스트링)(모든 유저가 key값으로 있음)
    public readonly ConcurrentDictionary<string, string> PointInfo = new ConcurrentDictionary<string, string>();
    
    #endregion

    public  GameManager(GameConfig gameConfig, Room currentRoom)
    {
        
        this.currentRoom = currentRoom;
        stateMachine = new StateMachine<IState>();
        
        
        
        startState = new GameStartState(stateMachine, this, gameConfig.stateGameStartTime);
        genreAssignAndLiarSelectState =
            new GenreAssignAndLiarSelectState(stateMachine, this, gameConfig.stateGenreAssignAndLiarSelectTime);
        keywordDistributeState = new KeywordDistributeState(stateMachine, this, gameConfig.stateKeywordDistributeTime);
        martEnterState = new MartEnterState(stateMachine, this, gameConfig.stateMartEnterTime);
        martMoveState = new MartMoveState(stateMachine, this, gameConfig.stateMartMoveTime);
        martReturnState = new MartReturnState(stateMachine, this, gameConfig.stateMartReturnTime);
        showItemAndSpeakState = new ShowItemAndSpeakState(stateMachine, this, gameConfig.stateShowItemAndSpeakTime);
        speechEndState = new SpeechEndState(stateMachine, this, gameConfig.stateSpeechEndTime);
        pointAtSuspectState = new PointAtSuspectState(stateMachine, this, gameConfig.statePointAtSuspectTime);
        pointAtSuspectEndState = new PointAtSuspectEndState(stateMachine, this, gameConfig.statePointAtSuspectEndTime);
        debateTimeState = new DebateTimeState(stateMachine, this, gameConfig.stateDebateTime);
        debateEndState = new DebateEndState(stateMachine, this, gameConfig.stateDebateEndTime);
        voteState = new VoteState(stateMachine, this, gameConfig.stateVoteTime);
        voteEndState = new VoteEndState(stateMachine, this, gameConfig.stateVoteEndTime);
        liarConfirmedState = new LiarConfirmedState(stateMachine, this, gameConfig.stateLiarConfirmedTime);
        liarKeywordGuessState = new LiarKeywordGuessState(stateMachine, this, gameConfig.stateLiarKeywordGuessTime);
        liarKeywordGuessEndState = new LiarKeywordGuessEndState(stateMachine, this, gameConfig.stateLiarKeywordGuessEndTime);
        scoreTallyState = new ScoreTallyState(stateMachine, this, gameConfig.stateScoreTallyTime);
        scoreTallyEndState = new ScoreTallyEndState(stateMachine, this, gameConfig.stateScoreTallyEndTime);
        finalResultState = new FinalResultState(stateMachine, this, gameConfig.stateFinalResultTime);
        finalResultEndState = new FinalResultEndState(stateMachine, this, gameConfig.stateFinalResultEndTime);
        liarOutButtonPressedState = new LiarOutButtonPressedState(stateMachine,this, gameConfig.stateLiarOutButtonPressedTime);
        
        stateMachine.Add(startState);
        stateMachine.Add(genreAssignAndLiarSelectState);
        stateMachine.Add(keywordDistributeState);
        stateMachine.Add(martEnterState);
        stateMachine.Add(martMoveState);
        stateMachine.Add(martReturnState);
        stateMachine.Add(showItemAndSpeakState);
        stateMachine.Add(speechEndState);
        stateMachine.Add(pointAtSuspectState);
        stateMachine.Add(pointAtSuspectEndState);
        stateMachine.Add(debateTimeState);
        stateMachine.Add(debateEndState);
        stateMachine.Add(voteState);
        stateMachine.Add(voteEndState);
        stateMachine.Add(liarConfirmedState);
        stateMachine.Add(liarKeywordGuessState);
        stateMachine.Add(liarKeywordGuessEndState);
        stateMachine.Add(scoreTallyState);
        stateMachine.Add(scoreTallyEndState);
        stateMachine.Add(finalResultState);
        stateMachine.Add(finalResultEndState);
        stateMachine.Add(liarOutButtonPressedState);
        
    }

    public void GameStart()
    {
        if(IsGameRunning) return;
        // if(currentRoom.members.Count < 3)
        // {
        //     Console.WriteLine($"[총 유저가 3명 미만] 총 유저 수 : {currentRoom.members.Count}");
        //     return;
        // };
        Init();
        stateMachine.ChangeState<GameStartState>();
    }

    private void Init()
    {
        IsGameRunning = true;
        Console.WriteLine($"테스트1번 위치");
        List<Room.Member> memberList = currentRoom.members.Values.ToList(); 
        users = new User[memberList.Count];
        for (int i = 0; i < memberList.Count; i++)
        {
            users[i] = memberList[i].User;
        }

        for (int i = 0; i < currentRoom.members.Count; i++)
        {

            currentRoom.members[users[i].Id].score = 0;
            currentRoom.members[users[i].Id].IsReady = false;
            
        }

        Console.WriteLine($"테스트2번 위치 및 리스트 갯수: {users.Length}");
        for(int i = 0; i < users.Length; i++)
        {
        Console.WriteLine($"테스트2.2번 위치 {users[i]?.Id}");
           bool s = PointInfo.TryAdd(users[i]?.Id, "");
        Console.WriteLine($"테스트2.3번 위치 {users[i]?.Id} 성공 여부: {s}");
            
        }
        Console.WriteLine($"테스트3번 위치");
        SetRandomCategories();
        Console.WriteLine($"테스트4번 위치");

        currentCycle = 0;
        currentRound = 0;
        currentCategory = AllCategories[0];
        maxSpeakedCount = users.Length;
        User focausUser = new User();

        OldKeyWords = new List<KeyWordDef>();
    }

    private void SetRandomCategories()
    {
        CategoryType[] temp = new CategoryType[currentRoom.members.Count];
        Console.WriteLine($"테스트2.1번 위치");
        Random randomObj = new Random();

        List<int> list = new List<int>();
        
        int randomValue;

        for(int i = 0; i < currentRoom.GameConfig.MaxCycle; i++)
        {
            randomValue = randomObj.Next(0,DataManager.Instance.ItemCategories.Count);

            int MaxCount = 10;
            if (list.Contains(randomValue))
            {
                while (true)
                {
                    if (list.Contains(randomValue) == false || MaxCount < 0)
                    {
                        break;
                    }
                    randomValue = randomObj.Next(0,DataManager.Instance.ItemCategories.Count);
                    MaxCount--;
                }
            }
            list.Add(randomValue);
            
        }

        Console.WriteLine($"테스트2.3번 위치 및 PointInfo: {PointInfo.Count}");
        AllCategories = new CategoryType[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            AllCategories[i] = (CategoryType)list[i];
        }
        Console.WriteLine($"테스트2.4번 위치 및 AllCategories: {AllCategories.Length}");

    }
    public void GameEnd()
    {
        IsGameRunning = false;
        stateMachine.StopStateMachine();
        PointInfo.Clear();
        CurrentGanre = new GenreDef();
        CurrentKeyWord  = new KeyWordDef();
        CurrentLiarKeyword = new KeyWordDef();
        OldKeyWords.Clear();
    }
}