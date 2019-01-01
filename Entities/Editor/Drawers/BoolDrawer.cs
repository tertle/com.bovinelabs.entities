// <copyright file="BoolDrawer.cs" company="Timothy Raines">
//     Copyright (c) Timothy Raines. All rights reserved.
// </copyright>

#if ODIN_INSPECTOR
namespace BovineLabs.Entities.Editor.Drawers
{
    using BovineLabs.Entities.Helpers;
    using JetBrains.Annotations;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws <see cref="Bool"/> in the inspector.
    /// </summary>
    [UsedImplicitly]
    public class BoolDrawer : OdinValueDrawer<Bool>
    {
        /// <inheritdoc />
        protected override void DrawPropertyLayout(GUIContent label)
        {
            this.ValueEntry.SmartValue = EditorGUILayout.Toggle(label, this.ValueEntry.SmartValue);
        }
    }
}
#endif