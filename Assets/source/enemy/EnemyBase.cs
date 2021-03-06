﻿using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;
using System.Collections.Generic;

public class EnemyBase : NetworkBehaviour {
    [System.Serializable]
    public class CardData
    {
        public CardDataBase.NPC_TYPE m_npcType = CardDataBase.NPC_TYPE.NPC_NONE;
        public float m_healFactor = 0.0f;
        public float m_damageFactor = 0.0f;
    }

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
    public bool m_isBoss = false;

    [HideInInspector] public PathDefs.AI_PATH_TYPE m_pathType = PathDefs.AI_PATH_TYPE.PATH_LINEAR;

    protected int m_hitStatus = 0;
    protected int m_moveStatus = 0;
    [SyncVar]
    public BulletManager.BulletType m_bulletType = BulletManager.BulletType.BULLET_NONE;

    protected PathBase m_path = null;
    [SyncVar(hook = "OnSetPlayerInstanceId")]
    public NetworkInstanceId m_playerInstanceId;

    public float m_damage = 1.0f;

    private List<Transform> m_NPCTargetList = new List<Transform>();

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

    // Special Ability
    public enum SpecialAbility
    {
        ABILITY_NONE,
        ABILITY_MOVE_FAST_RANDOM_ON_HIT,
        ABILITY_MOVE_FAST_TOWARDS_ON_HIT,
    }
    public SpecialAbility m_specialAbility = SpecialAbility.ABILITY_NONE;

    // Ability generic
    private float m_abilityStartTime;
    private float m_originalSpeed;

    // Ability Move Fast
    public float m_abilitySpeed;
    public float m_abilityDuration;
    

    public int m_points = 0;
    [SyncVar(hook = "OnSetParentInstanceId")]
    public NetworkInstanceId m_parentInstanceId = NetworkInstanceId.Invalid;

