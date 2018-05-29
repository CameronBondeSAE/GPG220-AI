using UnityEngine;

namespace JMiles42.Editor.PropertyDrawers
{
    public class JMilesPropertyDrawer : UnityEditor.PropertyDrawer{
        public const float singleLine = 16f;
        public const float indentSize = 16f;
        public const float lineGap = 2f;
    }
    public class JMilesPropertyDrawer<T> : JMilesPropertyDrawer where T : PropertyAttribute{
        public T GetAttribute{
            get { return (T)attribute; }
        }
    }
}
