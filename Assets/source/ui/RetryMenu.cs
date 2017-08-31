using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetryMenu : MonoBehaviour {

    public Text m_retryText;
    // Use this for initialization
    void Start() {
        m_retryText.text = "Retry : " + PlayerDefs.CONST_RETRY_GAME_PRICE + " candy";
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnClickYes()
    {
        GameManager.getInstance().pauseGame(false);
        GameManager.getInstance().m_player.onResumeGame();
        ItemManager.getInstance().usedCandy(PlayerDefs.CONST_RETRY_GAME_PRICE);
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_HUD);
    }

    public void onClickNo()
    {
        GameManager.getInstance().exitGame();
        DerivedNetworManager.getInstance().stopHost();
    }
}
