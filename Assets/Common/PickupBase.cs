using System;
using UnityEngine;
using System.Collections;

public class PickupBase : MonoBehaviour
{
	public float amount;
	private RaycastHit hitInfo;
	public event Action OnPickedup;
	public event Action<PickupBase> OnPickedupBase;

	public void Start()
	{
		if (Physics.Raycast(new Ray(transform.position, Vector3.down), out hitInfo))
		{
			transform.position = hitInfo.point;
		}
	}

	public virtual void OnTriggerEnter(Collider other)
	{

        if (!other.isTrigger)
        {
            if (OnPickedup != null) OnPickedup();
            if (OnPickedupBase != null) OnPickedupBase(this);
        }

    }
}
