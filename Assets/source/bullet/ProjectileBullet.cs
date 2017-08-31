using UnityEngine;
using System.Collections;

public class ProjectileBullet : BulletBase {

    private float m_maxdistance = 5.0f;
    private float m_angle = 0.0f;
    private float m_maxtime = 2.0f;
    private float m_time = 0.0f;
    private Vector3 m_startPos;
    private float m_vo = 0.0f;
    private float m_gravity = 1.0f;
    private float m_projAngle = 0.0f;

    public enum Direction
    {
        DIRECTION_LEFT = -1,
        DIRECTION_RIGHT = 1,
    }
    private Direction m_projDirection = Direction.DIRECTION_LEFT;

    // Use this for initialization
    void Start() {
        BStart();
    }

    protected override void BStart()
    {
        m_startPos = transform.position;
    }

    // Update is called once per frame
    void Update() {
        BUpdate();
    }

    protected override void BUpdate()
    {
        if (m_time >= m_maxtime)
        {
            BulletManager.getInstance().onDestroyBullet(gameObject);
            return;
        }
        Vector3 pos = transform.position;

        float vx = m_vo * Mathf.Cos(Mathf.Deg2Rad * m_projAngle) * m_time;
        float vy = m_vo * Mathf.Sin(Mathf.Deg2Rad * m_angle) * m_time - 0.5f * m_gravity * m_time * m_time;

        m_angle = Mathf.Lerp(0, 180, m_time / m_maxtime);
        m_time += (Time.deltaTime / m_maxtime);

        pos.x = m_startPos.x + vx * (int)m_projDirection;
        pos.y = m_startPos.y + vy;

        Vector3 direction = pos - transform.position;
        float angle = Vector3.Angle(transform.right, direction);
        angle = Vector3.Cross(transform.right, direction).z < 0 ? (360.0f - angle) % 360.0f : angle;
        transform.Rotate(new Vector3(0.0f, 0.0f, angle));
        transform.position = pos;

    }
    public void setup(Direction d, float distance, float time, float gravity, float angle)
    {
        m_projDirection = d;
        m_maxdistance = distance;
        m_maxtime = time;
        m_gravity = gravity;
        m_projAngle = angle;
        m_vo = m_maxdistance / m_maxtime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.BOnTriggerEnter2D(collision);
    }
}
