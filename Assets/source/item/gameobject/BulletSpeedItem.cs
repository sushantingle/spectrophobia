using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpeedItem : ItemBase {

    public float m_speedFactor = 0.0f;

    protected override void Start()
    {
        base.Start();
        m_type = ItemManager.ITEM_TYPE.ITEM_A;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);
    }
}
