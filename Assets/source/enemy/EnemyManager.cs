using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[System.Serializable]
public class EnemyDictionary : DictionaryTemplate<EnemyManager.ENEMY_TYPE, GameObject> { }

[System.Serializable]
public class BossDictionary : DictionaryTemplate<EnemyManager.BOSS_TYPE, GameObject> { }

[System.Serializable]
public class IndicatorDictionary : DictionaryTemplate<Player.Player_Team, Material> { }

public class EnemyManager : NetworkBehaviour{
	public enum ENEMY_TYPE {
		ENEMY_LINEAR = 0,
		ENEMY_FOLLOWER,
		ENEMY_LINEAR_SHOOTER,
		ENEMY_FOLLOWER_SHOOTER,
        ENEMY_RANDOM,
        ENEMY_RANDOM_SHOOTER,
        ENEMY_SPAWNER_LINEAR_ON_HIT,
        ENEMY_RUNAWAY,
		ENEMY_COUNT,
	}

	public enum BOSS_TYPE {
		BOSS_NORMAL = 0,
		BOSS_COUNT,
	}

	private static EnemyManager m_instance = null;
    
    public List<EnemyDictionary> m_enemyPrefabs; // TODO: use this dictionary and remove enemy prefab list
    
	public List<BossDictionary> m_bossPrefabs;
	public GameObject m_spawnAnim;
	public int m_spawnCount = 1;
	private Transform m_player = null;

	public float 		m_enemySpawnSpeed 	= 1.0f;
	private float 		m_lastSpawnTime 	= 0.0f;

	private bool 		m_isBossActive = false;
	private int 		m_deadEnemyCount = 0;
	private int 		m_spawnEnemyCount = 0;
    private int         m_totalEnemyDied = 0;
    public List<GameObject> m_spawnedEnemyList;
    public int          m_preloadCount = 30;

    private NetworkEnemyManager m_networkEnemyManager = null;
    public List<IndicatorDictionary> m_teamIndicatorMaterial;

    public static EnemyManager getInstance() {
		return m_instance;
	}

    public NetworkEnemyManager getNetworkEnemyManager()
    {
        return m_networkEnemyManager;
    }
    
	void Awake() {
		m_instance = this;

        // Preload enemy
        foreach (var obj in m_enemyPrefabs)
        {
            ObjectPool.Preload(obj._value, m_preloadCount);
        }
	}

    public void setPlayer(Transform player)
    {
        m_player = player;
        m_networkEnemyManager = m_player.GetComponent<NetworkEnemyManager>();
    }

	// Use this for initialization
	void Start () {
        m_spawnedEnemyList = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.getInstance ().isGamePaused ())
			return;

