using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnerEnemy)), CanEditMultipleObjects]
public class EditorSpawnerEnemy : EnemyBaseEditor {

    public SerializedProperty
        m_spawnType_Prop,
        m_spawnCondition_Prop,
        m_changeTimeOffset_Prop,
        m_spawnTimeInterval_Prop,
        m_spawnCount_Prop,
        m_pathType_Prop,
        m_safeDistance_Prop,
        m_popOutInterval_Prop,
        m_stayOnDuration_Prop,
        m_childSpeed_Prop,
        m_childHealth_Prop,
        m_childBulletPrefab_Prop,
        m_childBulletInterval_Prop,
        m_childBulletSpeed_Prop,
        m_childBulletType_Prop,
        m_childChangeTimeOffset_Prop,
        m_childSafeDistance_Prop;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_spawnType_Prop = serializedObject.FindProperty("m_spawnType");
        m_changeTimeOffset_Prop = serializedObject.FindProperty("m_changeTimeOffset");
        m_spawnCondition_Prop = serializedObject.FindProperty("m_spawnCondition");
        m_spawnTimeInterval_Prop = serializedObject.FindProperty("m_spawnTimeInterval");
        m_spawnCount_Prop = serializedObject.FindProperty("m_spawnCount");
        m_pathType_Prop = serializedObject.FindProperty("m_pathType");
        m_safeDistance_Prop = serializedObject.FindProperty("m_safeDistance");
        m_popOutInterval_Prop = serializedObject.FindProperty("m_popOutInterval");
        m_stayOnDuration_Prop = serializedObject.FindProperty("m_stayOnDuration");

        // Child
        m_childSpeed_Prop = serializedObject.FindProperty("m_childSpeed");
        m_childHealth_Prop = serializedObject.FindProperty("m_childHealth");
        m_childBulletPrefab_Prop = serializedObject.FindProperty("m_childBulletPrefab");
        m_childBulletInterval_Prop = serializedObject.FindProperty("m_childBulletInterval");
        m_childBulletSpeed_Prop = serializedObject.FindProperty("m_childBulletSpeed");
        m_childBulletType_Prop = serializedObject.FindProperty("m_childBulletType");
        m_childChangeTimeOffset_Prop = serializedObject.FindProperty("m_childChangeTimeOffset");
        m_childSafeDistance_Prop = serializedObject.FindProperty("m_childSafeDistance");

        m_points_Prop = serializedObject.FindProperty("m_points");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(m_spawnType_Prop);

        EnemyManager.ENEMY_TYPE type = (EnemyManager.ENEMY_TYPE)m_spawnType_Prop.enumValueIndex;
        {
            EditorGUILayout.PropertyField(m_spawnCount_Prop);
            EditorGUILayout.PropertyField(m_childSpeed_Prop);
            EditorGUILayout.PropertyField(m_childHealth_Prop);

            switch (type)
            {
                case EnemyManager.ENEMY_TYPE.ENEMY_RANDOM:
                case EnemyManager.ENEMY_TYPE.ENEMY_RANDOM_SHOOTER:
                    EditorGUILayout.PropertyField(m_changeTimeOffset_Prop);
                    break;

                case EnemyManager.ENEMY_TYPE.ENEMY_RUNAWAY:
                    EditorGUILayout.PropertyField(m_changeTimeOffset_Prop);
                    EditorGUILayout.PropertyField(m_safeDistance_Prop);
                    break;
            }

            EditorGUILayout.PropertyField(m_childBulletType_Prop);
            BulletManager.BulletType childBulletType = (BulletManager.BulletType)m_childBulletType_Prop.enumValueIndex;

            if (childBulletType != BulletManager.BulletType.BULLET_NONE)
            {
                EditorGUILayout.PropertyField(m_childBulletPrefab_Prop);
                EditorGUILayout.PropertyField(m_childBulletInterval_Prop);
                EditorGUILayout.PropertyField(m_childBulletSpeed_Prop);
            }
            
            EditorGUILayout.PropertyField(m_spawnCondition_Prop);
            SpawnerEnemy.SpawnCondition cond = (SpawnerEnemy.SpawnCondition)m_spawnCondition_Prop.enumValueIndex;

            if (cond == SpawnerEnemy.SpawnCondition.SPAWN_PERIODICALLY)
            {
                EditorGUILayout.PropertyField(m_spawnTimeInterval_Prop);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
