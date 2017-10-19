using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimData {

    public float m_duration = 0.0f;
    public Vector3 m_position = Vector3.zero;
}

public class BlastAnimData : AnimData{
    public Vector3 m_originalScale;
    public Vector3 m_targetScale;

    public BlastAnimData(Transform transform, Vector3 targetScale, float duration)
    {
        m_originalScale = transform.localScale;
        m_position = transform.position;
        m_targetScale = targetScale;
        m_duration = duration;
    }
}
