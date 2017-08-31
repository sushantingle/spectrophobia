using UnityEngine;
using System.Collections;

public class LinearBullet : BulletBase {
	
	// Use this for initialization
	void Start () {
        base.BStart();
    }

	// Update is called once per frame
	void Update () {
        BUpdate();
	}

    protected override void BUpdate()
    {
        Vector3 pos = transform.position;

        pos += new Vector3(m_direction.x * m_speed, m_direction.y * m_speed, 0);
        transform.position = pos;

        if (isOutsideOfScreen())
        {
            BulletManager.getInstance().onDestroyBullet(gameObject);
        }
    }

    public void setup(Vector3 direction, float speed)
	{
		m_direction = direction;
		m_speed = speed;
        float angle = Vector3.Angle(transform.right, direction);
        angle = Vector3.Cross(transform.right, direction).z < 0 ? (360.0f - angle) % 360.0f : angle;
        transform.Rotate(new Vector3(0.0f, 0.0f, angle));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.BOnTriggerEnter2D(collision);
    }
}
