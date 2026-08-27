using HelloServer.State;
using Jay.FSM;
using Study.MiniDefence;

namespace HelloServer;

public class GameManager
{
    
    StateMachine<IState> stateMachine;

    public bool IsGameRunning = false;
    public GameState allStateString = new GameState();
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
    
    public CategoryType[] AllCategories;
    public CategoryType currentCategory;
    public int currentCycle = 0;
    public int currentRound = 0;
    public User[] users;
    public User focausUser = null;
    public string CurrentGanre;
    public string currentNomalKeyword;
    public string currentLiarKeyword;
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
        finalResultEndState = new FinalResultEndState(stateMachine, this, gameConfig.stateFinalResultTime);
        
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
        stateMachine.Add(finalResultState);
        stateMachine.Add(finalResultEndState);
        
    }

    public void GameStart()
    {
        if(IsGameRunning) return;
        Init();
        stateMachine.ChangeState<GameStartState>();
    }

    private void Init()
    {
        IsGameRunning = true;
        AllCategories = GetRandomCategories();
        currentCycle = 0;
        currentRound = 0;
        currentCategory = AllCategories[0];
        users = new User[currentRoom.members.Count];
        int index = 0;
        foreach (Room.Member member  in currentRoom.members.Values)
        {

            users[index] = member.User;
            index++;
        }
    }

    private CategoryType[] GetRandomCategories()
    {
        CategoryType[] temp = new CategoryType[currentRoom.members.Count];
        
        Random randomObj = new Random();

        List<int> list = new List<int>();
        
        int randomValue;

        for(int i = 0; i < currentRoom.GameConfig.MaxCycle; i++)
        {
            randomValue = randomObj.Next(DataManager.Instance.ItemCategories.Count);

            if (list.Contains(randomValue) == false)
                list.Add(randomValue);
        }

        AllCategories = new CategoryType[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            temp[i] = (CategoryType)list[i];
        }
        return  temp;

    }
    public void GameEnd()
    {
        
    }
}