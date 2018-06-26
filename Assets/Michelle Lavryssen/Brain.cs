using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michelle
{
	public class Brain : CharacterBase
	{

		public bool isEnemyNear;
		public GameObject Necromancer;
		public GameObject[] undead;
		public Vector3 myPosition;


		void Update ()
		{
			myPosition = GameObject.Find ("Necromancer").transform.position;

		}

		public override void Ability1 ()
		{
			base.Ability1 ();
			debugText = "Im siphoning health";
		}

		public override void Ability2 ()
		{
			base.Ability2 ();
			debugText = "I'm raising undead";
			instantiateUndead ();


		}

		public override void Ability3 ()
		{
			base.Ability3 ();
			debugText = "I'm creating a bone shield between myself and the enemy";
		}
			

		public void instantiateUndead ()
		{
			GameObject[] clone = undead;
			Vector3 UndeadSpawnPosition = new Vector3 (myPosition.x + Random.Range (-10f, 10f), myPosition.y, myPosition.z + Random.Range (-10f, 10f));
			Instantiate (clone [0], UndeadSpawnPosition, Quaternion.identity);
			Instantiate (clone [1], UndeadSpawnPosition, Quaternion.identity);
			Instantiate (clone [2], UndeadSpawnPosition, Quaternion.identity);
			Instantiate (clone [3], UndeadSpawnPosition, Quaternion.identity);
		Instantiate (clone [4], UndeadSpawnPosition, Quaternion.identity);
			//instantiates undead minions and then destroys them after 10 seconds
		}
			
		

		}
	}
	  