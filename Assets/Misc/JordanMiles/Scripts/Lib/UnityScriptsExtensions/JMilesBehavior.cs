using UnityEngine;


namespace JMiles42.Components {
        public class JMilesBehavior : MonoBehaviour {
        private Transform m_Transform;

        public Transform Transform {
            get { return m_Transform ?? (m_Transform = GetComponent<Transform>()); }
            set { m_Transform = value; }
        }

        public Quaternion Rotation {
            get { return Transform.rotation; }
            set { Transform.rotation = value; }
        }

        public Vector3 Position {
            get { return Transform.position; }
            set { Transform.position = value; }
        }
    }

    public class JMiles2DBehavior : MonoBehaviour {
        private RectTransform m_RectTransform;

        public RectTransform Transform {
            get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
            set { m_RectTransform = value; }
        }

        public Quaternion Rotation {
            get { return Transform.rotation; }
            set { Transform.rotation = value; }
        }

        public Vector3 Position {
            get { return Transform.position; }
            set { Transform.position = value; }
        }
    }
}