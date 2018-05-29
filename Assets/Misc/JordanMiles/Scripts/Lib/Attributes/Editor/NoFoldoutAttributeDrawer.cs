using JMiles42.Attributes;
using UnityEditor;
using UnityEngine;

namespace JMiles42.Editor.PropertyDrawers {
	[CustomPropertyDrawer(typeof(NoFoldoutAttribute), true), CustomPropertyDrawer(typeof(NoFoldoutClass), true)]
	public class NoFoldoutAttributeDrawer : JMilesPropertyDrawer<NoFoldoutAttribute> {
		public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label) {
			//var property = serializedProperty.Copy();
			//bool next = property.NextVisible(true);
			//if (!next) return;
			serializedProperty.isExpanded = true;
			int depth = serializedProperty.depth;
			var rect = position;
			rect.height = singleLine;
			while (serializedProperty.NextVisible(true) && serializedProperty.depth > depth) {
				serializedProperty.isExpanded = true;
				EditorGUI.PropertyField(rect, serializedProperty);
				rect.y += singleLine;
			}
		}

		public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label) {
			var childCount = 0;
			int depth = serializedProperty.depth;
			while (serializedProperty.NextVisible(true) && serializedProperty.depth > depth) {
				if (serializedProperty.isExpanded) childCount++;
			}
			return (childCount * singleLine);
		}
	}
}