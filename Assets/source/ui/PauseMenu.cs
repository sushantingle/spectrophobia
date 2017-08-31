using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnEnable()
    {
        GameManager.getInstance().pauseGame(true);
    }

    public void OnClickResume()
    {
        GameManager.getInstance().pauseGame(false);
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_HUD);
    }

    public void OnClickExit()
    {
        GameManager.getInstance().exitGame();
        DerivedNetworManager.getInstance().stopHost();
    }
}
