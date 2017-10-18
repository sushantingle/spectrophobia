using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
/// <summary>
/// /class : Player
/// /brief : handles player's functionality. Animation, keypress, bullets and all.
/// </summary>
public class Player : NetworkBehaviour {
    public enum Player_Team
    {
        TEAM_NONE,
        TEAM_CT,
        TEAM_T,
    }

    [SyncVar(hook = "OnSetTeam")]
    public Player_Team m_team = Player_Team.TEAM_NONE;
    public float 		m_speed 		= 0.1f;
	public 	float 		m_fireInterval 	= 0.5f;
    public float        m_bulletSpeed   = 0.1f;
	private float 		m_lastFireTime 	= 0;	
	public float 		m_raycastOffset = 0.0f; 
	public Text 		m_healthText;

	private	bool 			m_hitLeft 		= false;
	private bool 			m_hitRight 		= false;
	private bool 			m_hitUp 		= false;
	private bool 			m_hitDown 		= false;
	private BoxCollider2D	m_boxCollider;	
    private float           m_health;    
    public  ProgressBar     m_healthBar;

    public bool m_autoAim = false;
    private Vector3 m_lastFireDirection = Vector3.zero;
	// Use this for initialization
	void Start () {
		m_boxCollider = GetComponent<BoxCollider2D> ();
        m_health = PlayerDefs.CONST_PLAYER_MAX_HEALTH;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.getInstance ().isGamePaused ())
			return;

        if (!isLocalPlayer)
            return;
        // handle animations
		Vector3 pos = transform.position;
#if UNITY_ANDROID
        Vector3 deltaTargetPos = (new Vector3(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"), transform.position.z)) * m_speed;
        CustomDebug.Log("Target Position: " + deltaTargetPos);
        if ((deltaTargetPos.x > 0.0f && !m_hitRight) || (deltaTargetPos.x < 0.0f && !m_hitLeft))
            pos.x += deltaTargetPos.x;

        if ((deltaTargetPos.y > 0.0f && !m_hitUp) || (deltaTargetPos.y < 0.0f && !m_hitDown))
            pos.y += deltaTargetPos.y;

#else
        Vector3 deltaTargetPos = (new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), transform.position.z)) * m_speed;
        //CustomDebug.Log("Target Position: " + deltaTargetPos);
        if ((deltaTargetPos.x > 0.0f && !m_hitRight) || (deltaTargetPos.x < 0.0f && !m_hitLeft))
            pos.x += deltaTargetPos.x;

        if ((deltaTargetPos.y > 0.0f && !m_hitUp) || (deltaTargetPos.y < 0.0f && !m_hitDown))
            pos.y += deltaTargetPos.y;
#endif
        // update player position on key press
        transform.position = pos;

        Vector3 fireDirection = Vector3.zero;
        if (m_autoAim)
        {
            GameObject obj = EnemyManager.getInstance().getNearestEnemy(transform.position);
            if (obj != null)
                fireDirection = (obj.transform.position - transform.position).normalized;
        }
        else
        {
            #if UNITY_ANDROID
                fireDirection = (new Vector3(CrossPlatformInputManager.GetAxis("fire_horizontal"), CrossPlatformInputManager.GetAxis("fire_vertical"), 0.0f));
            #else
                fireDirection = (new Vector3(Input.GetAxis("fire_horizontal"), Input.GetAxis("fire_vertical"), 0.0f));
            #endif
        }

