using UnityEngine;
using System.Collections;

public class EnergyPickup : PickupBase
{
	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);


		if (!other.isTrigger)
		{

			if (other.GetComponent<Energy>())
			{
				other.GetComponent<Energy>().Change(amount);
				Destroy(gameObject);
			}
		}
	}
}
