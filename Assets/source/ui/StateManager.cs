using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateDictionary : DictionaryTemplate<StateManager.MenuState, GameObject> { }

[System.Serializable]
public class PopupDictionary : DictionaryTemplate<StateManager.PopupType, GameObject> { }

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

    public enum PopupType {
        POPUP_NONE,
        POPUP_RECEIVED_REWARD,
        POPUP_NO_AD_AVAILABLE,
        POPUP_NOT_ENOUGH_SOUL,
        POPUP_WATCH_AD,
    }

    private static StateManager m_instance;
    private MenuState           m_currentStateId = MenuState.STATE_NONE;
    private PopupType           m_popupId = PopupType.POPUP_NONE;

    public List<StateDictionary> m_stateArray;
    public List<PopupDictionary> m_popupArray;

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
            getStateObject(m_currentStateId).SetActive(false);

        if(state != MenuState.STATE_NONE)
            getStateObject(state).SetActive(true);

        m_currentStateId = state;
    }

    public GameObject getStateObject(MenuState id)
    {
        var obj = m_stateArray.Find(item => item._key == id);
        if (obj != null)
            return obj._value;
        return null;
    }


    // Popup

    public PopupType getCurrentPopup()
    {
        return m_popupId;
    }

    public void pushPopup(PopupType type)
    {
        if (m_popupId != PopupType.POPUP_NONE)
            getPopupObject(m_popupId).SetActive(false);

        if (type != PopupType.POPUP_NONE)
            getPopupObject(type).SetActive(true);

        m_popupId = type;
    }

    public void popPopup()
    {
        if(m_popupId != PopupType.POPUP_NONE)
            getPopupObject(m_popupId).SetActive(false);

        m_popupId = PopupType.POPUP_NONE;
    }

    public GameObject getPopupObject(PopupType id)
    {
        var obj = m_popupArray.Find(item => item._key == id);
        if (obj != null)
            return obj._value;
        return null;
    }

}
