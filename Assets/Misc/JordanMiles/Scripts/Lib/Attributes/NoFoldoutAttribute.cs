using UnityEngine;

namespace JMiles42.Attributes
{
    public class NoFoldoutAttribute : PropertyAttribute {
        public bool ShowVariableName { get; private set; }

        public NoFoldoutAttribute(bool showName = false) {
            ShowVariableName = showName;
        }
    }

    public class NoFoldoutClass {
    }
}
