﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAnim : MonoBehaviour {

    public float m_targetScale = 5.0f;
    public float m_speed = 1.0f;
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
        Vector3 targetScale = new Vector3(m_targetScale, m_targetScale, 1.0f);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime);

        if(Time.time - m_startTime > m_speed)
            Destroy(gameObject);
    }
}