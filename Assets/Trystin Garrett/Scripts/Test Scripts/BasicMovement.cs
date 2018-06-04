using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour {

    [Range (0,1)]
    public float Speed;
	
	// Update is called once per frame
	void Update () {

        float H = Input.GetAxis("Horizontal");
        float V = Input.GetAxis("Vertical");

        Vector3 MovemntVector = new Vector3(H * Speed, 0f,V * Speed);
        transform.Translate(MovemntVector);
	}
}
