using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpeedItem : ItemBase {

    protected override void Start()
    {
        base.Start();
        m_type = ItemManager.ITEM_TYPE.ITEM_BULLET_SPEED;
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
