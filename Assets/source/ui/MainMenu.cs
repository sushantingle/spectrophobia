﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {

    private bool toggleAutoAim = true;
    public GameObject m_multiplayerBtn;
    public GameObject m_GPGSBtn;
    public GameObject m_lbBtn;
	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
#if !ENABLE_MULTIPLAYER
        m_multiplayerBtn.SetActive(false);
#endif

#if !UNITY_ANDROID
        m_GPGSBtn.SetActive(false);
        m_lbBtn.SetActive(false);
#endif
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "add soul"))
        {
            GameStats.SOULS += 1;
            CustomDebug.Log("Gamestats Soul: " + GameStats.SOULS);
        }
    }

    public void onClickSinglePlayer()
    {
#if UNITY_ANDROID
        if (GameStats.SOULS < PlayerDefs.CONST_START_GAME_PRICE)
            StateManager.getInstance().pushPopup(StateManager.PopupType.POPUP_WATCH_AD);
        else
#endif
            GameManager.getInstance().startSinglePlayerGame();
    }

    public void onClickMultiplayer()
    {
        GameManager.getInstance().setGameplayMode(GameManager.GameplayMode.MULTIPLAYER);

        StateManager.getInstance().pushState(StateManager.MenuState.STATE_MULTIPLAYER);
    }

    public void onClickSettings()
    {
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_SETTINGS);
    }

    public void onClickGoogleLogin()
    {
#if UNITY_ANDROID
            GPGSInterface.loginGPGS();
#endif
    }

    public void onClickGlobalLB()
    {
#if UNITY_ANDROID
            GPGSInterface.showGlobalLeaderboard();
#endif
    }
}
