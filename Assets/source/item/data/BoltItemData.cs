using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltItemData : BaseItemData {
    private float m_bonusBoost;

    public void init(ItemManager.ITEM_TYPE _type, float _startTime, float _duration, float _bonusBoost)
    {
        base.init(_type, _duration);
        m_bonusBoost = _bonusBoost;
    }

    public override void update()
    {
        base.update();
    }

    public float getBonusBoost()
    {
        return m_bonusBoost;
    }
}
