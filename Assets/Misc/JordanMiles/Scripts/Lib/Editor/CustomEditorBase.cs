using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Reflection;
using JMiles42.BaseClasses;

namespace JMiles42.Editor {
	//THIS IS NOT MY CODE
	//obtained from
	//https://gist.github.com/t0chas/34afd1e4c9bc28649311
	//Though I have made a few changes
	//Currently only adding the Copy paste buttons at the top of OnInspectorGUI()
	//Removed animation on lists enabled/disabled
	//and a few other things

	//[CustomEditor(typeof(Object), true, isFallback = true), CanEditMultipleObjects]
	[CustomEditor(typeof(JMilesBehavior), true, isFallback = true), CanEditMultipleObjects]
	//JMilesBehavior
	public class CustomEditorBase : UnityEditor.Editor {
		private Dictionary<string, ReorderableListProperty> reorderableLists;
		private bool foldout;

		protected virtual void OnEnable() { reorderableLists = new Dictionary<string, ReorderableListProperty>(10); }

		~CustomEditorBase() {
			reorderableLists.Clear();
			reorderableLists = null;
		}

		public override void OnInspectorGUI() { DrawGUI(); }

		public void DrawGUI() {
			var changedObj = target;

			foldout = EditorGUILayout.Foldout(foldout, "Copy & Paste");
			if (foldout) {
				EditorGUILayout.BeginHorizontal("Box");
				serializedObject.Update();
				changedObj = EditorHelpers.CopyPastObjectButtons(changedObj);
				serializedObject.ApplyModifiedProperties();
				EditorGUILayout.EndHorizontal();
			}

			var cachedGuiColor = GUI.color;
			serializedObject.Update();
			var property = serializedObject.GetIterator();
			bool next = property.NextVisible(true);
			if (next)
				do {
					GUI.color = cachedGuiColor;
					HandleProperty(property);
				}
				while (property.NextVisible(false));
			serializedObject.ApplyModifiedProperties();
		}

		protected void HandleProperty(SerializedProperty property) {
			//Debug.LogFormat("name: {0}, displayName: {1}, type: {2}, propertyType: {3}, path: {4}", property.name, property.displayName, property.type, property.propertyType, property.propertyPath);
			bool isdefaultScriptProperty = property.name.Equals("m_Script") &&
										   property.type.Equals("PPtr<MonoScript>") &&
										   property.propertyType == SerializedPropertyType.ObjectReference &&
										   property.propertyPath.Equals("m_Script");
			bool cachedGUIEnabled = GUI.enabled;
			if (isdefaultScriptProperty) {
				GUI.enabled = false;
			}
			//var attr = this.GetPropertyAttributes(property);
			if (PropertyIsArrayAndNotString(property)) {
				HandleArray(property);
			}
			else EditorGUILayout.PropertyField(property, property.isExpanded);
			if (isdefaultScriptProperty) {
				GUI.enabled = cachedGUIEnabled;
			}
		}

		protected bool PropertyIsArrayAndNotString(SerializedProperty property) { return property.isArray && property.propertyType != SerializedPropertyType.String; }

		protected void HandleArray(SerializedProperty property) {
			var listData = GetReorderableList(property);
			if (property.isExpanded) listData.List.DoLayoutList();
			else
				property.isExpanded = EditorGUILayout.ToggleLeft(
					string.Format("{0}\t[{1}]", property.displayName, property.arraySize),
					property.isExpanded,
					EditorStyles.boldLabel);
		}

		protected object[] GetPropertyAttributes(SerializedProperty property) { return GetPropertyAttributes<PropertyAttribute>(property); }

		protected object[] GetPropertyAttributes<T>(SerializedProperty property) where T : System.Attribute {
			const BindingFlags bindingFlags = BindingFlags.GetField |
											  BindingFlags.GetProperty |
											  BindingFlags.IgnoreCase |
											  BindingFlags.Instance |
											  BindingFlags.NonPublic |
											  BindingFlags.Public;
			if (property.serializedObject.targetObject == null) return null;
			var targetType = property.serializedObject.targetObject.GetType();
			var field = targetType.GetField(property.name, bindingFlags);
			return field != null ? field.GetCustomAttributes(typeof(T), true) : null;
		}

		private ReorderableListProperty GetReorderableList(SerializedProperty property) {
			ReorderableListProperty ret;
			if (reorderableLists.TryGetValue(property.name, out ret)) {
				ret.Property = property;
				return ret;
			}
			ret = new ReorderableListProperty(property);
			reorderableLists.Add(property.name, ret);
			return ret;
		}

		#region Inner-class ReorderableListProperty
		private class ReorderableListProperty {
			/// <summary>
			/// ref http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
			/// </summary>
			public ReorderableList List { get; private set; }

			private SerializedProperty _property;

			public SerializedProperty Property {
				private get { return _property; }
				set {
					_property = value;
					List.serializedProperty = _property;
				}
			}

			public ReorderableListProperty(SerializedProperty property) {
				_property = property;
				CreateList();
			}

			~ReorderableListProperty() {
				_property = null;
				List = null;
			}

			private void CreateList() {
				const bool dragable = true, header = true, add = true, remove = true;
				List = new ReorderableList(Property.serializedObject, Property, dragable, header, add, remove);
				List.drawHeaderCallback += (rect) => {
					_property.isExpanded = EditorGUI.ToggleLeft(
						rect,
						string.Format("{0}\t[{1}]", _property.displayName, _property.arraySize),
						_property.isExpanded,
						EditorStyles.boldLabel);
				};
				List.onCanRemoveCallback += (list) => List.count > 0;
				List.drawElementCallback += DrawElement;
				List.elementHeightCallback += (index) => Mathf.Max(
															 EditorGUIUtility.singleLineHeight,
															 EditorGUI.GetPropertyHeight(_property.GetArrayElementAtIndex(index), GUIContent.none, true)) +
														 4.0f;
			}

			private void DrawElement(Rect rect, int index, bool active, bool focused) {
				rect.height = EditorGUI.GetPropertyHeight(_property.GetArrayElementAtIndex(index), GUIContent.none, true);
				rect.y += 1;
				EditorGUI.PropertyField(
					rect,
					_property.GetArrayElementAtIndex(index),
					_property.GetArrayElementAtIndex(index).propertyType == SerializedPropertyType.Generic ?
						new GUIContent(_property.GetArrayElementAtIndex(index).displayName) :
						GUIContent.none,
					true);
				List.elementHeight = rect.height + 4.0f;
			}
		}
		#endregion
	}

	[CustomEditor(typeof(JMilesScriptableObject), true, isFallback = true), CanEditMultipleObjects]
	public class CustomEditorBaseScriptableObject : CustomEditorBase {
	}
}