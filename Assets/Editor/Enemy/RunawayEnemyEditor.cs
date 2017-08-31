using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RunawayEnemy)), CanEditMultipleObjects]
public class RunawayEnemyEditor : EnemyBaseEditor {

    public SerializedProperty
        m_changeTimeOffset_Prop,
        m_safeDistance_Prop;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_changeTimeOffset_Prop = serializedObject.FindProperty("m_changeTimeOffset");
        m_safeDistance_Prop = serializedObject.FindProperty("m_safeDistance");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_changeTimeOffset_Prop);
        EditorGUILayout.PropertyField(m_safeDistance_Prop);
        serializedObject.ApplyModifiedProperties();
    }
}
