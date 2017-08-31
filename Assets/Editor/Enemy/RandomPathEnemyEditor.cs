using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomPathEnemy)), CanEditMultipleObjects]
public class RandomPathEnemyEditor : EnemyBaseEditor {

    public SerializedProperty
        m_changeTimeOffset_Prop;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_changeTimeOffset_Prop = serializedObject.FindProperty("m_changeTimeOffset");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_changeTimeOffset_Prop);
        serializedObject.ApplyModifiedProperties();
    }
}
