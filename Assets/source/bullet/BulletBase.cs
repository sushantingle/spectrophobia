using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletBase : MonoBehaviour
{
    protected float m_speed = 0.5f;
    protected Vector3 m_direction = Vector3.zero;
    public BulletManager.BulletType m_type = BulletManager.BulletType.BULLET_NONE;
    protected NetworkInstanceId m_parentNetId = NetworkInstanceId.Invalid;
    protected BulletColorScriptablePObj m_bulletColorScriptableObj;
    public CardDataBase.NPC_TYPE m_targetNPCType = CardDataBase.NPC_TYPE.NPC_NONE;

    protected virtual void Awake()
    {
        CustomDebug.Log("Loading Scriptable Object Bullet Material");
        m_bulletColorScriptableObj = Resources.Load<BulletColorScriptablePObj>("bulletdata/Bullet_Material");
        if (m_bulletColorScriptableObj == null)
            CustomDebug.LogError("Scriptable object Bullet Material is missing");
    }
    // Use this for initialization
    void Start()
    {
        BStart();
    }

    protected virtual void BStart()
    {

    }

    protected virtual void setup(NetworkInstanceId parentNetId, Vector3 direction, float speed)
    {
        m_direction = direction;
        m_speed = speed;
        m_parentNetId = parentNetId;
        #if ENABLE_MULTIPLAYER
        if (GameManager.getInstance().isMultiplayer())
            {
                CardDataBase.NPC_TYPE npcType = getParentNPCType();
                if (npcType != CardDataBase.NPC_TYPE.NPC_NONE)
                    GetComponent<SpriteRenderer>().material = m_bulletColorScriptableObj.getMaterialOf(npcType);
                else
                    CustomDebug.Log("Not valid NPC Type");
            }
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        BUpdate();
    }

    protected virtual void BUpdate()
    {

    }

    private void FixedUpdate()
    {
        BFixedUpdate();
    }

    protected virtual void BFixedUpdate()
    {

    }

    protected bool isOutsideOfScreen()
    {
        if (GameManager.getInstance().isSinglePlayer())
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (screenPos.x < 0 || screenPos.x > Screen.width)
                return true;
            if (screenPos.y < 0 || screenPos.y > Screen.height)
                return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BOnTriggerEnter2D(collision);
    }

    protected virtual void BOnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.getInstance().isSinglePlayer())
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("wall"))
            {
                Destroy(gameObject);
            }
        }
    }

    public NetworkInstanceId getParentNetId()
    {
        return m_parentNetId;
    }

    public CardDataBase.NPC_TYPE getParentNPCType()
    {
        if (m_parentNetId == NetworkInstanceId.Invalid)
        {
            CustomDebug.LogError("Invalid bullet parent id");
            return CardDataBase.NPC_TYPE.NPC_NONE;
        }

        GameObject parent = null;
        if (GameManager.getInstance().isServer())
        {
            parent = NetworkServer.FindLocalObject(m_parentNetId);
        }
        else
        {
            parent = ClientScene.FindLocalObject(m_parentNetId);
        }

        if (parent == null)
        {
            CustomDebug.LogError("Bullet Parent Missing");
            return CardDataBase.NPC_TYPE.NPC_NONE;
        }

        if (gameObject.layer == LayerMask.NameToLayer("enemybullet"))
        {
            EnemyBase enemy = parent.GetComponent<EnemyBase>();
            if (enemy.m_cardData.m_npcType == CardDataBase.NPC_TYPE.NPC_KILLER_AND_HEALER)
            {
                if (enemy.m_lookAt != null)
                {
                    CustomDebug.Log("Target Team : " + enemy.m_lookAt.GetComponent<Player>().getTeam());
                    CustomDebug.Log("NPC Team : " + enemy.Team);
                    if (enemy.m_lookAt.GetComponent<Player>().getTeam() == enemy.Team)
                        return CardDataBase.NPC_TYPE.NPC_KILLER;
                    else
                        return CardDataBase.NPC_TYPE.NPC_HEALER;
                }
                else
                {
                    CustomDebug.LogError("Bullet parent Look At is null");
                }
            }
            else
            {
                CustomDebug.LogError("Bullet parent NPC Type : "+ enemy.m_cardData.m_npcType);
                return enemy.m_cardData.m_npcType;
            }
        }
        CustomDebug.LogError("NPC type of bullet parent is unknown");
        return CardDataBase.NPC_TYPE.NPC_NONE;
    }

    public bool isNPCParentArmy()
    {
        if (m_parentNetId == NetworkInstanceId.Invalid)
        {
            CustomDebug.LogError("Parent instanceid is invalid");
            return false;
        }

        if (gameObject.layer == LayerMask.NameToLayer("enemybullet"))
        {
            GameObject parent = null;
            if (GameManager.getInstance().isServer())
            {
                parent = NetworkServer.FindLocalObject(m_parentNetId);
            }
            else
            {
                parent = ClientScene.FindLocalObject(m_parentNetId);
            }

            if (parent == null)
            {
                CustomDebug.LogError("Bullet Parent Missing");
                return false;
            }

            EnemyBase enemy = parent.GetComponent<EnemyBase>();
            return enemy.isNPCArmy();
        }

        CustomDebug.LogError("Bullet layer is PlayerBullet and it can't be of type Army");
        return false;
    }

    public Player.Player_Team getParentTeam()
    {
        if (m_parentNetId == NetworkInstanceId.Invalid)
        {
            CustomDebug.LogError("Parent instanceid is invalid");
            return Player.Player_Team.TEAM_NONE;
        }

        GameObject parent = null;
        if (GameManager.getInstance().isServer())
        {
            parent = NetworkServer.FindLocalObject(m_parentNetId);
        }
        else
        {
            parent = ClientScene.FindLocalObject(m_parentNetId);
        }

        if (parent == null)
        {
            CustomDebug.LogError("Bullet Parent Missing");
            return Player.Player_Team.TEAM_NONE;
        }

        if (gameObject.layer == LayerMask.NameToLayer("enemybullet"))
        {
            EnemyBase enemy = parent.GetComponent<EnemyBase>();
            return enemy.Team;
        }
        else if (gameObject.layer == LayerMask.NameToLayer("playerbullet"))
        {
            Player player = parent.GetComponent<Player>();
            return player.getTeam();
        }

        CustomDebug.LogError("Bullet layer is invalid");
        return Player.Player_Team.TEAM_NONE;
    }
}