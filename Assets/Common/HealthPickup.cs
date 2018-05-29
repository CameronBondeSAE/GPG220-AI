using UnityEngine;
using System.Collections;

public class HealthPickup : PickupBase
{
	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);

		Health health = other.GetComponent<Health>();

		if (health)
		{
			health.Change(amount, gameObject);

			Destroy(gameObject);
		}
	}

}
