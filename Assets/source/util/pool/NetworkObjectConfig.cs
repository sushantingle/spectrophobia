using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkObjectConfig : NetworkBehaviour {

    [SerializeField]
    List<Behaviour> m_components;  // components which should be excluded while toggling other components

    [SerializeField]
    List<GameObject> m_childs; //children which should be excluded while toggling status of other children
    NetworkInstanceId m_parent = NetworkInstanceId.Invalid;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool hasLocalOwner()
    {
        if (m_parent == NetworkInstanceId.Invalid)
            return false;
        GameObject obj = null;
        if (GameManager.getInstance().isServer())
            obj = NetworkServer.FindLocalObject(m_parent);
        else
            obj = ClientScene.FindLocalObject(m_parent);

        return obj.GetComponent<NetworkBehaviour>().isLocalPlayer;
    }

    public void onActivate()
    {
        foreach (Behaviour b in GetComponents<Behaviour>())
        {
            if(m_components.Contains(b) == false)
                b.enabled = true;
        }

        foreach (GameObject obj in m_childs)
        {
            if(m_childs.Contains(obj) == false)
                obj.SetActive(true);
        }
    }

    public void onDeactivate()
    {
        foreach (Behaviour b in m_components)
        {
            if(m_components.Contains(b) == false)
                b.enabled = false;
        }

        foreach (GameObject obj in m_childs)
        {
            if(m_childs.Contains(obj) == false)
               obj.SetActive(false);
        }
    }

}
