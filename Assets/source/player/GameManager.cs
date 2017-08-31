using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public void OnGUI()
    {
        if (GUI.Button(new Rect(new Vector2(10, 10), new Vector2(100,50)), "Candy"))
            ItemManager.getInstance().collectedCandy(5);
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

    public void onStartLocalPlayer(GameObject obj)
    {
        m_player = obj.GetComponent<Player>();
        pauseGame(false);
        m_player.onStartGame();
        m_mainCamera.GetComponent<CameraFollower>().setTarget(obj.transform);

        EnemyManager.getInstance().reset();
        EnemyManager.getInstance().setPlayer(obj.transform);
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_HUD);
        ItemManager.getInstance().usedCandy(PlayerDefs.CONST_START_GAME_PRICE);
        BulletManager.getInstance().setOnlineManager(obj);
    }

    private void reset()
    {
        EnemyManager.getInstance().reset();
        m_player.resetPlayer();
        m_player = null;
        m_mainCamera.GetComponent<CameraFollower>().setTarget(null);
    }
}
