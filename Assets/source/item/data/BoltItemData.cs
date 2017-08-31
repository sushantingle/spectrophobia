using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltItemData : BaseItemData {
    private float m_startTime;
    private float m_duration;
    private float m_bonusBoost;

    public void init(ItemManager.ITEM_TYPE _type, float _startTime, float _duration, float _bonusBoost)
    {
        base.init(_type);
        m_startTime = _startTime;
        m_duration = _duration;
        m_bonusBoost = _bonusBoost;
    }

    public override void update()
    {
        base.update();

        if (m_startTime + m_duration < Time.time)
            onDeactivate();
    }

    public float getBonusBoost()
    {
        return m_bonusBoost;
    }
}
