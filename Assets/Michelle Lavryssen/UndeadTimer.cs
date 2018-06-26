using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadTimer : MonoBehaviour {
	public float targetTime;
	// Use this for initialization
	public void randomizePositions()
	{
		gameObject.transform.position += new Vector3(Random.Range (-10f, 10f), 0, Random.Range (-10f, 10f));
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
