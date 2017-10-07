using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBase : MonoBehaviour {
    public enum CARD_TYPE {
        CARD_NPC_NONE = 0,
        CARD_NPC_FOLLOWER_HEALER,
        CARD_NPC_FOLLOWER_KILLER,
        CARD_NPC_FOLLOWER_HEALER_AND_KILLER,
        CARD_NPC_FOLLOWER_ARMY,
        CARD_NPC_COUNT,
    }

    public enum NPC_TYPE
    {
        NPC_NONE,
        NPC_KILLER,
        NPC_HEALER,
        NPC_KILLER_AND_HEALER,
        NPC_ARMY,
        NPC_COUNT,
    }

    public EnemyManager.ENEMY_TYPE m_enemyType = EnemyManager.ENEMY_TYPE.ENEMY_LINEAR;
    public NPC_TYPE m_npcType = NPC_TYPE.NPC_NONE;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
