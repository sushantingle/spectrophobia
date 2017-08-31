using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemData {
    protected bool m_isActive;
    protected ItemManager.ITEM_TYPE m_type;

    public void init(ItemManager.ITEM_TYPE _type)
    {
        m_isActive = true;
        m_type = _type;
        CustomDebug.Log("Activated : " + m_type);
    }

    public virtual void update()
    {

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
