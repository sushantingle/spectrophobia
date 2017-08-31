using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerPath : PathBase {
    public float m_updateTargetTick = 3.0f;
    private float m_lastUpdateTick;
    private Vector3 m_targetPosition;

    public override void init(Transform _transform, float _speed, Transform _lookAt)
    {
        m_lookAt = _lookAt;
        base.init(_transform, _speed);

        m_lastUpdateTick = Time.time;
        m_targetPosition = m_lookAt.position;
        m_pathType = PathDefs.AI_PATH_TYPE.PATH_FOLLOW;
    }

    public override void update()
    {
        if (m_transform == null)
            return;
        base.update();
        if (Time.time - m_lastUpdateTick > m_updateTargetTick)
        {
            m_lastUpdateTick = Time.time;
            m_targetPosition = m_lookAt.position;
        }
        m_transform.position = Vector3.MoveTowards(m_transform.position, m_targetPosition, m_speed);
    }

    public override void fixedUpdate()
    {
        base.fixedUpdate();
    }
}
