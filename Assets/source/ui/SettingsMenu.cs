using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    public Toggle m_autoAimToggle;
    public Toggle m_soundToggle;
	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
        GameSettings.m_autoAim = m_autoAimToggle.isOn;
        GameSettings.m_soundOn = m_soundToggle.isOn;
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void onClickAutoAim()
    {
        CustomDebug.Log("Enable Auto Aim : " + m_autoAimToggle.isOn);
        GameSettings.m_autoAim = m_autoAimToggle.isOn;
    }

    public void onClickSounds()
    {
        CustomDebug.Log("Enable Sound : " + m_soundToggle.isOn);
        GameSettings.m_soundOn = m_soundToggle.isOn;
    }

    public void onClickBackButton()
    {
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_MAIN);
    }
}
