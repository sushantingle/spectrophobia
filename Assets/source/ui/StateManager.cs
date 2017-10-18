using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateManager : MonoBehaviour {

    public enum MenuState {
        STATE_NONE,
        STATE_MAIN,
        STATE_HUD,
        STATE_PAUSE,
        STATE_RETRY,
        STATE_RESULT,
        STATE_MULTIPLAYER,
        STATE_TEAM_SELECTION,
        STATE_WAITING_FOR_PLAYER,
        STATE_SETTINGS,
    }

    private static StateManager m_instance;
    private MenuState           m_currentStateId = MenuState.STATE_NONE;

    public List<GameObject> m_stateArray;

    public static StateManager getInstance()
    {
        return m_instance;
    }

    private void Awake()
    {
        m_instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public MenuState getCurrentState()
    {
        return m_currentStateId;
    }

    public void pushState(MenuState state)
    {
        if(m_currentStateId != MenuState.STATE_NONE)
            m_stateArray[(int)m_currentStateId].SetActive(false);

        if(state != MenuState.STATE_NONE)
            m_stateArray[(int)state].SetActive(true);

        m_currentStateId = state;
    }
}
