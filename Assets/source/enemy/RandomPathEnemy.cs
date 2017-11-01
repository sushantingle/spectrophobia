using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPathEnemy : EnemyBase {

    public float m_changeTimeOffset = 0.2f;
    public float m_bonusSpeed = 1.0f;
    public float m_bonusSpeedDuration = 0.0f;
    private float m_bonusSpeedStartTime;
    private float m_originalSpeed;

    private void Awake()
    {
        m_pathType = PathDefs.AI_PATH_TYPE.PATH_RANDOM;
    }

    // Use this for initialization
    void Start() {
        //base.EStart();
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EStart();
        m_bonusSpeedStartTime = Time.time;
        m_originalSpeed = m_speed;
    }

    private void OnDisable()
    {
        m_speed = m_originalSpeed;
    }
    protected override void EUpdate()
    {
        base.EUpdate();
        m_path.update();

        if (m_isBoss)
        {
            if (Time.time - m_bonusSpeedStartTime > m_bonusSpeedDuration)
            {
                base.changeSpeed(m_originalSpeed);
            }
        }
    }

    protected override void EFixedUpdate()
    {
        base.EFixedUpdate();
        m_path.fixedUpdate();
    }

    public void setup(float speed, float health, float changeTimeOffset, BulletManager.BulletType bulletType, GameObject prefab = null, Transform target = null, float bulletInterval = 0.0f, float bulletSpeed = 0.0f)
    {
        m_changeTimeOffset = changeTimeOffset;
        base.setup(speed, health, bulletType, prefab, target, bulletInterval, bulletSpeed);
        m_path.init(transform, m_speed, m_changeTimeOffset);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnETriggerEnter(collider);
        if (m_isBoss)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("playerbullet"))
            {
                base.changeSpeed(m_originalSpeed + m_bonusSpeed);
                m_bonusSpeedStartTime = Time.time;
            }
        }
    }
}
