using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPathEnemy : EnemyBase {

    public float m_changeTimeOffset = 0.2f;

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
    }

    protected override void EUpdate()
    {
        base.EUpdate();
        m_path.update();
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
    }
}
