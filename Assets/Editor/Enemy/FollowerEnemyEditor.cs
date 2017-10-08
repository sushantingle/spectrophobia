using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FollowerEnemy)), CanEditMultipleObjects]
public class FollowerEnemyEditor : EnemyBaseEditor {
    public SerializedProperty
       m_distanceOffset_Prop;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_distanceOffset_Prop = serializedObject.FindProperty("m_distanceOffset");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_distanceOffset_Prop);
        serializedObject.ApplyModifiedProperties();
    }
}
