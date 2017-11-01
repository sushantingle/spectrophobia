using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager {
    static GoogleAdsInterface m_googleAdsInterface = new GoogleAdsInterface();

    public enum AD_REQUEST_ID {
        AD_REQUEST_NONE,
        AD_REQUEST_CONTINUE_GAME,
        AD_REQUEST_MAINMENU,
    }

    static AD_REQUEST_ID m_lastRequestId = AD_REQUEST_ID.AD_REQUEST_NONE;
    private static bool m_adLoaded = false;
    private static bool m_hasRequested = false;

    public static void init()
    {
        m_googleAdsInterface.init();
    }

    // Request
    public static void requestRewardedAd()
    {
        if (m_hasRequested)
        {
            CustomDebug.Log("Ad Already requested");
            return;
        }
        CustomDebug.Log("Request Rewarded Ad");
        m_googleAdsInterface.requestRewardedVideo();
        m_adLoaded = false;
        m_hasRequested = true;
        m_lastRequestId = AD_REQUEST_ID.AD_REQUEST_NONE;
    }

    public static bool showRewardedAd(AD_REQUEST_ID requestId)
    {
        CustomDebug.Log("show rewarde ad");
        if (m_adLoaded)
        {
            CustomDebug.Log("Ad lOaded");
            m_lastRequestId = requestId;
            m_googleAdsInterface.showRewardedAd();
            m_hasRequested = false;
            return true;
        }
        return false;
    }

    static void processRequestCallback()
    {
        switch (m_lastRequestId)
        {
            case AD_REQUEST_ID.AD_REQUEST_CONTINUE_GAME:
            case AD_REQUEST_ID.AD_REQUEST_MAINMENU:
                {
                    GameObject state = StateManager.getInstance().getPopupObject(StateManager.PopupType.POPUP_RECEIVED_REWARD);
                    state.GetComponent<OKPopup>().setup("Received Reward : 1 Soul");
                    StateManager.getInstance().pushPopup(StateManager.PopupType.POPUP_RECEIVED_REWARD);
                }
                break;
        }
    }

    // Callback

    public static void OnRewardedAdClosed()
    {
        requestRewardedAd();
    }

    public static void OnRewardedAdLoaded()
    {
        m_adLoaded = true;
        CustomDebug.Log("Ad Loaded successfully");
    }

    public static void OnRewardedAdStarted()
    {

    }

    public static void OnRewardedAdRewarded(string type, double amount)
    {
        switch (type)
        {
            case "soul":
                GameStats.SOULS += (int)amount;
                break;
            // Temp
            case "coins":
                GameStats.SOULS += 2;
                break;
        }

        processRequestCallback();
    }

    public static void OnRewardedAdFailedToLoad()
    {
        requestRewardedAd();
    }
}
