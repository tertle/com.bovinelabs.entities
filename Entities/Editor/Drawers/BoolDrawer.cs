// <copyright file="BoolDrawer.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Entities.Editor.Drawers
{
    using BovineLabs.Entities.Helpers;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(Bool))]
    public class BoolDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, prop);
            SerializedProperty value = prop.FindPropertyRelative("value");
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();
            bool currentVal = EditorGUI.Toggle(position, value.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                value.boolValue = currentVal;
            }

            EditorGUI.EndProperty();
        }
    }
}