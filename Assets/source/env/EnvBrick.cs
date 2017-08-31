using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvBrick : MonoBehaviour {

    public bool  m_isBrekable   = false;
    public int   m_health       = 0;
    public List<Material> m_brickStates;

    public enum BrickPower
    {
        BRICK_NONE,
        BRICK_ITEM,
        BRICK_BLAST,
        BRICK_ENEMY,
    }

    public BrickPower m_type = BrickPower.BRICK_NONE;

    public ItemManager.ITEM_TYPE m_itemType = ItemManager.ITEM_TYPE.ITEM_NONE;
    public EnemyManager.ENEMY_TYPE m_enemyType = EnemyManager.ENEMY_TYPE.ENEMY_LINEAR;
    public float m_radius = 0.0f;
    public float m_damage = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("playerbullet"))
        {
            // destroy bullet
            Destroy(collision.gameObject);

            // destroy brick if breakable
            if (m_isBrekable)
            {
                if (m_health > 0)
                {
                    --m_health;
                    updateBrick();
                }
                else
                {
                    if (m_type == BrickPower.BRICK_ITEM || m_type == BrickPower.BRICK_ENEMY)
                        popObject();
                    else if(m_type == BrickPower.BRICK_BLAST)
                        explosion();
                    Destroy(gameObject);
                }
            }
        }
    }

    private void updateBrick()
    {
        if(m_health >= 0 && m_health < m_brickStates.Count)
            GetComponent<SpriteRenderer>().material = m_brickStates[m_health];
    }

    private void popObject()
    {
        if(m_type == BrickPower.BRICK_ITEM)
            ItemManager.getInstance().generateItem(transform.position, m_itemType);
        else if(m_type == BrickPower.BRICK_ENEMY)
           EnemyManager.getInstance().spawnEnemy(m_enemyType, transform.position, Quaternion.identity);
    }

    private void explosion()
    {
        // TODO: should affect nearby enemies, player and bricks
        EnemyManager.getInstance().onExplosion(transform.position, m_radius, m_damage);
        GameManager.getInstance().m_player.damageInRange(transform.position, m_radius, m_damage);
    }
}
