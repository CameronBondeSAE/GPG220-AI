using UnityEngine;

namespace JMiles42.Components {
    [RequireComponent(typeof(Rigidbody))]
    public class JMilesRigidbodyBehavior : JMilesBehavior {
        private Rigidbody m_Rigidbody;
        public Rigidbody Rigidbody {
            get {
                return m_Rigidbody ?? (m_Rigidbody = GetComponent<Rigidbody>());
            }
            set { m_Rigidbody = value; }
        }
    }
    [RequireComponent(typeof(Rigidbody2D))]
    public class JMiles2DRigidbodyBehavior : JMilesBehavior {
        private Rigidbody2D m_Rigidbody2D;
        public Rigidbody2D Rigidbody2D {
            get {
                return m_Rigidbody2D ?? (m_Rigidbody2D = GetComponent<Rigidbody2D>());
            }
            set { m_Rigidbody2D = value; }
        }
    }
}
