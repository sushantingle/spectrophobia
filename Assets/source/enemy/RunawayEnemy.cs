using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayEnemy : EnemyBase {

    public float m_changeTimeOffset = 1.0f;
    public float m_safeDistance = 3.0f;

    private void Awake()
    {
        m_pathType = PathDefs.AI_PATH_TYPE.PATH_RUNAWAY;
    }

    private void Start()
    {
     //   EStart();
     
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EStart();
    }

    public void setup(float speed, float health, float changeTimeOffset, float safeDistance, BulletManager.BulletType bulletType, GameObject prefab = null, Transform lookAt = null, float bulletInterval = 0.0f, float bulletSpeed = 0.0f)
    {
        CustomDebug.Log("Runaway Speed : " + m_speed);
        m_changeTimeOffset = changeTimeOffset;
        m_safeDistance = safeDistance;
        base.setup(speed, health, bulletType, prefab, lookAt, bulletInterval, bulletSpeed);
        m_path.init(transform, m_speed, m_lookAt, m_changeTimeOffset, m_safeDistance);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnETriggerEnter(collision);
    }
}
