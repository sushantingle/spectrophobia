using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine;

public class GPGSInterface
{

    private static string GLOBAL_LEADERBOARD_ID = "CgkIzI3HzbcZEAIQAg";

    public static void init()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        //.EnableSavedGames()                                                   // TODO: Fix crash
        // requests the email address of the player be available.
        // Will bring up a prompt for consent.
        .RequestEmail()
        // requests a server auth code be generated so it can be passed to an
        //  associated back end server application and exchanged for an OAuth token.
        .RequestServerAuthCode(false)
        // requests an ID token be generated.  This OAuth token can be used to
        //  identify the player to other services such as Firebase.
        .RequestIdToken()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public static void loginGPGS()
    {
        // authenticate user:
        Social.localUser.Authenticate((bool success) => {
            // handle success or failure
            if (success)
                CustomDebug.Log("Login Success");
            else
                CustomDebug.Log("Login Failed");
        });
    }

    public static void postScore(int score)
    {
        // post score 12345 to leaderboard ID "Cfji293fjsie_QA")
        Social.ReportScore(score, GLOBAL_LEADERBOARD_ID, (bool success) => {
            // handle success or failure
            if (success)
                CustomDebug.Log("Score Submitted succesfully");
            else
                CustomDebug.Log("Score submission failed");
        });
    }

    public static void showGlobalLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI(GLOBAL_LEADERBOARD_ID);
    }
}