		if (m_spawnEnemyCount < m_spawnCount) {
			if (Time.time - m_lastSpawnTime > m_enemySpawnSpeed) {
                CustomDebug.Log("Spawn Enemy");
				m_lastSpawnTime = Time.time;
				spawnEnemies ();
			}
		} else {
			if (!m_isBossActive) {
				m_isBossActive = true;
				spawnBoss ();
			}
		}

	}

	public void spawnEnemies()
	{
		for (int i = 0; i < m_spawnCount; i++) {

			int enemyId = Random.Range (0, 100);
            enemyId = enemyId % (int) m_enemyPrefabs.Count;

			bool spawn = true;
			int count = 0;

            while (spawn) {
				if (count > 10)
					spawn = false;
				float x = Random.Range (LevelManager.getInstance().getMinX(), LevelManager.getInstance().getMaxX());
				float y = Random.Range(LevelManager.getInstance().getMinY(), LevelManager.getInstance().getMaxY());
                Vector3 worldPos = new Vector3 (x, y, 1.0f);

                PolygonCollider2D polyCollider = getEnemyPrefab((ENEMY_TYPE)enemyId).GetComponent<PolygonCollider2D>();
                if (polyCollider.bounds.Contains(worldPos))
                {
                    count++;
                    continue;
                }

				/*BoxCollider2D boxCollider = getEnemyPrefab((ENEMY_TYPE)enemyId).GetComponent<BoxCollider2D> ();                
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
				}*/
				spawnEnemy ((ENEMY_TYPE)enemyId, worldPos, Quaternion.identity);
				spawn = false;
				m_spawnEnemyCount++;
			}

		
		}
	}

    public GameObject getEnemyPrefab(ENEMY_TYPE id)
    {
        var obj = m_enemyPrefabs.Find(item => item._key == id);
        if (obj != null)
            return obj._value;
        return null;
    }

    private GameObject getBossPrefab(BOSS_TYPE id)
    {
        var obj = m_bossPrefabs.Find(item => item._key == id);
        if (obj != null)
            return obj._value;
        return null;
    }

	public void spawnEnemy(ENEMY_TYPE enemyType, Vector3 position, Quaternion rotation)
	{
        GameObject e = null;
        if (GameManager.getInstance().isMultiplayer())
        {
            GameObject target = getEnemyTarget(enemyType);
            NetworkInstanceId netId = target.GetComponent<NetworkIdentity>().netId;
            NetworkInstanceId parentNetId = m_networkEnemyManager.GetComponent<NetworkIdentity>().netId;
            m_networkEnemyManager.Cmd_SpawnEnemy(enemyType, ClanManager.getInstance().SelectedTeam, position, netId, parentNetId);
        }
        else if (GameManager.getInstance().isSinglePlayer())
        {
            e = (GameObject)ObjectPool.Spawn(getEnemyPrefab(enemyType), position, rotation);
            m_spawnedEnemyList.Add(e);
            setupEnemy(enemyType, e);
        }
        CustomDebug.Log("Spawned Enemy : " + enemyType);
   }

    public GameObject getEnemyTarget(ENEMY_TYPE enemyType)
    {
        // TODO : for now, target is current player for all enemy types
        return GameManager.getInstance().m_player.gameObject;
    }

    public GameObject getEnemyTarget(BOSS_TYPE type)
    {
        // TODO: For now target is current player
        return GameManager.getInstance().m_player.gameObject;
    }

    private void setupEnemy(ENEMY_TYPE enemyType, GameObject e)
    {
        switch (enemyType)
        {
            case ENEMY_TYPE.ENEMY_LINEAR:
                //Setup
                e.GetComponent<LinearEnemy>().setup(m_player);
                break;
            case ENEMY_TYPE.ENEMY_FOLLOWER:
                e.GetComponent<FollowerEnemy>().setup(m_player);
                break;
            case ENEMY_TYPE.ENEMY_LINEAR_SHOOTER:
                e.GetComponent<LinearEnemy>().setup(m_player);
                break;
            case ENEMY_TYPE.ENEMY_FOLLOWER_SHOOTER:
                e.GetComponent<FollowerEnemy>().setup(m_player);
                break;
            case ENEMY_TYPE.ENEMY_RANDOM:
            case ENEMY_TYPE.ENEMY_RANDOM_SHOOTER:
                e.GetComponent<RandomPathEnemy>().setup(m_player);
                break;
            case ENEMY_TYPE.ENEMY_SPAWNER_LINEAR_ON_HIT:
                e.GetComponent<SpawnerEnemy>().setup(m_player);
                break;
            case ENEMY_TYPE.ENEMY_RUNAWAY:
                e.GetComponent<RunawayEnemy>().setup(m_player);
                break;
        }
    }

    public void onCustomEnemySpawned(GameObject obj)
    {
        m_spawnedEnemyList.Add(obj);
    }

	public void OnEnemyDeath(GameObject obj)
	{
        m_deadEnemyCount += 1;
        m_totalEnemyDied += 1;
        //obj.SetActive (false);
		CustomDebug.Log ("Death Count : " + m_deadEnemyCount);

        if (m_totalEnemyDied % PlayerDefs.CONST_ITEM_SPAWN_RATE == 0)
            ItemManager.getInstance().generateItem();
        
        GameManager.getInstance().addScore(obj.GetComponent<EnemyBase>().m_points);

        if (GameManager.getInstance().isSinglePlayer())
        {
            m_spawnedEnemyList.Remove(obj);
            ObjectPool.Despawn(obj);
        }
        else
        {
            CustomDebug.Log("Destroyed Enemy Object");
            m_networkEnemyManager.Cmd_destroyObject(obj.GetComponent<NetworkIdentity>().netId);
        }
    }

	public int getEnemyDeathCount()
	{
		return m_deadEnemyCount;
	}

	public void onBossDead(GameObject obj)
	{
        CustomDebug.Log("On Boss Dead");
		m_isBossActive = false;
		m_deadEnemyCount = 0;
		m_spawnEnemyCount = 0;
        m_totalEnemyDied += 1;
        if (GameManager.getInstance().isSinglePlayer())
        {
            m_spawnedEnemyList.Remove(obj);
            ObjectPool.Despawn(obj);
        }
        else
        {
            CustomDebug.Log("Destroying Boss object");
            //Destroy(obj);
            m_networkEnemyManager.Cmd_destroyObject(obj.GetComponent<NetworkIdentity>().netId);
        }
        GameManager.getInstance().incrementXP();
	}
	
    public List<GameObject> getSpawnedEnemyList() { return m_spawnedEnemyList; }

    public GameObject getNearestEnemy(Vector3 pos)
    {
        GameObject obj = null;
        float minDist = 100.0f;
        foreach (GameObject enemy in m_spawnedEnemyList)
        {
            float dist = Vector3.Distance(pos, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                obj = enemy;
            }
        }
        return obj;
    }
        
	public void spawnBoss()
	{
			int bossId = 0;
			//enemyId = enemyId % (int)ENEMY_TYPE.ENEMY_COUNT;

			bool spawn = true;
			int count = 0;

			while (spawn) {
				if (count > 10)
					spawn = false;
            float x = Random.Range(LevelManager.getInstance().getMinX(), LevelManager.getInstance().getMaxX());
            float y = Random.Range(LevelManager.getInstance().getMinY(), LevelManager.getInstance().getMaxY());
            Vector3 worldPos = new Vector3(x, y, 1.0f);
            PolygonCollider2D polyCol = getBossPrefab((BOSS_TYPE)bossId).GetComponent<PolygonCollider2D>();
            if (polyCol.bounds.Contains(worldPos))
            {
                count++;
                continue;
            }
            /*BoxCollider2D boxCollider = getBossPrefab((BOSS_TYPE)bossId).GetComponent<BoxCollider2D> ();
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
				}*/
            //spawnEnemy ((ENEMY_TYPE)enemyId, worldPos, Quaternion.identity);
                
                if (GameManager.getInstance().isMultiplayer())
                {
                    BOSS_TYPE type = (BOSS_TYPE)bossId;
                    GameObject target = getEnemyTarget(type);
                    NetworkInstanceId netId = target.GetComponent<NetworkIdentity>().netId;
                    NetworkInstanceId parentNetId = m_networkEnemyManager.GetComponent<NetworkIdentity>().netId;
                    m_networkEnemyManager.Cmd_SpawnBoss(type, ClanManager.getInstance().SelectedTeam, worldPos, netId, parentNetId);
                }
                else if (GameManager.getInstance().isSinglePlayer())
                {
                    GameObject e = (GameObject)ObjectPool.Spawn(getBossPrefab((BOSS_TYPE)bossId), worldPos, Quaternion.identity);
                    m_spawnedEnemyList.Add(e);
                }
                spawn = false;
                
        }
    }
    
    public void CommandSpawnEnemy(ENEMY_TYPE type,Player.Player_Team team, Vector3 pos, NetworkInstanceId netId, NetworkInstanceId parentNetId)
    {
        CustomDebug.Log("Command Spawn Enemy : " + netId.Value);
        GameObject obj = (GameObject)Instantiate(getEnemyPrefab(type), pos, Quaternion.identity);
        EnemyBase enemyBase = obj.GetComponent<EnemyBase>();
        enemyBase.Team = team;
        enemyBase.m_playerInstanceId = netId;
        enemyBase.m_parentInstanceId = parentNetId;
        //setupEnemy(type, obj);
        NetworkServer.Spawn(obj);
    }

    public void CommandSpawnBoss(BOSS_TYPE type, Player.Player_Team team, Vector3 pos, NetworkInstanceId netId, NetworkInstanceId parentNetId)
    {
        CustomDebug.Log("Instantiating boss");
        GameObject obj = (GameObject)Instantiate(getBossPrefab(type), pos, Quaternion.identity);
        EnemyBase enemyBase = obj.GetComponent<EnemyBase>();
        enemyBase.Team = team;
        enemyBase.m_playerInstanceId = netId;
        enemyBase.m_parentInstanceId = parentNetId;
        NetworkServer.Spawn(obj);
    }

    public void reset()
    {
        m_isBossActive = false;
        m_spawnEnemyCount = 0;
        m_deadEnemyCount = 0;

        /*Object[] objects = FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = (GameObject)objects[i];
            if (obj.layer == LayerMask.NameToLayer("enemy"))
                Destroy(obj);
        }*/

        if(GameManager.getInstance().isSinglePlayer())
        {
            foreach (GameObject obj in m_spawnedEnemyList)
            {
                obj.GetComponent<EnemyBase>().reset();
                ObjectPool.Despawn(obj);
            }
        }
        m_spawnedEnemyList.Clear();
        m_player = null;
    }

    public void onShockwaveCollected(float damage)
    {
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = (GameObject)objects[i];
            if (obj.layer == LayerMask.NameToLayer("enemy"))
            {
                EnemyBase enemyBase = obj.GetComponent<EnemyBase>();
                if (enemyBase != null)
                {
                    enemyBase.onExternalDamage(damage);
                }
            }
        }
    }

    public void onExplosion(Vector3 position, float range, float damage)
    {
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = (GameObject)objects[i];
            if (obj.layer == LayerMask.NameToLayer("enemy"))
            {
                EnemyBase enemyBase = obj.GetComponent<EnemyBase>();
                if (enemyBase != null && Vector3.Distance(position, enemyBase.transform.position) < range)
                {
                    enemyBase.onExternalDamage(damage);
                }
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CustomDebug.Log("EnemyManager OnStartClient");
        
    }

    public Material getMaterial(Player.Player_Team id)
    {
        var obj = m_teamIndicatorMaterial.Find(team => team._key == id);
        if (obj != null)
            return obj._value;
        return null;
    }
}