        if (((Time.time - m_lastFireTime) > m_fireInterval) && fireDirection != Vector3.zero)  {

            if (!fireDirection.Equals(Vector3.zero))
            {
                fireBullet(fireDirection);
                m_lastFireDirection = fireDirection;
            }
        }
	}

	void FixedUpdate() {
        if (!isLocalPlayer)
            return;
        // check if player is at wall
        int layermask = 1 << LayerMask.NameToLayer ("wall");

		RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, m_boxCollider.bounds.size.y / 2 + m_raycastOffset, layermask);
		Debug.DrawRay (transform.position, Vector2.up * (m_boxCollider.bounds.size.y / 2 + m_raycastOffset), Color.green);
		m_hitUp = (hitUp.collider != null);

		RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, m_boxCollider.bounds.size.y / 2 + m_raycastOffset, layermask);
		Debug.DrawRay (transform.position, -Vector2.up * (m_boxCollider.bounds.size.y / 2 + m_raycastOffset), Color.blue);
		m_hitDown = (hitDown.collider != null);

		RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, m_boxCollider.bounds.size.x / 2 + m_raycastOffset, layermask);
		Debug.DrawRay (transform.position, Vector2.right * (m_boxCollider.bounds.size.x / 2 + m_raycastOffset), Color.grey);
		m_hitRight = (hitRight.collider != null);

		RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, m_boxCollider.bounds.size.x / 2 + m_raycastOffset, layermask);
		Debug.DrawRay (transform.position, Vector2.left * (m_boxCollider.bounds.size.x / 2 + m_raycastOffset), Color.magenta);
		m_hitLeft = (hitLeft.collider != null);

	}

    // Fire bullet	
    void fireBullet(Vector3 direction) {
        int layer = LayerMask.NameToLayer("playerbullet");
        NetworkInstanceId parentNetId = GetComponent<NetworkIdentity>().netId;

        if (ItemManager.getInstance().hasItemActive(ItemManager.ITEM_TYPE.ITEM_S))
        {
            BulletManager.getInstance().initBullet(BulletManager.BulletType.BULLET_LINEAR_N_PI, layer, transform, direction, m_bulletSpeed, 4, parentNetId);
        }
        else if (ItemManager.getInstance().hasItemActive(ItemManager.ITEM_TYPE.ITEM_D))
        {
            BulletManager.getInstance().initBullet(BulletManager.BulletType.BULLET_SPIRAL, layer, transform, direction, m_bulletSpeed, 1, parentNetId);
        }
        else
        {
            BulletManager.getInstance().initBullet(BulletManager.BulletType.BULLET_LINEAR, layer, transform, direction, m_bulletSpeed, parentNetId);
        }

		m_lastFireTime = Time.time;
	}

    // callback on hit by bullet/ enemy
	public void onDamage(float damage = 1.0f) {
        // check if player has invincible power
		if (ItemManager.getInstance ().hasItemActive (ItemManager.ITEM_TYPE.ITEM_INVINCIBLE))
			return;

        // update player's health
		m_health -= damage;
		if (m_health <= 0) {
            onPlayerDied();
			GameManager.getInstance ().endGame ();
		}

        if (m_health > PlayerDefs.CONST_PLAYER_MAX_HEALTH)
            m_health = PlayerDefs.CONST_PLAYER_MAX_HEALTH;
        
        updateHealthBar();
	}

    void updateHealthBar()
    {
        m_healthBar.setValue(m_health / PlayerDefs.CONST_PLAYER_MAX_HEALTH);
    }
	public void onStartGame() {
        resetPlayer();
        m_team = ClanManager.getInstance().SelectedTeam;
    }

    public void onResumeGame()
    {
        resetPlayer();
    }

    public void resetPlayer() {
        GetComponent<SpriteRenderer>().material.color = Color.green;
        m_health = PlayerDefs.CONST_PLAYER_MAX_HEALTH;
        updateHealthBar();
        ItemManager.getInstance().resetItemManager();
    }

    private void onPlayerDied() {
        GetComponent<SpriteRenderer>().material.color = Color.red;
    }

    // update health on collecting health item
	public void collectedLife(int count) {
		m_health += count;
        if (m_health > PlayerDefs.CONST_PLAYER_MAX_HEALTH)
            m_health = PlayerDefs.CONST_PLAYER_MAX_HEALTH;
        updateHealthBar();
	}

    public void collectedSpeedBullet(float increment)
    {
        m_bulletSpeed += increment;
        if (m_bulletSpeed > PlayerDefs.CONST_MAX_FIRE_SPEED)
            m_bulletSpeed = PlayerDefs.CONST_MAX_FIRE_SPEED;
    }

    // Collision callback
	void OnTriggerEnter2D(Collider2D col)
	{
        return;
		if (col.gameObject.layer == LayerMask.NameToLayer ("enemy")) {
            EnemyBase enemy = col.gameObject.GetComponent<EnemyBase>();
            if (GameManager.getInstance().isSinglePlayer())
                onDamage(enemy.m_damage);
#if ENABLE_MULTIPLAYER
            else if (GameManager.getInstance().isMultiplayer())
            {
                if (m_team == enemy.Team)
                {
                    CustomDebug.Log("Same Team Player and Enemy");
                    return;
                }
                // TODO: Decide if touching other enemies will cause problem or not.
            }
#endif
        }

		if (col.gameObject.layer == LayerMask.NameToLayer ("enemybullet")) {
            BulletManager.getInstance().onDestroyBullet(col.gameObject);
            if (GameManager.getInstance().isSinglePlayer())
            {
                onDamage();
            }
#if ENABLE_MULTIPLAYER
            else if (GameManager.getInstance().isMultiplayer())
            {
                NetworkInstanceId parentNetId = col.gameObject.GetComponent<BulletBase>().getParentNetId();
                CustomDebug.Log("Bullet Parent Net id :" + parentNetId.Value);
                if (parentNetId == NetworkInstanceId.Invalid)
                {
                    CustomDebug.LogError("Bullet parent is missing");
                    onDamage(0.0f);
                }
                else
                {
                    EnemyBase enemy = null;
                    if (GameManager.getInstance().isServer())
                        enemy = NetworkServer.FindLocalObject(parentNetId).GetComponent<EnemyBase>();
                    else
                        enemy = ClientScene.FindLocalObject(parentNetId).GetComponent<EnemyBase>();
                    onDamage(enemy.getNPCDamageFor(m_team));
                }
            }
#endif
        }
	}

    public void damageInRange(Vector3 position, float range, float damage)
    {
        if (Vector3.Distance(position, transform.position) < range)
            onDamage(damage);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GameManager.getInstance().onStartLocalPlayer(gameObject);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        Debug.Log("Spawned Network object : "+m_team);
    }

    public override void OnNetworkDestroy()
    {
        base.OnNetworkDestroy();

        Debug.Log("Destroyed network object : "+m_team);
        if (m_team == Player_Team.TEAM_CT)
            ClanManager.getInstance().onDestroyingCT(gameObject);
        else if (m_team == Player_Team.TEAM_T)
            ClanManager.getInstance().onDestroyingT(gameObject);
    }

    public void OnSetTeam(Player_Team team)
    {
        CustomDebug.Log("Team : " + team);
        m_team = team;
        if (m_team == Player_Team.TEAM_CT)
        {
            ClanManager.getInstance().onSpawnedCT(gameObject);
        }
        else if (m_team == Player_Team.TEAM_T)
        {
            ClanManager.getInstance().onSpawnedT(gameObject);
        }
    }

    public Player.Player_Team getTeam()
    {
        return m_team;
    }
}
