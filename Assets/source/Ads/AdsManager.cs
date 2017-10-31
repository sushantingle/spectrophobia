﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager {
    static GoogleAdsInterface m_googleAdsInterface = new GoogleAdsInterface();

    public enum AD_REQUEST_ID {
        AD_REQUEST_NONE,
        AD_REQUEST_CONTINUE_GAME,
    }

    static AD_REQUEST_ID m_lastRequestId = AD_REQUEST_ID.AD_REQUEST_NONE;

    public static void init()
    {
        m_googleAdsInterface.init();
    }

    // Request
    public static void requestRewardedAd()
    {
        CustomDebug.Log("Request Rewarded Ad");
        m_googleAdsInterface.requestRewardedVideo();
    }

    public static bool showRewardedAd(AD_REQUEST_ID requestId)
    {
        CustomDebug.Log("show rewarde ad");
        if (m_googleAdsInterface.isLoaded())
        {
            CustomDebug.Log("Ad lOaded");
            m_lastRequestId = requestId;
            m_googleAdsInterface.showRewardedAd();
            return true;
        }
        return false;
    }

    static void processRequestCallback()
    {
        switch (m_lastRequestId)
        {
            case AD_REQUEST_ID.AD_REQUEST_CONTINUE_GAME:
                {
                    if (GameStats.SOULS > PlayerDefs.CONST_RETRY_GAME_PRICE)
                    {
                        GameManager.getInstance().continueGame();
                    }
                    else
                    {
                        StateManager.getInstance().pushState(StateManager.MenuState.STATE_RESULT);
                    }
                }
                break;
        }

        m_lastRequestId = AD_REQUEST_ID.AD_REQUEST_NONE;
        m_googleAdsInterface.requestRewardedVideo();
    }

    // Callback

    public static void OnRewardedAdClosed()
    {

    }

    public static void OnRewardedAdLoaded()
    {

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
        m_googleAdsInterface.requestRewardedVideo();
    }
}
