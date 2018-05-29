using UnityEngine;

namespace JMiles42.BaseClasses {
	public class JMilesBehavior : CharacterBase {
		private Transform m_Transform;

		public Transform Transform {
			get { return m_Transform ?? (m_Transform = GetComponent<Transform>()); }
			set { m_Transform = value; }
		}
		private Rigidbody m_Rigidbody;
		public Rigidbody Rigidbody
		{

			get { return m_Rigidbody ?? (m_Rigidbody = GetComponent<Rigidbody>()); }
			set { m_Rigidbody = value; }
		}

		public Vector3 Velocity {
			get { return Rigidbody.velocity; }
			set { Rigidbody.velocity = value; }
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

	public class JMilesScriptableObject : ScriptableObject {
		//THIS ONLY EXISTS so that I can override the editor GUI
	}
}