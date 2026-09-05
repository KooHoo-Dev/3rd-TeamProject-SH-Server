using System.Collections.Concurrent;
using HelloServer.State;
using Jay.FSM;

namespace HelloServer;

public class GameManager
{

    public class UserInfo
    {
        public Protocol.User user;
        public bool IsLiar;
        public string[] ItemIds;
        public string HoldingItem;
        public int score;
        public bool IsQuestSuccess;
        public bool IsPushedState; // 현재 밀쳐진 상태인가?
        public UserInfo(Protocol.User user, int score)
        {
            this.user = user;
            this.score = score;

            
        }

        public UserInfo()
        {
            
        }
    }
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
    
    public ConcurrentDictionary<string,UserInfo> UserGameInfos;
    public Protocol.User focausUser;



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

    public ConcurrentDictionary<CategoryType,ConcurrentQueue<string>> AllMartItems = new ConcurrentDictionary<CategoryType,ConcurrentQueue<string>>();
    public readonly SemaphoreSlim gameLock 
        = new SemaphoreSlim(1, 1);

    #region 비동기 함수에서 보내는 정보들
    public readonly ConcurrentQueue<Protocol.VoteMessage> VoteQueue = new ConcurrentQueue<Protocol.VoteMessage>();
    
    // 라이어 버튼을 누른 '일반 유저ID'가 담기는 버튼
    public readonly ConcurrentQueue<string> LiarOutButtonQueue = new ConcurrentQueue<string>();


    private string liarId;

    public string LiarId
    {
        get
        {
            lock (gameLock)
            {
                return liarId;
            }
        }
        set
        {
            lock (gameLock)
            {
                liarId = value;
            }
        }
    }

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
    private int skipCount = 0;
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
    // Key :한 유저, Value : 목표 아이템ID (만약 없다면 빈 스트링)(모든 유저가 key값으로 있음)
    public readonly ConcurrentDictionary<string, string> QuestInfo = new ConcurrentDictionary<string, string>();
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
        UserGameInfos = new ConcurrentDictionary<string, UserInfo>();
        for (int i = 0; i < memberList.Count; i++)
        {
            UserGameInfos.TryAdd(memberList[i].User.Id, new UserInfo(memberList[i].User,0));
        }
        PointInfo.Clear();
        QuestInfo.Clear();
        foreach (var VARIABLE in UserGameInfos)
        {
            currentRoom.members[VARIABLE.Key].IsReady = false;
            bool s = PointInfo.TryAdd(VARIABLE.Key, "");
            bool q = QuestInfo.TryAdd(VARIABLE.Key, "");
        Console.WriteLine($"테스트2.3번 위치 {VARIABLE.Key} 성공 여부: {s}, {q}");
            
            
        }
        
            
        

        Console.WriteLine($"테스트3번 위치");
        MartItemsCategoryClear();
        SetRandomCategories();
        for (int i = 0; i < AllCategories.Length; i++)
        {
            AllMartItems.TryAdd(AllCategories[i], new ConcurrentQueue<string>());
        }
        Console.WriteLine($"테스트4번 위치");

        currentCycle = 0;
        currentRound = 0;
        currentCategory = AllCategories[0];
        maxSpeakedCount = UserGameInfos.Count;
        Protocol.User focausUser = new Protocol.User();

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

    public void MartItemsCategoryClear()
    {
        foreach (var VARIABLE in AllMartItems)
        {
            VARIABLE.Value?.Clear();
        }
        AllMartItems.Clear();
    }
    public void MartItemsClear()
    {
        foreach (var VARIABLE in AllMartItems)
        {
            VARIABLE.Value?.Clear();
        }

    }
    public void RemovePlayerSelectedItemFromBag()
    {
        foreach (var categoryType in AllCategories)
        {
            List<string> list = AllMartItems[categoryType].ToList();


            foreach (var userInfo in UserGameInfos)
            {
                for (int i = 0; i < userInfo.Value.ItemIds.Length; i++)
                {
                    if (list.Contains(userInfo.Value.ItemIds[i]))
                        list.Remove(userInfo.Value.ItemIds[i]);

                }
            }

            AllMartItems[categoryType].Clear();
            for (int i = 0; i < list.Count; i++)
            {
                AllMartItems[categoryType].Enqueue(list[i]);
            }
        }
    }

    public void ChangeCategory()
    {
        int currentCategoryIndex = -1;
        for (int i = 0; i < AllCategories.Length; i++)
        {
            if (currentCategory == AllCategories[i])
            {
                currentCategoryIndex = i;
                break;
            }
        }

        if (currentCategoryIndex != -1)
        {
            currentCategory = AllCategories[(currentCategoryIndex + 1) % AllCategories.Length];
            
        }
    }
}