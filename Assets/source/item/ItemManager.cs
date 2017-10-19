using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemDictionary : DictionaryTemplate<ItemManager.ITEM_TYPE, ItemDistrubution> { }

[System.Serializable]
public class ItemDistrubution
{
    public GameObject m_prefab;
    public float m_weight;
}

public class ItemManager : MonoBehaviour {

	public enum ITEM_TYPE {
        ITEM_NONE,
		ITEM_ONE_UP,
		ITEM_INVINCIBLE,
		ITEM_LIFE,
		ITEM_D, // Spiral Bullet
        ITEM_C, // Shockwave
        ITEM_S, // 4 bullets
        ITEM_A, // increase bullet speed
		ITEM_COUNT,
	}
    
	private static ItemManager m_instance;
    public List<ItemDictionary> m_itemPrefabList;
    private List<BaseItemData> 	m_ownedItemList;
    private float m_totalWeight;

	public static ItemManager getInstance()
	{
		return m_instance;
	}

	void Awake() {
		m_instance = this;
	}

	// Use this for initialization
	void Start () {
		m_ownedItemList = new List<BaseItemData> ();
        m_totalWeight = calculateTotalWeight();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.getInstance ().isGamePaused ())
			return;

        updateItemData();
	}

    void updateItemData()
    {
        // update itemdata of specific items only
        foreach (BaseItemData itemData in m_ownedItemList)
        {
            switch (itemData.getItemType())
            {
                case ITEM_TYPE.ITEM_D:
                    itemData.update();
                    break;
                case ITEM_TYPE.ITEM_INVINCIBLE:
                    (itemData as InvincibleItemData).update();
                    break;
            }
        }
    }
    /*
	 * Manager functions 
	 */

    private GameObject getItemPrefab(ITEM_TYPE itemType)
    {
        var obj = m_itemPrefabList.Find(item => item._key == itemType);
        if (obj != null)
            return obj._value.m_prefab;
        return null;
    }

    public void generateItem(ITEM_TYPE itemType = ITEM_TYPE.ITEM_NONE)
	{
		if (m_itemPrefabList.Count > 0) {
            // generate random item
            int index = (int) itemType;

            if(itemType == ITEM_TYPE.ITEM_NONE)
               index = getRandomIndex();

			bool spawn = true;
			int count = 0;

			while (spawn) {
				if (count > 10)
					spawn = false;
                float x = Random.Range(LevelManager.getInstance().getMinX(), LevelManager.getInstance().getMaxX());
                float y = Random.Range(LevelManager.getInstance().getMinY(), LevelManager.getInstance().getMaxY());
                Vector3 worldPos = new Vector3(x, y, 1.0f);
				BoxCollider2D boxCollider = getItemPrefab((ITEM_TYPE) index).GetComponent<BoxCollider2D> ();
				if (boxCollider != null) {
					Vector2 topLeft = new Vector2 (worldPos.x - boxCollider.size.x / 2, worldPos.y - boxCollider.size.y / 2);
					Vector2 bottomRight = new Vector2 (worldPos.x + boxCollider.size.x / 2, worldPos.y + boxCollider.size.y / 2);
					if (Physics2D.OverlapArea (topLeft, bottomRight)) {
						count++;
						continue;
					}
				} else {
					if (Physics2D.OverlapPoint (new Vector2 (worldPos.x, worldPos.y))) {
						count++;
						continue;
					}
				}
				Instantiate (getItemPrefab((ITEM_TYPE)index), worldPos, Quaternion.identity);
				spawn = false;
				CustomDebug.Log ("Spawned Item : " + (ITEM_TYPE)index);
			}
		}
	}

    public void generateItem(Vector3 pos, ITEM_TYPE type)
    {
        int itemIndex = (type == ITEM_TYPE.ITEM_NONE) ? getRandomIndex() : (int)type;
        Instantiate(m_itemPrefabList[(int)type]._value.m_prefab, pos, Quaternion.identity);
        CustomDebug.Log("Spawned Item : " + type);
    }

    public void generateSpecialItems()
    {
        for (int i = 0; i < PlayerDefs.CONST_SPECIAL_ITEM_SPAWN_COUNT; i++)
        {
            generateItem(ITEM_TYPE.ITEM_ONE_UP);
        }
    }

    public void addItem(ItemBase itemObj)
    {
        if (itemObj != null)
        {
            switch (itemObj.getItemType())
            {
                case ITEM_TYPE.ITEM_ONE_UP:
                    {
                        CandyItem candyItem = (CandyItem)itemObj;
                        CustomDebug.Log("Candy Added : " + candyItem.m_count);
                        //ItemDefs.addItem(ItemDefs.ItemType.ITEM_ID_CANDY, candyItem.m_count);
                        GameStats.SOULS += candyItem.m_count;
                    }
                    break;
                case ITEM_TYPE.ITEM_D:
                    {
                        BaseItemData itemData = new BaseItemData();
                        itemData.init(ITEM_TYPE.ITEM_D);

                        //remove S power
                        if (hasItemActive(ITEM_TYPE.ITEM_S))
                            removeItem(ITEM_TYPE.ITEM_S);

                        if (!hasItemActive(ITEM_TYPE.ITEM_D))
                            m_ownedItemList.Add(itemData);
                    }
                    break;
                case ITEM_TYPE.ITEM_INVINCIBLE:
                    {
                        InvincibleItem invincibleItem = (InvincibleItem) itemObj;
                        InvincibleItemData invincibleItemData = new InvincibleItemData();
                        invincibleItemData.init(ITEM_TYPE.ITEM_INVINCIBLE, Time.time, invincibleItem.m_activeDuration);
                        m_ownedItemList.Add(invincibleItemData);
                        GameManager.getInstance().m_player.onCollectedInvincible();
                    }
                    break;
                case ITEM_TYPE.ITEM_LIFE:
                    {
                        LifeItem lifeItem = (LifeItem)itemObj;
                        GameManager.getInstance().m_player.collectedLife(lifeItem.m_life);
                        CustomDebug.Log("Collected Life : "+ lifeItem.m_life);
                    }
                    break;
                case ITEM_TYPE.ITEM_C:
                    {
                        EnemyManager.getInstance().onShockwaveCollected((itemObj as ShockwaveItem).m_damage);
                    }
                    break;
                case ITEM_TYPE.ITEM_A:
                    {
                        BulletSpeedItem speedItem = (BulletSpeedItem)itemObj;
                        GameManager.getInstance().m_player.collectedSpeedBullet(speedItem.m_speedFactor);
                    }
                    break;
                case ITEM_TYPE.ITEM_S:
                    {
                        BaseItemData itemData = new BaseItemData();
                        itemData.init(ITEM_TYPE.ITEM_S);

                        // if has D power, then remove it
                        if (hasItemActive(ITEM_TYPE.ITEM_D))
                            removeItem(ITEM_TYPE.ITEM_D);

                        if(!hasItemActive(ITEM_TYPE.ITEM_S))
                            m_ownedItemList.Add(itemData);
                    }
                    break;
            }
        }
    }

    public void removeItem(BaseItemData itemData) {
        onRemovedItem(itemData.getItemType());
        m_ownedItemList.Remove(m_ownedItemList.Find(item => (item.getItemType() == itemData.getItemType())));
	}

    public void removeItem(ITEM_TYPE itemType) {
        onRemovedItem(itemType);
        m_ownedItemList.Remove(m_ownedItemList.Find(item => (item.getItemType() == itemType)));
    }

    private void onRemovedItem(ITEM_TYPE type)
    {
        switch (type)
        {
            case ITEM_TYPE.ITEM_INVINCIBLE:
                GameManager.getInstance().m_player.onRemovedInvincible();
                break;
        }
    }

	public void collectedCandy(int count) {
        //ItemDefs.addItem(ItemDefs.ItemType.ITEM_ID_CANDY, count);
        GameStats.SOULS += count;
	}

    public void usedCandy(int count = 1) {
        //ItemDefs.removeItem(ItemDefs.ItemType.ITEM_ID_CANDY, count);
        GameStats.SOULS -= count;
    }
	/*
	 * Util Functions
	 */

	public List<BaseItemData> getOwnedItemList() {
		return m_ownedItemList;
	}

	public bool hasItemActive(ITEM_TYPE itemType) {
		foreach (BaseItemData item in m_ownedItemList) {
			if (item.getItemType () == itemType)
				return true;
		}
		return false;
	}

	public BaseItemData getItemOfType(ITEM_TYPE itemType) {
		foreach (BaseItemData item in m_ownedItemList) {
			if (item.getItemType () == itemType)
				return item;
		}
		return null;
	}

    public void onDeactivateItem(BaseItemData _data)
    {
        StartCoroutine(removeItemOnEndFrame(_data));
    }

    IEnumerator removeItemOnEndFrame(BaseItemData _data)
    {
        yield return new WaitForEndOfFrame();
        removeItem(_data);
    }

    public void resetItemManager()
    {
        m_ownedItemList.Clear();
    }

    // calculate total weight of items
    private float calculateTotalWeight()
    {
        float totalWeight = 0.0f;
        foreach (var item in m_itemPrefabList)
        {
            totalWeight += item._value.m_weight;
        }

        return totalWeight;
    }

    // pick random item based on weight assigned
    private int getRandomIndex()
    {
        if (m_itemPrefabList.Count == 0)
            return -1;

        int selected = 0;
        float random = UnityEngine.Random.Range(0, m_totalWeight);

        for (int i = 0; i < m_itemPrefabList.Count; i++)
        {
            random -= m_itemPrefabList[i]._value.m_weight;
            if (random < 0.0f)
            {
                selected = (int)m_itemPrefabList[i]._key;
                break;
            }
        }
        return selected;
    }
}
