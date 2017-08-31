using UnityEngine;
using System.Collections;

public class FollowerEnemy : EnemyBase {

    private void Awake()
    {
        m_pathType = PathDefs.AI_PATH_TYPE.PATH_FOLLOW;
    }
    // Use this for initialization
    void Start () {
//        EStart();
	}

    protected override void OnEnable()
    {
        base.OnEnable();
        EStart();
    }

    private void Update()
    {
        if (GameManager.getInstance().isGamePaused())
            return;

        EUpdate();
    }

    protected override void EUpdate()
    {
        base.EUpdate();
        m_path.update();
    }

    private void FixedUpdate()
    {
        if (GameManager.getInstance().isGamePaused())
            return;

        EFixedUpdate();
    }

    protected override void EFixedUpdate()
    {
        base.EFixedUpdate();
        m_path.fixedUpdate();
    }

    public override void setup(Transform lookAt)
	{
		m_lookAt = lookAt;
        m_path.init(transform, m_speed, m_lookAt);
    }

    public void setup(float speed, float health, Transform lookAt, BulletManager.BulletType bulletType, GameObject bulletPrefab = null, float bulletInterval = 0.0f, float bulletSpeed = 0.0f)
    {
        base.setup(speed, health, bulletType, bulletPrefab, lookAt, bulletInterval, bulletSpeed);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        base.OnETriggerEnter(col);
    }
}
