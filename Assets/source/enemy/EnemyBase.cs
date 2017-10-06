using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;

public class EnemyBase : NetworkBehaviour {

    public Player.Player_Team Team
    {
        get { return m_team; }
        set { m_team = value; }
    }
    [SyncVar(hook = "OnSetTeam")]
    public Player.Player_Team m_team = Player.Player_Team.TEAM_NONE;

    [SyncVar]
    public float m_speed = 0.1f;
    [SyncVar]
    public Transform m_lookAt = null;
    [SyncVar]
    protected float m_health = 5;

    public float m_raycastOffset = 0.5f;

    protected PolygonCollider2D m_boxCollider;
    protected Animator m_animator;

    //Bullet properties
    [SyncVar]
    public float m_bulletInterval = 1.0f;
    [SyncVar]
    public float m_bulletSpeed = 0.2f;
    
    // Linear N Bullet Type
    [SyncVar]
    public int m_bulletCount = 4;

    // Sine Bullet, Spiral Bullet
    [SyncVar]
    public float m_bulletAmplitude = 1.0f;

    // Projectile Bullet, Projectile Bullet 4
    [SyncVar]
    public float m_bulletDistance = 1.0f;
    [SyncVar]
    public float m_bulletMaxTime = 1.0f;
    [SyncVar]
    public float m_bulletGravity = 2.0f;
    [SyncVar]
    public float m_bulletProjectionAngle = 30.0f;

    protected float m_fireStartTime;
    protected bool m_isBoss = false;

    [HideInInspector]public PathDefs.AI_PATH_TYPE m_pathType = PathDefs.AI_PATH_TYPE.PATH_LINEAR;

    protected int m_hitStatus = 0;
    protected int m_moveStatus = 0;
    [SyncVar]
    public BulletManager.BulletType m_bulletType = BulletManager.BulletType.BULLET_NONE;

    protected PathBase m_path = null;
    [SyncVar (hook = "OnSetPlayerInstanceId")]
    public NetworkInstanceId m_playerInstanceId;
    
    // Special powers
    public enum SpecialPower
    {
        POWER_NONE,
        POWER_AUTO_RECOVERY,
        POWER_EXPLODE_ON_DEATH,
    }
    public SpecialPower m_specialPower;
    // Auto Recovery Power
    private float m_lastHitTime;
    public float m_recoveryRate;
    public float m_powerActivationDelay;
    public float m_maxHealth;

    // Explode on Death
    public float m_explosionRange;
    public float m_explosionDelay;
    public float m_explosionDamage;

    public int m_points = 0;
    [SyncVar (hook = "OnSetParentInstanceId")]
    public NetworkInstanceId m_parentInstanceId = NetworkInstanceId.Invalid;

    // Use this for initialization
    protected virtual void EStart() {
        
        // special power
        m_health = m_maxHealth;
    }

    protected virtual void OnEnable() {

        m_boxCollider = GetComponent<PolygonCollider2D>();
        m_animator = GetComponent<Animator>();
        createPath();
        m_health = m_maxHealth;
    }

    private void createPath()
    {
        CustomDebug.Log("Path Type : " + m_pathType);
        switch (m_pathType)
        {
            case PathDefs.AI_PATH_TYPE.PATH_LINEAR:
                m_path = new LinearPath();
                break;
            case PathDefs.AI_PATH_TYPE.PATH_FOLLOW:
                m_path = new FollowerPath();
                break;
            case PathDefs.AI_PATH_TYPE.PATH_RANDOM:
                m_path = new RandomPath();
                break;
            case PathDefs.AI_PATH_TYPE.PATH_RUNAWAY:
                m_path = new RunawayPath();
                break;
            case PathDefs.AI_PATH_TYPE.PATH_RABBIT:
                m_path = new RabbitPath();
                break;
        }
    }
    // Update is called once per frame
    protected virtual void Update() {
        if (GameManager.getInstance().isGamePaused())
            return;
        if (GameManager.getInstance().isMultiplayer() && !GameManager.getInstance().isServer())
            return;

        EUpdate();
    }

    protected virtual void EUpdate()
    {
        if (m_bulletType != BulletManager.BulletType.BULLET_NONE)
        {
            if(m_pathType != PathDefs.AI_PATH_TYPE.PATH_RABBIT)
                updateBullet();
        }

        updateSpecialPower();
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.getInstance().isGamePaused())
            return;

