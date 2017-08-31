using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour {

    public Text m_ipInfo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onClickHost()
    {
        DerivedNetworManager.getInstance().startHost();
    }

    public void onClickClient()
    {
        string[] ipInfo = m_ipInfo.text.Split(':');
        if (ipInfo.Length > 1) // ipInfo should contain ipAddress and port number
        {
            int port = 0;
            int.TryParse(ipInfo[1], out port);

            DerivedNetworManager.getInstance().startClient(ipInfo[0], port);
        }
    }
}
