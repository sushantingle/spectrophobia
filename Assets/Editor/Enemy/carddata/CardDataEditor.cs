using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardDataScriptableObject))]
public class CardDataEditor : Editor
{
    CardDataScriptableObject cardObj;

    public EnemyManager.ENEMY_TYPE m_enemyType = EnemyManager.ENEMY_TYPE.ENEMY_LINEAR;
    public CardDataBase.NPC_TYPE m_npcType = CardDataBase.NPC_TYPE.NPC_NONE;
    public float m_healFactor = 0;
    public float m_damageFactor = 0;
    public float m_health = 5;

    protected virtual void OnEnable()
    {
        cardObj = (CardDataScriptableObject)target;
    }

    public override void OnInspectorGUI()
    {
        cardObj.card.m_enemyType = (EnemyManager.ENEMY_TYPE)EditorGUILayout.EnumPopup("EnemyType", cardObj.card.m_enemyType);
        cardObj.card.m_npcType = (CardDataBase.NPC_TYPE) EditorGUILayout.EnumPopup("NpcType", cardObj.card.m_npcType);
        if(cardObj.card.m_npcType != CardDataBase.NPC_TYPE.NPC_KILLER && cardObj.card.m_npcType != CardDataBase.NPC_TYPE.NPC_ARMY)
            cardObj.card.m_healFactor = EditorGUILayout.FloatField("Heal Factor", cardObj.card.m_healFactor);
        if (cardObj.card.m_npcType == CardDataBase.NPC_TYPE.NPC_KILLER ||
            cardObj.card.m_npcType == CardDataBase.NPC_TYPE.NPC_KILLER_AND_HEALER ||
            cardObj.card.m_npcType == CardDataBase.NPC_TYPE.NPC_ARMY)
            cardObj.card.m_damageFactor = EditorGUILayout.FloatField("Damage Factor", cardObj.card.m_damageFactor);
        cardObj.card.m_health = EditorGUILayout.FloatField("Health", cardObj.card.m_health);
        EditorUtility.SetDirty(cardObj);
    }
}
