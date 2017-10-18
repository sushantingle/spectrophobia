using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    private bool toggleAutoAim = true;
    public GameObject m_multiplayerBtn;
	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
#if !ENABLE_MULTIPLAYER
        m_multiplayerBtn.SetActive(false);
#endif
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void OnGUI()
    {
        //toggleAutoAim = GUI.Toggle(new Rect(Screen.width - 80, 5, 80, 50), toggleAutoAim, "Auto Aim");
        //if(GameManager.getInstance() != null && GameManager.getInstance().m_player != null)
        //    GameManager.getInstance().m_player.m_autoAim = toggleAutoAim; // TODO
    }

    public void onClickSinglePlayer()
    {
        GameManager.getInstance().setGameplayMode(GameManager.GameplayMode.SINGLE_PLAYER);
        DerivedNetworManager.getInstance().startHost();
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
}
