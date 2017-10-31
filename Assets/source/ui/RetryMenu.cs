using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetryMenu : MonoBehaviour {

    public Text m_retryText;
    public GameObject m_soulIcon;

    // Use this for initialization
    void OnEnable() {

        if (GameStats.SOULS > PlayerDefs.CONST_RETRY_GAME_PRICE)
        {
            m_retryText.text = "Continue : " + PlayerDefs.CONST_RETRY_GAME_PRICE;
            m_soulIcon.SetActive(true);
        }
        else
        {
            m_retryText.text = "Watch Ad To Continue";
            m_soulIcon.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnClickYes()
    {
        if (GameStats.SOULS > PlayerDefs.CONST_RETRY_GAME_PRICE)
        {
            GameManager.getInstance().continueGame();
        }
        else
        {
            if (AdsManager.showRewardedAd(AdsManager.AD_REQUEST_ID.AD_REQUEST_CONTINUE_GAME) == false)
            {
                StateManager.getInstance().pushState(StateManager.MenuState.STATE_RESULT);
            }
        }
    }

    public void onClickNo()
    {
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_RESULT);
    }
}
