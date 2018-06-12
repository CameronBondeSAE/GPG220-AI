using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cam
{


	public class Model : CharacterBase
	{
		public bool IsEnemyAngry { get; set; }

		public override void Ability1()
		{
			base.Ability1();

			debugText = "Ability1";
		}

		public bool IsEnemyNear()
		{
			return true;
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}

}
