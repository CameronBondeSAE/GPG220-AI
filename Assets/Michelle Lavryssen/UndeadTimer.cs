using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Remoting.Messaging;


public class UndeadTimer : MonoBehaviour {
	public float targetTime;
    public GameObject necro;

	// Use this for initialization
	public void randomizePositions()
	{
		gameObject.transform.position += new Vector3(Random.Range (necro.transform.position.x - 0.5f, necro.transform.position.x + 0.5f), 0, Random.Range (necro.transform.position.z - 0.5f, necro.transform.position.z + 0.5f));
	}

	void Start ()
	{
		randomizePositions ();
	}
	
	// Update is called once per frame
	void Update ()
	{

		targetTime -= Time.deltaTime;

	}
		

}
