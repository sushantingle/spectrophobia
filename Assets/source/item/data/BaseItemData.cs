using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemData {
    protected bool m_isActive;
    protected ItemManager.ITEM_TYPE m_type;
    public float m_duration = -1;
    private float m_startTime;

    public void init(ItemManager.ITEM_TYPE _type, float duration = -1)
    {
        m_startTime = Time.time;
        m_isActive = true;
        m_type = _type;
        m_duration = duration;
        CustomDebug.Log("Activated : " + m_type);
    }

    public virtual void update()
    {
        if (m_duration >= 0.0f)
        {
            if (m_startTime + m_duration < Time.time)
                onDeactivate();

            if (GameManager.getInstance().m_player != null)
            {
                GameManager.getInstance().m_player.updatePowerBar(1 - ((Time.time - m_startTime) / m_duration));
            }
        }

    }

    public bool isActive()
    {
        return m_isActive;
    }

    public ItemManager.ITEM_TYPE getItemType()
    {
        return m_type;
    }

    public virtual void onDeactivate()
    {
        m_isActive = false;
        ItemManager.getInstance().onDeactivateItem(this);
        CustomDebug.Log("Deactivated : " + m_type);
    }
}
