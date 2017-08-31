using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelection : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onClickCT()
    {
        //ClanManager.getInstance().SelectedTeam = Player.Player_Team.TEAM_CT;
        ClanManager.getInstance().spawnCT();
    }

    public void onClickT()
    {
        //ClanManager.getInstance().SelectedTeam = Player.Player_Team.TEAM_T;
        ClanManager.getInstance().spawnT();
    }
}
