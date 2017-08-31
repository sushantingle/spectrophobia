using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleItemData : BaseItemData {
    private float m_startTime;
    private float m_duration;

    public void init(ItemManager.ITEM_TYPE _type, float _startTime, float _duration)
    {
        base.init(_type);
        m_startTime = _startTime;
        m_duration = _duration;
    }

    public override void update()
    {
        base.update();

        if (m_startTime + m_duration < Time.time)
            onDeactivate();
    }
}
