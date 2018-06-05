using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhys
{
    public class Controller : ControllerBase
    {

        Health My_Health;
        Body My_Body;

        // Use this for initialization
        void Start()
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

        private void OnTriggerEnter(Collider other)
        {
            
        }

        private void OnTriggerExit(Collider other)
        {
            
        }

        void Update()
        {

        }
    }
}