        EFixedUpdate();
    }

    protected virtual void EFixedUpdate()
    {
        if (GameManager.getInstance().isGamePaused())
            return;

        if (GameManager.getInstance().isMultiplayer() && !GameManager.getInstance().isServer())
            return;

        int layermask = 1 << LayerMask.NameToLayer("wall");
        //layermask = ~layermask;

        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, m_boxCollider.bounds.size.y / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.up * (m_boxCollider.bounds.size.y / 2 + m_raycastOffset), Color.green);
        if (hitUp.collider != null)
            setHitStatus(PathDefs.AI_Direction.MOVE_UP);
        else resetHitStatus(PathDefs.AI_Direction.MOVE_UP);

        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, m_boxCollider.bounds.size.y / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, -Vector2.up * (m_boxCollider.bounds.size.y / 2 + m_raycastOffset), Color.blue);
        if (hitDown.collider != null)
            setHitStatus(PathDefs.AI_Direction.MOVE_DOWN);
        else resetHitStatus(PathDefs.AI_Direction.MOVE_DOWN);

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, m_boxCollider.bounds.size.x / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.right * (m_boxCollider.bounds.size.x / 2 + m_raycastOffset), Color.grey);
        if (hitRight.collider != null)
            setHitStatus(PathDefs.AI_Direction.MOVE_RIGHT);
        else resetHitStatus(PathDefs.AI_Direction.MOVE_RIGHT);

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, m_boxCollider.bounds.size.x / 2 + m_raycastOffset, layermask);
        //Debug.DrawRay (transform.position, Vector2.left * (m_boxCollider.bounds.size.x / 2 + m_raycastOffset), Color.magenta);
        if (hitLeft.collider != null)
            setHitStatus(PathDefs.AI_Direction.MOVE_LEFT);
        else resetHitStatus(PathDefs.AI_Direction.MOVE_LEFT);
    }

    public virtual void setup(Transform lookAt)
    {
        CustomDebug.Log("Setup 1");
        m_lookAt = lookAt;
        m_path.init(transform, m_speed, lookAt);
    }

    public virtual void setup(float speed, float health, BulletManager.BulletType bulletType = BulletManager.BulletType.BULLET_NONE, GameObject prefab = null, Transform lookAt = null, float bulletInterval = 0.0f, float bulletSpeed = 0.0f)
    {
        CustomDebug.Log("Setup 2");
        CustomDebug.Log("Base Speed : " + speed);
        CustomDebug.Log("Bullet Type: " + bulletType);
        CustomDebug.Log("Bullet Prefab: " + prefab.name);
        
        m_speed = speed;
        m_health = health;
        m_bulletInterval = bulletInterval;
        m_bulletSpeed = bulletSpeed;
        m_bulletType = bulletType;
        m_lookAt = lookAt;
        m_path.init(transform, m_speed, m_lookAt);
    }

    protected virtual void updateBullet()
    {
        if (m_lookAt == null) // need to add this check because it takes time setup enemy
            return;
        int layer = LayerMask.NameToLayer("enemybullet");
        NetworkInstanceId parentNetId = GetComponent<NetworkIdentity>().netId;

        if (Time.time - m_fireStartTime > m_bulletInterval)
        {
            m_fireStartTime = Time.time;

            if (m_bulletType == BulletManager.BulletType.BULLET_LINEAR || m_bulletType == BulletManager.BulletType.BULLET_MISSILE)
            {
                BulletManager.getInstance().initBullet(m_bulletType, layer, transform, m_lookAt, m_bulletSpeed, parentNetId);
            }
            else if (m_bulletType == BulletManager.BulletType.BULLET_LINEAR_N || m_bulletType == BulletManager.BulletType.BULLET_LINEAR_N_PI)
            {
                BulletManager.getInstance().initBullet(m_bulletType, layer, transform, m_lookAt.position - transform.position, m_bulletSpeed, m_bulletCount, parentNetId);
            }
            else if (m_bulletType == BulletManager.BulletType.BULLET_PROJECTILE)
            {
                BulletManager.getInstance().initBullet(BulletManager.BulletType.BULLET_PROJECTILE, layer, transform, ProjectileBullet.Direction.DIRECTION_RIGHT, m_bulletDistance, m_bulletMaxTime, m_bulletGravity, m_bulletProjectionAngle, parentNetId);
            }
            else if (m_bulletType == BulletManager.BulletType.BULLET_PROJECTILE_4)
            {
                BulletManager.getInstance().initBullet(BulletManager.BulletType.BULLET_PROJECTILE_4, layer, transform, m_bulletDistance, m_bulletMaxTime, m_bulletGravity, m_bulletProjectionAngle, parentNetId);
            }
            else if (m_bulletType == BulletManager.BulletType.BULLET_SINE || m_bulletType == BulletManager.BulletType.BULLET_SPIRAL)
            {
                BulletManager.getInstance().initBullet(m_bulletType, layer, transform, m_lookAt.position - transform.position, m_bulletSpeed, m_bulletAmplitude, parentNetId);
            }
        }
    }

    protected virtual void OnETriggerEnter(Collider2D col)
    {
        
        if (col.gameObject.layer == LayerMask.NameToLayer("playerbullet"))
        {
            BulletBase bulletBase = col.gameObject.GetComponent<BulletBase>();
            if (bulletBase)
            {
                if (bulletBase.m_type == BulletManager.BulletType.BULLET_MISSILE)
                {
                    if ((bulletBase as MissileBullet).getTarget() != transform)
                        return;
                }
            }

            //Destroy(col.gameObject);
            BulletManager.getInstance().onDestroyBullet(col.gameObject);

            if (GameManager.getInstance().isMultiplayer() && isCustomLocalPlayer(col.gameObject) == false)
            {
                CustomDebug.Log("Enemy Base Not a local player");
                return;
            }

            m_health--;
            if (m_health <= 0)
            {
                if (m_specialPower == SpecialPower.POWER_EXPLODE_ON_DEATH)
                    StartCoroutine(invokeOnExplosion(m_explosionDelay));
                else
                    onDeath();
            }

            // special Power
            if (m_specialPower == SpecialPower.POWER_AUTO_RECOVERY)
            {
                m_lastHitTime = Time.time;
            }
        }
    }

    private void onDeath()
    {
        CustomDebug.Log("OnDeath");
        if (GameManager.getInstance().isSinglePlayer())
        {
            reset();
            if (m_isBoss)
                EnemyManager.getInstance().onBossDead(gameObject);
            else
                EnemyManager.getInstance().OnEnemyDeath(gameObject);
        }
        else
        {
            //commandOnDeath();
            if (m_isBoss)
                EnemyManager.getInstance().onBossDead(gameObject);
            else
                EnemyManager.getInstance().OnEnemyDeath(gameObject);
        }
    }

    public void commandOnDeath()
    {
        CustomDebug.Log("Command On Death");
        reset();
        GameManager.getInstance().m_player.GetComponent<NetworkEnemyManager>().Cmd_onDeath(netId, m_isBoss);
    }

    private bool isCustomLocalPlayer(GameObject bullet = null)
    {
        if (GameManager.getInstance().isMultiplayer())
        {
            //return GameManager.getInstance().isServer();

            if (m_parentInstanceId == NetworkInstanceId.Invalid)
            {
                CustomDebug.Log("Enemy parent net id is invalid");
                return false;
            }

            GameObject enemyParent = null;
            if (GameManager.getInstance().isServer())
                enemyParent = NetworkServer.FindLocalObject(m_parentInstanceId);
            else
                enemyParent = ClientScene.FindLocalObject(m_parentInstanceId);

            if (enemyParent == null)
            {
                CustomDebug.Log("Enemy Parent is null");
                return false;
            }

            if (enemyParent.GetComponent<Player>() != null && enemyParent.GetComponent<Player>().isLocalPlayer == false)
            {
                CustomDebug.Log("Enemy Parent is not local player");
                return false;
            }

            if (enemyParent.GetComponent<EnemyBase>() != null)
            {
                CustomDebug.Log("Bullet Parent is Enemy");
                EnemyBase enemyObj = enemyParent.GetComponent<EnemyBase>();
                if (enemyObj.getParentNetworkId() != NetworkInstanceId.Invalid)
                {
                    GameObject enemyParentObj = null;
                    if (GameManager.getInstance().isServer())
                        enemyParentObj = NetworkServer.FindLocalObject(enemyObj.getParentNetworkId());
                    else
                        enemyParentObj = ClientScene.FindLocalObject(enemyObj.getParentNetworkId());

                    if (enemyParentObj.GetComponent<NetworkEnemyManager>().isLocalPlayer == false)
                    {
                        CustomDebug.Log("Bullet Parent enemy is Not local Enemy");
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void reset()
    {
        m_lookAt = null;
        m_path.reset();
    }

    public void onExternalDamage(float damage)
    {
        if (isCustomLocalPlayer() == false)
        {
			// TODO : Need to check
            CustomDebug.Log("On External Damage Enemy Base Not a local player");
            return;
        }

        m_health -= damage;
        if (m_health <= 0)
        {
            if (m_specialPower == SpecialPower.POWER_EXPLODE_ON_DEATH)
                StartCoroutine(invokeOnExplosion(m_explosionDelay));
            else
                onDeath();
        }
    }

    protected void setHitStatus(PathDefs.AI_Direction flag)
    {
        m_hitStatus =  m_hitStatus | (int)flag;
    }

    protected void resetHitStatus(PathDefs.AI_Direction flag)
    {
        m_hitStatus = m_hitStatus & ~ (int)flag;
    }

    protected bool isHitStatusSet(PathDefs.AI_Direction flag)
    {

        return ((m_hitStatus & (int)flag) != 0);
    }

    protected void resetAllHitStatus()
    {
        m_hitStatus = 0;
    }

    protected void setMoveStatus(PathDefs.AI_Direction flag)
    {
        m_moveStatus = m_moveStatus | (int) flag;
    }

    protected void resetMoveStatus(PathDefs.AI_Direction flag)
    {
        m_moveStatus = m_moveStatus & ~(int) flag;
    }

    protected bool isMovingInDirection(PathDefs.AI_Direction flag)
    {

        return ((m_moveStatus & (int)flag) != 0);
    }

    protected void resetMoveDirections()
    {
        m_moveStatus = 0;
    }

    // Special Power
    private void updateSpecialPower()
    {
        if (m_specialPower == SpecialPower.POWER_NONE)
            return;

        switch (m_specialPower)
        {
            case SpecialPower.POWER_AUTO_RECOVERY:
                updateAutoRecoveryPower();
                break;
        }
    }

    private void updateAutoRecoveryPower()
    {
        if (m_lastHitTime + m_powerActivationDelay > Time.time)
        {
            m_health += m_recoveryRate;
            if (m_health > m_maxHealth)
                m_health = m_maxHealth;
        }
    }

    IEnumerator invokeOnExplosion(float time)
    {
        /// TODO : need to check explosion in multiplayer mode
        yield return new WaitForSeconds(time);

        if (Vector3.Distance(transform.position, GameManager.getInstance().m_player.transform.position) < m_explosionRange)
        {
            GameManager.getInstance().m_player.onDamage(m_explosionDamage);
        }

        if (isCustomLocalPlayer())
        {
            onDeath();
        }
        else
        {
            CustomDebug.Log("Invoke Explosion Enemy Base Not a local player");
        }

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CustomDebug.Log("Enemy Base : On Start Client : "+m_team);
        if (m_team != Player.Player_Team.TEAM_NONE)
        {
            OnSetTeam(m_team);
        }

        if (m_parentInstanceId != NetworkInstanceId.Invalid)
        {
            OnSetParentInstanceId(m_parentInstanceId);
        }
        transform.Find("netid").GetComponent<TextMesh>().text = "" + netId.Value;
    }

    public override void OnNetworkDestroy()
    {
        base.OnNetworkDestroy();
        CustomDebug.Log("Destroying enemy");
    }

    private void OnSetPlayerInstanceId(NetworkInstanceId netId)
    {
        m_playerInstanceId = netId;
        CustomDebug.Log("OnSetPlayerInstance Id : " + netId.Value+ "    m_playerInstance Id : "+m_playerInstanceId);
        GameObject obj = null;
        if(GameManager.getInstance().isServer())
            obj = NetworkServer.FindLocalObject(netId);
        else
           obj = ClientScene.FindLocalObject(netId);
    
        if (obj != null)
        {
            CustomDebug.Log("Network Player found");
            setup(obj.transform);
        }
    }

    private void OnSetTeam(Player.Player_Team team)
    {
        m_team = team;
        if (m_team == Player.Player_Team.TEAM_CT)
        {
            ClanManager.getInstance().onSpawnedCT(gameObject);
        }
        else if (m_team == Player.Player_Team.TEAM_T)
        {
            ClanManager.getInstance().onSpawnedT(gameObject);
        }

        // set team indicator 
        Transform indicator = transform.Find("team_indicator");
        if (indicator != null)
        {
            indicator.gameObject.SetActive(true);
            indicator.GetComponent<SpriteRenderer>().material = EnemyManager.getInstance().getMaterial(m_team);
        }
    }

    public void setParentNetworkId(NetworkInstanceId netId)
    {
        m_parentInstanceId = netId;
    }

    public NetworkInstanceId getParentNetworkId()
    {
        return m_parentInstanceId;
    }

    public void OnSetParentInstanceId(NetworkInstanceId netId)
    {
        CustomDebug.Log("On set Parent Instance Id");
        m_parentInstanceId = netId;
    }
}
	