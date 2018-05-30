using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Health health;

    // Use this for initialization
    void Start()
    {
        health = GetComponent<Health>();

        health.OnDeathEvent += OnDeath;
    }

    private void OnDeath()
    {
        print("I'm dead");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
