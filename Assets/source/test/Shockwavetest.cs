using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwavetest : MonoBehaviour {
    public float m_targetScale = 5.0f;
    public float m_speed = 1.0f;
    Vector3 originalScale;
	// Use this for initialization
	void Start () {
        originalScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(m_targetScale, m_targetScale, 1.0f), Time.deltaTime * m_speed);
	}

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Again"))
        {
            again();
        }
    }

    void again()
    {
        transform.localScale = originalScale;
    }
}
