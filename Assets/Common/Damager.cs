using UnityEngine;
using System.Collections;

public class Damager : MonoBehaviour
{
	public float scalar = 1;

	public void OnTriggerStay(Collider other)
	{
		var health = other.gameObject.GetComponent<Health>();

		if (health)
		{
			health.Change(-Time.deltaTime * scalar, gameObject);
		}
	}

}
