using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDefs {

    public enum ItemType{
        ITEM_ID_CANDY = 0,
        ITEM_COUNT,
    }

    private static Dictionary<ItemType, int> m_items = new Dictionary<ItemType, int>();

    public static void setItemCount(ItemType itemId, int count)
    {
        if (itemId < 0 || itemId >= ItemType.ITEM_COUNT)
            return;

        if (m_items.ContainsKey(itemId))
            m_items[itemId] = count;
        else
            m_items.Add(itemId, count);

        CustomDebug.Log("Item : " + itemId + "  count : " + ItemDefs.getItemCount(itemId));
    }

    public static void addItem(ItemType itemId, int count)
    {
        int value = m_items.ContainsKey(itemId) ? m_items[itemId] : 0;
        if (m_items.ContainsKey(itemId))
            m_items[itemId] = value + count;
        else
            m_items.Add(itemId, value + count);

        CustomDebug.Log("Item : " + itemId + "  count : " + ItemDefs.getItemCount(itemId));
    }

    public static void removeItem(ItemType itemId, int count)
    {
        if (m_items.ContainsKey(itemId))
        {
            int value = 0;
            m_items.TryGetValue(itemId, out value);
            value -= count;
            if (value < 0)
                value = 0;

            m_items[itemId] = value;

            CustomDebug.Log("Item : " + itemId + "  count : " + ItemDefs.getItemCount(itemId));
        }
    }

    public static int getItemCount(ItemType itemId)
    {
        int value = m_items.ContainsKey(itemId) ? m_items[itemId] : 0;
        return value;
    }
}
