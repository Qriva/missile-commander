using UnityEngine;
using UnityEditor;
using Game;

namespace GameEditor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReadOnlyAttribute.Mode mode = (attribute as ReadOnlyAttribute).mode;

            bool guiDisabled = mode == ReadOnlyAttribute.Mode.Always || 
            (mode == ReadOnlyAttribute.Mode.PlaymodeOnly && Application.isPlaying) || 
            (mode == ReadOnlyAttribute.Mode.EditorOnly && !Application.isPlaying);

            GUI.enabled = !guiDisabled;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
