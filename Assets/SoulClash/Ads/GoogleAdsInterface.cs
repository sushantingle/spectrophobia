using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class GoogleAdsInterface {

    private RewardBasedVideoAd m_rewardBasedVideoAd;
    private static String AD_UNIT_ID_CONTINUE_GAME = "ca-app-pub-6537726950016872/5493362540";
    // Use this for initialization
    public void init () {
        m_rewardBasedVideoAd = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        m_rewardBasedVideoAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        m_rewardBasedVideoAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        m_rewardBasedVideoAd.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        m_rewardBasedVideoAd.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        m_rewardBasedVideoAd.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        m_rewardBasedVideoAd.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        m_rewardBasedVideoAd.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
    }

    public void requestRewardedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = AD_UNIT_ID_CONTINUE_GAME;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice("C0A3B33E0004DADAF457AF37555CDD67")
            .Build();
        // Load the rewarded video ad with the request.
        RewardBasedVideoAd.Instance.LoadAd(request, adUnitId);

        if (CustomDebug.isDebugBuild())
        {
            List<string> testDevices = request.TestDevices;
            foreach (string str in testDevices)
                CustomDebug.Log("Test Devices : " + str);
        }
#endif
    }

    public void showRewardedAd()
    {
        //if (m_rewardBasedVideoAd.IsLoaded())
        {
            m_rewardBasedVideoAd.Show();
        }
    }

    public bool isLoaded()
    {
        return m_rewardBasedVideoAd.IsLoaded();
    }

    private void HandleRewardBasedVideoLeftApplication(object sender, EventArgs e)
    {
        CustomDebug.Log("Callback RewardBasedVideoLeftApplication : " + e.ToString());
    }

    private void HandleRewardBasedVideoClosed(object sender, EventArgs e)
    {
        CustomDebug.Log("Callback RewardBasedVideoClosed : " + e.ToString());
        AdsManager.OnRewardedAdClosed();
    }

    private void HandleRewardBasedVideoRewarded(object sender, Reward e)
    {
        CustomDebug.Log("Callback RewardBasedVideoRewarded : Type = "+ e.Type +"    Amount = " + e.Amount.ToString());
        AdsManager.OnRewardedAdRewarded(e.Type, e.Amount);
    }

    private void HandleRewardBasedVideoStarted(object sender, EventArgs e)
    {
        CustomDebug.Log("Callback RewardBasedVideoStarted : " + e.ToString());
        AdsManager.OnRewardedAdStarted();
    }

    private void HandleRewardBasedVideoOpened(object sender, EventArgs e)
    {
        CustomDebug.Log("Callback RewardBasedVideoOpened : " + e.ToString());
    }

    private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        CustomDebug.Log("Callback RewardBasedVideoFailedToLoad : " + e.Message);
        AdsManager.OnRewardedAdFailedToLoad();
    }

    private void HandleRewardBasedVideoLoaded(object sender, EventArgs e)
    {
        CustomDebug.Log("Callback RewardBasedVideoLoaded : " + e.ToString());
        AdsManager.OnRewardedAdLoaded();
    }

   
}
