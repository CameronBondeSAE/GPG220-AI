using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cam
{
	public class Controller : ControllerBase
	{
		public Model model;

		public void Update()
		{
			if (Random.Range(0f, 10f) > 9.99f)
				model.Ability1();
			if (Random.Range(0f, 10f) > 9.99f)
				model.Ability2();
		}

	}

}