    [SyncVar(hook = "OnSetCardData")]
    public CardData m_cardData;

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
        m_originalSpeed = m_speed;
        m_abilityStartTime = Time.time;
    }

    protected virtual void OnDisable() {
        m_speed = m_originalSpeed;
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
#if ENABLE_MULTIPLAYER
        if (GameManager.getInstance().isMultiplayer() && !GameManager.getInstance().isServer())
            return;
#endif

        EUpdate();
    }

    protected virtual void EUpdate()
    {
        if (m_bulletType != BulletManager.BulletType.BULLET_NONE)
        {
            if (m_pathType != PathDefs.AI_PATH_TYPE.PATH_RABBIT)
                updateBullet();
        }

        updateSpecialPower();
        updateSpecialAbility();
#if ENABLE_MULTIPLAYER
        if (GameManager.getInstance().isMultiplayer())
        {
            updateNPCTarget();
        }
#endif
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
#if ENABLE_MULTIPLAYER
        if (GameManager.getInstance().isMultiplayer() && !GameManager.getInstance().isServer())
            return;
#endif

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

    public virtual void setup(float speed, float health, BulletManager.BulletType bulletType = BulletManager.BulletType.BULLET_NONE, GameObject prefab = null, Transform lookAt = null, float bulletInterval = 0.0f, float bulletSpeed = 0.0f, CardDataBase.NPC_TYPE npcType = CardDataBase.NPC_TYPE.NPC_NONE)
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
        m_cardData.m_npcType = npcType;
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

            if(m_specialAbility == SpecialAbility.ABILITY_MOVE_FAST_RANDOM_ON_HIT || m_specialAbility == SpecialAbility.ABILITY_MOVE_FAST_TOWARDS_ON_HIT)
                triggerSpecialAbility();
           
            //Destroy(col.gameObject);
#if ENABLE_MULTIPLAYER
            if (GameManager.getInstance().isMultiplayer() && bulletBase.getParentTeam() == Team)
            {
                CustomDebug.Log("Same Team. Ignore bullet collision");
                return;
            }
#endif

            BulletManager.getInstance().onDestroyBullet(col.gameObject);
#if ENABLE_MULTIPLAYER
            if (GameManager.getInstance().isMultiplayer() && isCustomLocalPlayer(col.gameObject) == false)
            {
                CustomDebug.Log("Enemy Base Not a local player");
                return;
            }
#endif
            AnimData animData = new AnimData();
            animData.m_duration = 0.3f;
            animData.m_position = transform.position;
            AnimationManager.getInstance().startAnim(AnimationManager.AnimType.ANIM_HIT, animData);

            m_health -= 1.0f * GameManager.getInstance().m_player.m_damage;
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

        if (col.gameObject.layer == LayerMask.NameToLayer("enemyBullet"))
        {
#if ENABLE_MULTIPLAYER
            if (GameManager.getInstance().isMultiplayer())
            {
                BulletBase bulletBase = col.gameObject.GetComponent<BulletBase>();
                if (bulletBase == null)
                {
                    CustomDebug.LogError("Not bullet");
                    return;
                }

                if (GameManager.getInstance().isMultiplayer() && bulletBase.getParentTeam() == Team)
                {
                    CustomDebug.Log("Same Team. Ignore bullet collision");
                    return;
                }

                if (bulletBase.getParentNPCType() != CardDataBase.NPC_TYPE.NPC_ARMY)
                {
                    CustomDebug.Log("Bullet parent is of type army");
                    return;
                }

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
#endif
        }
    }

    private void onDeath()
    {
        CustomDebug.Log("OnDeath");
        Vector3 targetScale = transform.localScale + new Vector3(4.0f, 4.0f, 0.0f);
        BlastAnimData data = new BlastAnimData(transform, targetScale, 0.5f);
        AnimationManager.getInstance().startAnim(AnimationManager.AnimType.ANIM_DEATH, data);

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
#if ENABLE_MULTIPLAYER
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
#endif
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
        m_hitStatus = m_hitStatus | (int)flag;
    }

    protected void resetHitStatus(PathDefs.AI_Direction flag)
    {
        m_hitStatus = m_hitStatus & ~(int)flag;
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
        m_moveStatus = m_moveStatus | (int)flag;
    }

    protected void resetMoveStatus(PathDefs.AI_Direction flag)
    {
        m_moveStatus = m_moveStatus & ~(int)flag;
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
        CustomDebug.Log("Enemy Base : On Start Client : " + m_team);
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
        CustomDebug.Log("OnSetPlayerInstance Id : " + netId.Value + "    m_playerInstance Id : " + m_playerInstanceId);
        GameObject obj = null;
        if (GameManager.getInstance().isServer())
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

    public void OnSetCardData(CardData card)
    {
        CustomDebug.Log("On Set Card Data");
        m_cardData.m_npcType = card.m_npcType;
        m_cardData.m_healFactor = card.m_healFactor;
        m_cardData.m_damageFactor = card.m_damageFactor;
    }

    public bool isNPCHealer()
    {
        return (m_cardData.m_npcType == CardDataBase.NPC_TYPE.NPC_HEALER || m_cardData.m_npcType == CardDataBase.NPC_TYPE.NPC_KILLER_AND_HEALER);
    }

    public bool isNPCKiller()
    {
        return (m_cardData.m_npcType == CardDataBase.NPC_TYPE.NPC_KILLER || m_cardData.m_npcType == CardDataBase.NPC_TYPE.NPC_KILLER_AND_HEALER);
    }

    public bool isNPCArmy()
    {
        return (m_cardData.m_npcType == CardDataBase.NPC_TYPE.NPC_ARMY);
    }

    public float getNPCDamageFor(Player.Player_Team team)
    {
        if (m_team == team)
        {
            if (isNPCHealer())
                return -m_cardData.m_healFactor;
        }
        else
        {
            if (isNPCKiller() || isNPCArmy())
                return m_cardData.m_damageFactor;
        }

        return 0.0f;
    }

    public void checkForNPCTargets()
    {
        m_NPCTargetList.Clear();

        List<Transform> targetList = Util.getTransformListWithLayer(LayerMask.NameToLayer("player"));
        //CustomDebug.Log("Player Target List Size : " + targetList.Count);
        if (isNPCKiller()) // pick same team mate to kill
        {
            foreach (Transform trans in targetList)
            {
                if (trans.gameObject.GetComponent<NetworkTransform>().enabled) // TODO: Remove this hack and do it properly
                {
                    if (trans.gameObject.GetComponent<Player>().getTeam() == Team)
                        m_NPCTargetList.Add(trans);
                }
            }
        }

        if (isNPCHealer()) // pick opponent to heal
        {
            foreach (Transform trans in targetList)
            {
                if (trans.gameObject.GetComponent<NetworkTransform>().enabled)
                {
                    if (trans.gameObject.GetComponent<Player>().getTeam() != Team)
                        m_NPCTargetList.Add(trans);
                }
            }
        }

        if (isNPCArmy())
        {
            List<Transform> playerTargetList = Util.getTransformListWithLayer(LayerMask.NameToLayer("player"));
            List<Transform> npcTargetList = Util.getTransformListWithLayer(LayerMask.NameToLayer("enemy"));

            //CustomDebug.Log("Player Target List Size : " + playerTargetList.Count);
            //CustomDebug.Log("NPC Target List Size : " + npcTargetList.Count);
            foreach (Transform trans in playerTargetList)
            {
                if (trans.gameObject.GetComponent<NetworkTransform>().enabled)
                {
                    if (trans.gameObject.GetComponent<Player>().getTeam() != Team)
                    {
                        m_NPCTargetList.Add(trans);
                    }
                }
            }

            foreach (Transform trans in npcTargetList)
            {
                if (trans.gameObject.GetComponent<NetworkTransform>().enabled)
                {
                    if (trans.gameObject.GetComponent<EnemyBase>().Team != Team)
                        m_NPCTargetList.Add(trans);
                }
            }
        }
    }

    public void updateNPCTarget()
    {
        checkForNPCTargets();

        Transform target = null;
        float minDistance = 9999.0f;
        //CustomDebug.Log("Target List Size : " + m_NPCTargetList.Count);

        foreach (Transform trans in m_NPCTargetList)
        {
            float distance = Vector3.Distance(trans.position, transform.position);
            //CustomDebug.Log("Distance from Target : " + distance);
            if (minDistance > distance)
            {
                minDistance = distance;
                target = trans;
            }
        }

        m_lookAt = target;
        m_path.init(transform, m_speed, m_lookAt);
    }

    protected void changeSpeed(float speed)
    {
        m_speed = speed;
        m_path.setSpeed(speed);
    }

    protected void updateSpecialAbility()
    {
        switch (m_specialAbility)
        {
            case SpecialAbility.ABILITY_MOVE_FAST_RANDOM_ON_HIT:
            case SpecialAbility.ABILITY_MOVE_FAST_TOWARDS_ON_HIT:
                {
                    if (Time.time - m_abilityStartTime > m_abilityDuration)
                    {
                        changeSpeed(m_originalSpeed);
                    }
                }
                break;
        }
    }

    protected void triggerSpecialAbility()
    {
        switch (m_specialAbility)
        {
            case SpecialAbility.ABILITY_MOVE_FAST_RANDOM_ON_HIT:
            case SpecialAbility.ABILITY_MOVE_FAST_TOWARDS_ON_HIT:
                {
                    changeSpeed(m_originalSpeed + m_abilitySpeed);
                    m_abilityStartTime = Time.time;
                }
                break;
        }
    }

    public Vector3 getMovingDirection()
    {
        return m_path.getMovingDirection().normalized;
    }
}
	