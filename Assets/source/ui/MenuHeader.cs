using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuHeader : MonoBehaviour {

    public Text     m_candyCount;
    public Text     m_globalScore;

    public Text     m_xp;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        m_candyCount.text = "" + ItemDefs.getItemCount(ItemDefs.ItemType.ITEM_ID_CANDY);
        m_globalScore.text = "" + GameManager.getInstance().getGlobalScore();
        m_xp.text = "" + GameManager.getInstance().getXp();
	}

    public void onClickPause()
    {
        CustomDebug.Log("on Click Pause");
        StateManager.getInstance().pushState(StateManager.MenuState.STATE_PAUSE);
    }
}
