using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BrickWeightDictionary : DictionaryTemplate<GameObject, float> { }

public class LevelGenerator : MonoBehaviour {

    public List<BrickWeightDictionary> m_brickWeightDictionary;
    public int[] m_brickCountRange = new int[2];

    private List<int> m_brickIndexToBeSpawned = new List<int>();
    private List<GameObject> m_spawnedBrickList = new List<GameObject>();

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Random"))
        {
            generateLevel();
        }

        if (GUI.Button(new Rect(150, 0, 100, 100), "Clear"))
        {
            clear();
        }
    }

    private void clear()
    {
        foreach (GameObject obj in m_spawnedBrickList)
            Destroy(obj);

        m_spawnedBrickList.Clear();
        m_brickIndexToBeSpawned.Clear();
    }
    private int getRandomIndex()
    {
        if (m_brickWeightDictionary.Count == 0)
            return -1;
        float totalWeight = calculateTotalWeight();

        int selected = 0;
        float random = UnityEngine.Random.Range(0.0f, totalWeight);

        for (int i = 0; i < m_brickWeightDictionary.Count; i++)
        {
            random -= m_brickWeightDictionary[i]._value;
            if (random < 0.0f)
            {
                selected = i;
                break;
            }
        }
        CustomDebug.Log("Random Selected : " + selected);
        return selected;
    }

    private float calculateTotalWeight()
    {
        float totalWeight = 0;

        foreach (BrickWeightDictionary obj in m_brickWeightDictionary)
        {
            totalWeight += obj._value;
        }

        return totalWeight;
    }

    private bool spawnBrick(int index)
    {
        bool spawn = true;
        int count = 0;
        GameObject obj = (GameObject)Instantiate(m_brickWeightDictionary[index]._key, new Vector3(0,0,0), Quaternion.identity);
        while (spawn)
        {
            if (count > 10)
            {
                Destroy(obj);
                spawn = false;
            }
            float x = UnityEngine.Random.Range(LevelManager.getInstance().getMinX(), LevelManager.getInstance().getMaxX());
            float y = UnityEngine.Random.Range(LevelManager.getInstance().getMinY(), LevelManager.getInstance().getMaxY());
            Vector3 worldPos = new Vector3(x, y, 0.0f);
            obj.transform.position = worldPos;
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null)
            {
                if (isOverlappingWithWall(col))
                {
                    count++;
                    CustomDebug.Log("Overlapped : reset to zero");
                    obj.transform.position = Vector3.zero;
                    continue;
                }
            }
            else
            {
                if (Physics2D.OverlapPoint(new Vector2(worldPos.x, worldPos.y)))
                {
                    count++;
                    CustomDebug.Log("Overlapped : reset to zero");
                    obj.transform.position = Vector3.zero;
                    continue;
                }
            }

            CustomDebug.Log("Adding to Spawned list");
            m_spawnedBrickList.Add(obj);
            
            spawn = false;
            CustomDebug.Log("Spawned brick : " + index);
            return true;
        }
        return false;
    }

    public void generateLevel()
    {
        int brickCount = UnityEngine.Random.Range(m_brickCountRange[0], m_brickCountRange[1]);
        for (int i = 0; i < brickCount; i++)
        {
            m_brickIndexToBeSpawned.Add(getRandomIndex());
        }

        //spawn brick
        int count = 0;
        for (int i = 0; i < brickCount; i++)
        {
            if (spawnBrick(m_brickIndexToBeSpawned[i]))
                count++;
        }
        CustomDebug.Log("Spawned bricks : " + count);
    }

    bool isOverlappingWithWall(Collider2D col)
    {
        CustomDebug.Log("Collider : " + col.bounds+ "   Active: "+col.gameObject.activeSelf);

        for (int i = 0; i < m_spawnedBrickList.Count; i++)
        {
            Collider2D col_2 = m_spawnedBrickList[i].GetComponent<Collider2D>();
            if (col.bounds.Intersects(col_2.bounds))
            {
                CustomDebug.Log("Overlapping with Spawned : "+col_2.bounds);
                return true;
            }
        }

        CustomDebug.Log("Not Overlapping");
        return false;
    }
}
