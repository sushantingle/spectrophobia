using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyBase)), CanEditMultipleObjects]
public class EnemyBaseEditor : Editor
{

    public SerializedProperty
        m_speed_Prop,
        m_maxhealth_Prop,
        m_bulletType_Prop,
        m_bulletInterval_Prop,
        m_bulletSpeed_Prop,
        m_bulletDistance_Prop,
        m_bulletMaxTime_Prop,
        m_bulletGravity_Prop,
        m_bulletProjectionAngle_Prop,
        m_bulletAmplitude_Prop,
        m_bulletCount_Prop,
        m_specialPower_Prop,
        m_recoveryRate_Prop,
        m_powerActivationDelay_Prop,
        m_explosionRange_Prop,
        m_explosionDelay_Prop,
        m_explosionDamage_Prop,
        m_points_Prop,
        m_Team_Prop;

    protected virtual void OnEnable()
    {
        m_speed_Prop = serializedObject.FindProperty("m_speed");
        m_maxhealth_Prop = serializedObject.FindProperty("m_maxHealth");
        m_bulletType_Prop = serializedObject.FindProperty("m_bulletType");
        m_bulletInterval_Prop = serializedObject.FindProperty("m_bulletInterval");
        m_bulletSpeed_Prop = serializedObject.FindProperty("m_bulletSpeed");
        m_bulletDistance_Prop = serializedObject.FindProperty("m_bulletDistance");
        m_bulletMaxTime_Prop = serializedObject.FindProperty("m_bulletMaxTime");
        m_bulletGravity_Prop = serializedObject.FindProperty("m_bulletGravity");
        m_bulletProjectionAngle_Prop = serializedObject.FindProperty("m_bulletProjectionAngle");
        m_bulletAmplitude_Prop = serializedObject.FindProperty("m_bulletAmplitude");
        m_bulletCount_Prop = serializedObject.FindProperty("m_bulletCount");
        m_specialPower_Prop = serializedObject.FindProperty("m_specialPower");
        m_recoveryRate_Prop = serializedObject.FindProperty("m_recoveryRate");
        m_powerActivationDelay_Prop = serializedObject.FindProperty("m_powerActivationDelay");
        m_explosionRange_Prop = serializedObject.FindProperty("m_explosionRange");
        m_explosionDelay_Prop = serializedObject.FindProperty("m_explosionDelay");
        m_explosionDamage_Prop = serializedObject.FindProperty("m_explosionDamage");
        m_points_Prop = serializedObject.FindProperty("m_points");
        m_Team_Prop = serializedObject.FindProperty("m_team");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_speed_Prop);
        EditorGUILayout.PropertyField(m_maxhealth_Prop);
        EditorGUILayout.PropertyField(m_bulletType_Prop);

        BulletManager.BulletType bulletType = (BulletManager.BulletType)m_bulletType_Prop.enumValueIndex;
        if (bulletType != BulletManager.BulletType.BULLET_NONE)
        {
            EditorGUILayout.PropertyField(m_bulletSpeed_Prop);
            EditorGUILayout.PropertyField(m_bulletInterval_Prop);

            switch (bulletType)
            {
                case BulletManager.BulletType.BULLET_LINEAR_N:
                    EditorGUILayout.PropertyField(m_bulletCount_Prop);
                    break;

                case BulletManager.BulletType.BULLET_PROJECTILE:
                case BulletManager.BulletType.BULLET_PROJECTILE_4:
                    EditorGUILayout.PropertyField(m_bulletDistance_Prop);
                    EditorGUILayout.PropertyField(m_bulletMaxTime_Prop);
                    EditorGUILayout.PropertyField(m_bulletGravity_Prop);
                    EditorGUILayout.PropertyField(m_bulletProjectionAngle_Prop);
                    break;

                case BulletManager.BulletType.BULLET_SINE:
                case BulletManager.BulletType.BULLET_SPIRAL:
                    EditorGUILayout.PropertyField(m_bulletAmplitude_Prop);
                    break;
            }
        }

        EditorGUILayout.PropertyField(m_specialPower_Prop);
        EnemyBase.SpecialPower powerType = (EnemyBase.SpecialPower) m_specialPower_Prop.enumValueIndex;
        if (powerType != EnemyBase.SpecialPower.POWER_NONE)
        {
            switch(powerType)
            {
                case EnemyBase.SpecialPower.POWER_AUTO_RECOVERY:
                    EditorGUILayout.PropertyField(m_recoveryRate_Prop);
                    EditorGUILayout.PropertyField(m_powerActivationDelay_Prop);
                    break;
                case EnemyBase.SpecialPower.POWER_EXPLODE_ON_DEATH:
                    EditorGUILayout.PropertyField(m_explosionRange_Prop);
                    EditorGUILayout.PropertyField(m_explosionDamage_Prop);
                    EditorGUILayout.PropertyField(m_explosionDelay_Prop);
                    break;
            }
            
        }
        EditorGUILayout.PropertyField(m_points_Prop);
        EditorGUILayout.PropertyField(m_Team_Prop);
        serializedObject.ApplyModifiedProperties();
    }
}
