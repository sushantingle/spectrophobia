using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OKPopup : MonoBehaviour {

    public Text m_popupText;
    private StateManager.MenuState m_parentState = StateManager.MenuState.STATE_NONE;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setup(string text, StateManager.MenuState parentState = StateManager.MenuState.STATE_NONE)
    {
        m_popupText.text = text;
        m_parentState = parentState;
    }

    public void onOkClick()
    {
        switch (StateManager.getInstance().getCurrentPopup())
        {
            case StateManager.PopupType.POPUP_NO_AD_AVAILABLE:
                    StateManager.getInstance().popPopup();
                break;

            case StateManager.PopupType.POPUP_RECEIVED_REWARD:
                {
                    StateManager.getInstance().popPopup();
                    StateManager.MenuState globalState = StateManager.getInstance().getCurrentState();
                    if (globalState == StateManager.MenuState.STATE_RETRY)
                    {
                        if (GameStats.SOULS > PlayerDefs.CONST_RETRY_GAME_PRICE)
                        {
                            GameManager.getInstance().continueGame();
                        }
                        else
                        {
                            GameObject state = StateManager.getInstance().getPopupObject(StateManager.PopupType.POPUP_NOT_ENOUGH_SOUL);
                            state.GetComponent<OKPopup>().setup("Not enough souls in inventory.");
                            StateManager.getInstance().pushPopup(StateManager.PopupType.POPUP_NOT_ENOUGH_SOUL);
                        }
                    }
                    else if (globalState == StateManager.MenuState.STATE_MAIN)
                    {
                        if (GameStats.SOULS > PlayerDefs.CONST_START_GAME_PRICE)
                        {
                            GameManager.getInstance().startSinglePlayerGame();
                        }
                        else
                        {
                            GameObject state = StateManager.getInstance().getPopupObject(StateManager.PopupType.POPUP_NOT_ENOUGH_SOUL);
                            state.GetComponent<OKPopup>().setup("Not enough souls in inventory.");
                            StateManager.getInstance().pushPopup(StateManager.PopupType.POPUP_NOT_ENOUGH_SOUL);
                        }
                    }
                }
                break;
            case StateManager.PopupType.POPUP_NOT_ENOUGH_SOUL:
                {
                    StateManager.getInstance().popPopup();
                    StateManager.getInstance().pushState(StateManager.MenuState.STATE_RESULT);
                }
                break;
        }
    }
}
