using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClanManager : NetworkBehaviour {

    private static ClanManager m_instance = null;

    private List<GameObject> m_ctList = new List<GameObject>();
    private List<GameObject> m_tList = new List<GameObject>();
    private Player.Player_Team m_selectedTeam;
    public Player.Player_Team SelectedTeam
    {
        get
        {
            return m_selectedTeam;
        }
        set
        {
            m_selectedTeam = value;
        }
    }

    public static ClanManager getInstance()
    {
        return m_instance;
    }

    private void Awake()
    {
        m_instance = this;
        setNetworkDelegate();
    }

    // Use this for initialization
    void Start () {
        
	}

    void setNetworkDelegate()
    {
        DerivedNetworManager.getInstance().setConnectionDelegates(OnServerConnect, OnClientConnect, OnStartHost, onServerAddPlayer);
        DerivedNetworManager.getInstance().setDisconnectionDelegates(OnServerDisconnect, OnClientDisconnect, OnStopHost);
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void OnServerConnect(bool success)
    {
        CustomDebug.Log("On server connect : " + success);
    }

    public void OnServerDisconnect()
    {
        CustomDebug.Log("On Server Disconnect");
    }

    public void OnClientConnect(bool success)
    {
        CustomDebug.Log("On Client Connect : "+success);
        ClientScene.AddPlayer(0);
        /*if (GameManager.getInstance().isSinglePlayer())
            
        else*/
#if ENABLE_MULTIPLAYER
        if (GameManager.getInstance().isMultiplayer())
        {
            CustomDebug.Log("Push Team selection");

            //StateManager.getInstance().pushState(StateManager.MenuState.STATE_TEAM_SELECTION);
        }
#endif
    }

    public void OnClientDisconnect()
    {
        CustomDebug.Log("On Client Disconnect");
    }

    public void OnStartHost(bool success)
    {
        CustomDebug.Log("On Start host");
    }

    public void OnStopHost()
    {
        CustomDebug.Log("On Stop Host");
        reset();
    }

    public void onServerAddPlayer(NetworkConnection conn)
    {
        GameManager.getInstance().getNetworkPool().OnServerAddPlayer(conn);
    }

    public void spawnCT()
    {
        //m_selectedTeam = Player.Player_Team.TEAM_CT;
        GameManager.getInstance().onTeamSelect(Player.Player_Team.TEAM_CT);
        ///ClientScene.AddPlayer(0);
    }

    public void spawnT()
    {
        //m_selectedTeam = Player.Player_Team.TEAM_T;
        CustomDebug.Log("Spawn T");
        GameManager.getInstance().onTeamSelect(Player.Player_Team.TEAM_T);
        //ClientScene.AddPlayer(0);
    }

    public List<GameObject> getCtList() { return m_ctList; }
    public List<GameObject> getTList() { return m_tList; }

    public void onSpawnedCT(GameObject obj)
    {
        if(m_ctList.Contains(obj) == false)
            m_ctList.Add(obj);
    }

    public void onSpawnedT(GameObject obj)
    {
        if(m_tList.Contains(obj) == false)
           m_tList.Add(obj);
    }

    public void onDestroyingCT(GameObject obj)
    {
        m_ctList.Remove(obj);
    }

    public void onDestroyingT(GameObject obj)
    {
        m_tList.Remove(obj);
    }

    private void reset()
    {
        foreach (GameObject obj in m_ctList)
            Destroy(obj);
        m_ctList.Clear();

        foreach (GameObject obj in m_tList)
            Destroy(obj);
        m_tList.Clear();
    }
}
