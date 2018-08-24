using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bonewall : MonoBehaviour {

    public float lifetime;

	// Use this for initialization
	void Start () {
        lifetime = 10;
	}
	
	// Update is called once per frame
	void Update () {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {

            Destroy(gameObject);
        }
        if (gameObject.GetComponent<Health>().Amount <= 0)

        {
            Destroy(gameObject);

        }


	}
}
