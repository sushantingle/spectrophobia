using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomPathEnemy)), CanEditMultipleObjects]
public class RandomPathEnemyEditor : EnemyBaseEditor {

    public SerializedProperty
        m_changeTimeOffset_Prop,
        m_bonusSpeed_Prop,
        m_bonusSpeedDuration_Prop;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_changeTimeOffset_Prop = serializedObject.FindProperty("m_changeTimeOffset");
        m_bonusSpeed_Prop = serializedObject.FindProperty("m_bonusSpeed");
        m_bonusSpeedDuration_Prop = serializedObject.FindProperty("m_bonusSpeedDuration");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_changeTimeOffset_Prop);

        bool isBoss = m_isBoss_Prop.boolValue;
        if (isBoss)
        {
            EditorGUILayout.PropertyField(m_bonusSpeed_Prop);
            EditorGUILayout.PropertyField(m_bonusSpeedDuration_Prop);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
