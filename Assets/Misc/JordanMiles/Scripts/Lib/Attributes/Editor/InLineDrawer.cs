using JMiles42.Attributes;
using UnityEditor;
using UnityEngine;

namespace JMiles42.Editor.PropertyDrawers {
    [CustomPropertyDrawer(typeof(MultiInLineAttribute), true)]
    public class MultiInLineAttributeDrawer : JMilesPropertyDrawer<MultiInLineAttribute> {
        private MultiInLineAttribute MultiInLineAttribute {
            get { return (MultiInLineAttribute)attribute; }
        }

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            return MultiInLineAttribute.index == 0 ? singleLine : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
            var property = new EditorEntry(label.text, prop);
            position.x += 16;
            position.width -= 16;
            var pos = position;
            pos.width = position.width / MultiInLineAttribute.totalAmount;

            if (MultiInLineAttribute.index == 0) {
                property.Draw(pos);
                return;
            }

            pos.y -= (singleLine + (2 * MultiInLineAttribute.index));
            pos.height = singleLine;
            pos.x += (pos.width) * MultiInLineAttribute.index;
            if (MultiInLineAttribute.expandToWidth)
                pos.width = position.width - (pos.width) * MultiInLineAttribute.index;
            property.Draw(pos);
        }
    }
}