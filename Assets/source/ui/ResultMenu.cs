using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultMenu : MonoBehaviour {

    public Text m_playerScore;
    public Text m_highScore;
    public GameObject m_rewardText;
    public GameObject m_lbBtn;
	// Use this for initialization
	void Start () {

	}

    private void OnEnable()
    {
        if (GameManager.getInstance().getGlobalScore() > GameStats.HIGHSCORE)
        {
            GameStats.HIGHSCORE = GameManager.getInstance().getGlobalScore();
            m_rewardText.SetActive(true);
        }
        else
        {
            m_rewardText.SetActive(false);
        }

        m_playerScore.text = "Your Score : " + GameManager.getInstance().getGlobalScore();
        m_highScore.text = "HighScore : " + GameStats.HIGHSCORE;

        #if UNITY_ANDROID
            GPGSInterface.postScore(GameManager.getInstance().getGlobalScore());
        #else
            m_lbBtn.SetActive(false);
        #endif
    }
    // Update is called once per frame
    void Update () {
		
	}

    public void onClickOK()
    {
        GameManager.getInstance().exitGame();
        DerivedNetworManager.getInstance().stopHost();
    }

    public void onClickGlobalLB()
    {
        #if UNITY_ANDROID
            GPGSInterface.showGlobalLeaderboard();
        #endif
    }
}
