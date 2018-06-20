using UnityEngine;
using System.Collections;

public class DeadbodyPickup : PickupBase
{
	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);

		// Do nothing because this is a special case when the character needs to decide to pick it up
		// TODO: Implement a generic function that can test itself for deletion
	}
}
