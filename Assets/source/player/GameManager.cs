using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
/// <summary>
/// /class : GameManager
/// /brief : starting point of game. handles add on functionalities such as pause game
/// </summary>
public class GameManager : MonoBehaviour {

    public enum GameplayMode {
        SINGLE_PLAYER,
        MULTIPLAYER,
    }

    private GameplayMode m_mode = GameplayMode.SINGLE_PLAYER;

	public GameObject m_startBtn; // start button

	private bool m_isPaused = true;
	private static GameManager m_instance;
	public Player	m_player;
    public GameObject m_mainCamera;

    // global score
    private int m_globalScore = 0;
    private int m_xp = 1;

    public List<GameObject> m_particlePrefabList;
    public int m_particlePreloadCount = 100;
    private NetworkObjectPool m_networkObjectPool;

    public NetworkObjectPool getNetworkPool() { return m_networkObjectPool; }

	public static GameManager getInstance() {
		return m_instance;
	}

	void Awake() {
		m_instance = this;
	}
	// Use this for initialization
	void Start () {
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_MAIN);

        foreach (GameObject obj in m_particlePrefabList)
        {
            ObjectPool.Preload(obj, m_particlePreloadCount);
        }
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
           // enables saving game progress.
           .EnableSavedGames()
           // requests the email address of the player be available.
           // Will bring up a prompt for consent.
           .RequestEmail()
           // requests a server auth code be generated so it can be passed to an
           //  associated back end server application and exchanged for an OAuth token.
           .RequestServerAuthCode(false)
           // requests an ID token be generated.  This OAuth token can be used to
           //  identify the player to other services such as Firebase.
           .RequestIdToken()
           .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    // on quit/game over
	public void endGame()
	{
		m_isPaused = true;
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_RETRY);
	}

    public void exitGame()
    {
        m_isPaused = true;
        reset();
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_MAIN);
    }

	public bool isGamePaused()
	{
		return m_isPaused;
	}

    public void pauseGame(bool value)
    {
        m_isPaused = value;
    }

    public void addScore(int score)
    {
        m_globalScore += score * m_xp;
    }

    public int getGlobalScore() { return m_globalScore; }

    public void incrementXP(int value = 1)
    {
        m_xp += value;
    }

    public int getXp() { return m_xp; }

    public void setGameplayMode(GameplayMode mode)
    {
        m_mode = mode;
    }

    public GameplayMode getGameplayMode() { return m_mode; }

    public bool isMultiplayer()
    {
        return (m_mode == GameplayMode.MULTIPLAYER);
    }

    public bool isSinglePlayer()
    {
        return (m_mode == GameplayMode.SINGLE_PLAYER);
    }

    public bool isServer()
    {
        return m_player.isServer;
    }

    public void onTeamSelect(Player.Player_Team team)
    {
        CustomDebug.Log("On Team Select");
        ClanManager.getInstance().SelectedTeam = team;
        startGame();
    }

    public void onStartLocalPlayer(GameObject obj)
    {
        m_player = obj.GetComponent<Player>();
        m_networkObjectPool = m_player.GetComponent<NetworkObjectPool>();
        EnemyManager.getInstance().preload();
        if (isSinglePlayer())
        {
            m_player.m_autoAim = GameSettings.AUTOAIM;
            startGame();
        }
#if ENABLE_MULTIPLAYER
        else if (isMultiplayer())
            StateManager.getInstance().pushState(StateManager.MenuState.STATE_TEAM_SELECTION);
#endif
    }

    private void startGame()
    {
        CustomDebug.Log("Start Game 1");
        pauseGame(false);
        m_player.onStartGame();
        m_mainCamera.GetComponent<CameraFollower>().setTarget(m_player.transform);

        EnemyManager.getInstance().reset();
        EnemyManager.getInstance().setPlayer(m_player.transform);
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_HUD);
        ItemManager.getInstance().usedCandy(PlayerDefs.CONST_START_GAME_PRICE);
        BulletManager.getInstance().setOnlineManager(m_player.gameObject);

        Social.localUser.Authenticate((bool success) => {
            // handle success or failure
            if (success)
                CustomDebug.Log("Login Success");
            else
                CustomDebug.Log("Login Failed");
        });
    }

    private void reset()
    {
        EnemyManager.getInstance().reset();
        m_globalScore = 0;
        m_player.resetPlayer();
        m_player = null;
        m_mainCamera.GetComponent<CameraFollower>().setTarget(null);
    }
}
