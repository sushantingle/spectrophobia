using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SineBullet : BulletBase {
    public float m_amplitude = 0.5f;
    private float m_life = 2 * Mathf.PI;
    private Vector3 m_startPos;

    private void Start()
    {
        base.BStart();
        m_startPos = transform.position;
    }

    private void Update()
    {
        BUpdate();
    }

    protected override void BUpdate()
    {
        Vector3 pos = transform.position;
        pos.x += m_direction.x * m_speed;
        if (m_life < 0.0f)
            m_life = 2 * Mathf.PI;
        
        pos.y = m_startPos.y + Mathf.Sin(m_life) * m_amplitude;
        m_life -= 0.1f; ;

        Vector3 direction = pos - transform.position;
        float angle = Vector3.Angle(transform.right, direction);
        angle = Vector3.Cross(transform.right, direction).z < 0 ? (360.0f - angle) % 360.0f : angle;
        transform.Rotate(new Vector3(0.0f, 0.0f, angle));
        transform.position = pos;

        if (isOutsideOfScreen())
        {
            BulletManager.getInstance().onDestroyBullet(gameObject);
        }
    }

    public void setup(Vector3 direction, float speed, float amplitude, NetworkInstanceId _parentNetId, float lifeStartPoint = Mathf.PI)
    {
        m_direction = (direction == Vector3.zero) ? Vector3.right : direction;
        m_speed = speed;
        m_amplitude = amplitude;
        m_life = lifeStartPoint;
        m_parentNetId = _parentNetId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.BOnTriggerEnter2D(collision);
    }
}
