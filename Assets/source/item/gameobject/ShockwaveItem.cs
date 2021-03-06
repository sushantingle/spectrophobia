﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveItem : ItemBase {
    public float m_damage = 1.0f;
    public GameObject m_anim;

    protected override void Start()
    {
        base.Start();
        m_type = ItemManager.ITEM_TYPE.ITEM_C;
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
