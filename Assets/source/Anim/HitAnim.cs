using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAnim : MonoBehaviour {

    public float m_duration = 1.0f;
    private float m_startTime;
    // Use this for initialization
    void Start () {
        m_startTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time - m_startTime > m_duration)
            Destroy(gameObject);
    }
}
