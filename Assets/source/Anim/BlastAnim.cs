using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAnim : MonoBehaviour {

    public Vector3 m_targetScale = Vector3.zero;
    public float m_duration = 1.0f;
    Vector3 originalScale;

    private float m_startTime;
    // Use this for initialization
    void Start()
    {
        originalScale = transform.localScale;
        m_startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, m_targetScale, Time.deltaTime);

        if(Time.time - m_startTime > m_duration)
            Destroy(gameObject);
    }
}
