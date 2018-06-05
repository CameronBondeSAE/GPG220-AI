using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    Health My_Health;
    Body My_Body;

	// Use this for initialization
	void Start ()
    {
        My_Health = GetComponent<Health>();
        My_Health.OnDeathEvent += Death;
        My_Body = GetComponent<Body>(); 
	}
	
	// Update is called once per frame

    private void Death()
    {
        My_Body.OnDeath();
    }

	void Update ()
    {
		
	}
}
