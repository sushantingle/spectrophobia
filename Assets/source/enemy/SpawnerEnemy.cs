using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnerEnemy : EnemyBase {

    public EnemyManager.ENEMY_TYPE m_spawnType = EnemyManager.ENEMY_TYPE.ENEMY_LINEAR;

    public enum SpawnCondition {
        NONE,
        SPAWN_ON_HIT,
        SPAWN_ON_DEATH,
        SPAWN_PERIODICALLY,
        SPAWN_WHEN_POPOUT,
    }

    public float m_spawnTimeInterval;
    public SpawnCondition m_spawnCondition;
    public int m_spawnCount;
    
    // random path enemy time offset
    public float m_changeTimeOffset;

    // runaway enemy
    public float m_safeDistance;

    private float m_startTime;
    private float m_spawnOffset = 2.0f;
    private float m_lastSpawnTime = 0;

    // childVaribles variables
    public float m_childSpeed;
    public float m_childLookAt;
    public float m_childHealth;
    public float m_childRaycastOffset;
    public BulletManager.BulletType m_childBulletType;
    public GameObject m_childBulletPrefab;
    public float m_childBulletInterval;
    public float m_childBulletSpeed;
    public float m_childChangeTimeOffset;
    public float m_childSafeDistance;

    // Rabbit path variables
    public float m_popOutInterval;
    public float m_stayOnDuration;
    private bool m_hasPoppedOut = false;
    private float m_lastPoppedOutTime = 0;

    // Use this for initialization
    void Start() {
        base.EStart();
        
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EStart();
        m_startTime = Time.time;
        m_fireStartTime = Time.time;
    }

    public override void setup(Transform lookAt)
    {
        m_lookAt = lookAt;
        if (m_pathType == PathDefs.AI_PATH_TYPE.PATH_LINEAR)
            m_path.init(transform, m_speed);
        else if (m_pathType == PathDefs.AI_PATH_TYPE.PATH_FOLLOW)
            m_path.init(transform, m_speed, m_lookAt);
        else if (m_pathType == PathDefs.AI_PATH_TYPE.PATH_RANDOM)
            m_path.init(transform, m_speed, m_changeTimeOffset);
        else if (m_pathType == PathDefs.AI_PATH_TYPE.PATH_RUNAWAY)
            m_path.init(transform, m_speed, m_lookAt, m_changeTimeOffset, m_safeDistance);
        else if (m_pathType == PathDefs.AI_PATH_TYPE.PATH_RABBIT)
        {
            m_path.init(transform, m_popOutInterval, m_stayOnDuration);
            enableRenderer(false);
            enableBoxCollider(false);
        }
    }

    protected override void EUpdate()
    {
        base.EUpdate();
        // Update path
        m_path.update();

        // update children
        if (m_spawnCondition == SpawnCondition.SPAWN_PERIODICALLY)
        {
            if (Time.time - m_startTime > m_spawnTimeInterval)
            {
                spawnChildren();
                m_startTime = Time.time;
            }
        }

        // Rabbit 
        if (m_pathType == PathDefs.AI_PATH_TYPE.PATH_RABBIT)
        {
            if (m_hasPoppedOut && (m_lastPoppedOutTime + m_stayOnDuration) < Time.time)
            {
                m_hasPoppedOut = false;
                onRabbitHide();
            }
        }
    }

    protected override void EFixedUpdate()
    {
        base.EFixedUpdate();
        m_path.fixedUpdate();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnETriggerEnter(collision);

        if (collision.gameObject.layer == LayerMask.NameToLayer("playerbullet") && m_spawnCondition == SpawnCondition.SPAWN_ON_HIT)
        {
            spawnChildren();
        }
        else if (m_spawnCondition == SpawnCondition.SPAWN_ON_DEATH)
        {
            if (m_health <= 0.0f)
                spawnChildren();
        }

    }

    private void spawnChildren()
    {
        if (Time.time - m_lastSpawnTime < m_spawnOffset)
            return;

        m_lastSpawnTime = Time.time;

        float leftRange = isHitStatusSet(PathDefs.AI_Direction.MOVE_LEFT) ? 0.0f : -1.0f;
        float rightRange = isHitStatusSet(PathDefs.AI_Direction.MOVE_RIGHT) ? 0.0f : 1.0f;
        float upRange = isHitStatusSet(PathDefs.AI_Direction.MOVE_UP) ? 0.0f : 1.0f;
        float downRange = isHitStatusSet(PathDefs.AI_Direction.MOVE_DOWN) ? 0.0f : -1.0f;

        for (int i = 0; i < m_spawnCount; i++)
        {
            float xDir = Random.Range(leftRange, rightRange);
            float yDir = Random.Range(upRange, downRange);

            float spawnDist = Random.Range(1.0f, 2.0f);

            if (GameManager.getInstance().isSinglePlayer())
            {
                GameObject prefab = EnemyManager.getInstance().getEnemyPrefab(m_spawnType);
                GameObject child = (GameObject)ObjectPool.Spawn(prefab, transform.position + (new Vector3(xDir, yDir, 0.0f) * spawnDist), Quaternion.identity);
                setupChild(m_spawnType, child, GameManager.getInstance().m_player.transform);
                EnemyManager.getInstance().onCustomEnemySpawned(child);
            }
#if ENABLE_MULTIPLAYER
            else if (GameManager.getInstance().isMultiplayer())
            {
                NetworkInstanceId parentNetId = gameObject.GetComponent<NetworkIdentity>().netId;
                NetworkInstanceId targetNetId = EnemyManager.getInstance().getEnemyTarget(m_spawnType).GetComponent<NetworkIdentity>().netId;
                Vector3 pos = transform.position + (new Vector3(xDir, yDir, 0.0f) * spawnDist);
                EnemyManager.getInstance().getNetworkEnemyManager().Cmd_spawnSpawnerChild(parentNetId, m_spawnType, pos, targetNetId);
            }
#endif
        }
    }

    public void CommandSpawnChild(EnemyManager.ENEMY_TYPE type, Vector3 position, NetworkInstanceId netId)
    {
        GameObject targetObj = NetworkServer.FindLocalObject(netId);
        GameObject prefab = EnemyManager.getInstance().getEnemyPrefab(m_spawnType);
        GameObject obj = (GameObject)GameManager.getInstance().getNetworkPool().Spawn(prefab, position, Quaternion.identity);
        obj.GetComponent<EnemyBase>().Team = m_team;
        obj.GetComponent<EnemyBase>().m_playerInstanceId = netId;
        obj.GetComponent<EnemyBase>().m_parentInstanceId = EnemyManager.getInstance().getNetworkEnemyManager().GetComponent<NetworkIdentity>().netId;
        setupChild(type, obj, targetObj.transform);
        //NetworkServer.Spawn(obj);
    }

    private void setupChild(EnemyManager.ENEMY_TYPE type, GameObject child, Transform target)
    {
        CustomDebug.Log("Child Speed : " + m_childSpeed);
        CustomDebug.Log("child Bullet Type : " + m_childBulletType);
        switch (type)
        {
            case EnemyManager.ENEMY_TYPE.ENEMY_LINEAR:
                child.GetComponent<EnemyBase>().setup(m_childSpeed, m_childHealth, m_childBulletType);
                break;
            case EnemyManager.ENEMY_TYPE.ENEMY_FOLLOWER:
                child.GetComponent<FollowerEnemy>().setup(m_childSpeed, m_childHealth, target, m_childBulletType);
                break;
            case EnemyManager.ENEMY_TYPE.ENEMY_LINEAR_SHOOTER:
                child.GetComponent<EnemyBase>().setup(m_childSpeed, m_childHealth, m_childBulletType, m_childBulletPrefab, target, m_childBulletInterval, m_childBulletSpeed);
                break;
            case EnemyManager.ENEMY_TYPE.ENEMY_FOLLOWER_SHOOTER:
                child.GetComponent<FollowerEnemy>().setup(m_childSpeed, m_childHealth, target, m_childBulletType,m_safeDistance, m_childBulletPrefab, m_childBulletInterval, m_childBulletSpeed);
                break;
            case EnemyManager.ENEMY_TYPE.ENEMY_RANDOM:
                child.GetComponent<RandomPathEnemy>().setup(m_childSpeed, m_childHealth, m_childChangeTimeOffset, m_childBulletType);
                break;
            case EnemyManager.ENEMY_TYPE.ENEMY_RANDOM_SHOOTER:
                child.GetComponent<RandomPathEnemy>().setup(m_childSpeed, m_childHealth, m_childChangeTimeOffset, m_childBulletType, m_childBulletPrefab, m_childBulletInterval, m_childBulletSpeed);
                break;
            case EnemyManager.ENEMY_TYPE.ENEMY_RUNAWAY:
                child.GetComponent<RunawayEnemy>().setup(m_childSpeed, m_childHealth, m_childChangeTimeOffset, m_childSafeDistance, m_childBulletType, m_childBulletPrefab, target, m_childBulletInterval, m_childBulletSpeed);
                break;

        }

    }

    public void onPopoutOfGround()
    {
        //CustomDebug.Log("On Rabbit Popped out of ground");
        StartCoroutine(invokeOnPopOutOfGround(1.0f)); // considering 1 sec delay of popping out animation
        m_hasPoppedOut = true;
        m_lastPoppedOutTime = Time.time;

        enableRenderer(true);
        enableBoxCollider(true);
    }

    IEnumerator invokeOnPopOutOfGround(float time)
    {
        yield return new WaitForSeconds(time);

        if (m_spawnCondition == SpawnCondition.SPAWN_WHEN_POPOUT)
        {
            spawnChildren();
        }

        if (m_bulletType != BulletManager.BulletType.BULLET_NONE)
        {
            updateBullet();
        }
    }

    private void onRabbitHide()
    {
        //CustomDebug.Log("On Rabbit Hide ");
        StartCoroutine(invokeHideRabbit(1.0f)); // animation delay
    }

    IEnumerator invokeHideRabbit(float time)
    {
        yield return new WaitForSeconds(time);

        RabbitPath path = (RabbitPath)m_path;
        path.onHide();

        enableRenderer(false);
        enableBoxCollider(false);
    }

    private void enableRenderer(bool value)
    {
        //CustomDebug.Log("Enable Renderer : " + value);
        //GetComponent<SpriteRenderer>().enabled = value;
    }

    private void enableBoxCollider(bool value)
    {
        //CustomDebug.Log("Enable Box Collider : " + value);
        //GetComponent<PolygonCollider2D>().enabled = value;
    }
}
