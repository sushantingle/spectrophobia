using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchAdPopup : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onClickPlay()
    {
        if (AdsManager.showRewardedAd(AdsManager.AD_REQUEST_ID.AD_REQUEST_MAINMENU) == false)
        {
            GameObject state = StateManager.getInstance().getPopupObject(StateManager.PopupType.POPUP_NO_AD_AVAILABLE);
            state.GetComponent<OKPopup>().setup("No Ad Available");
            StateManager.getInstance().pushPopup(StateManager.PopupType.POPUP_NO_AD_AVAILABLE);
        }
    }

    public void onClickClose()
    {
        StateManager.getInstance().popPopup();
    }
}
