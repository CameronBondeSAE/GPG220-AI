using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempdestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (gameObject.GetComponent<Health>().Amount <= 0)
        {

            Destroy(gameObject);
        }

	}
}
