using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhys
{
   
    

    public class Controller : ControllerBase
    {
        public GameObject target;

        Health My_Health;
        Body My_Body;
        
        public Vector3 dir;

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
            Collisions();


        }

        private void Collisions()
        {
            dir = (target.transform.position - transform.position).normalized;
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, transform.forward, out hit, 20))
            {
                if (hit.transform != transform)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.blue);
                    dir += hit.normal * 100;
                }
            }
            Vector3 leftR = transform.position;
            Vector3 rightR = transform.position;

            leftR.x -= 5;
            rightR.x += 5;

            if (Physics.Raycast(leftR, transform.forward, out hit, 20))
            {
                if (hit.transform != transform)
                {
                    Debug.DrawLine(leftR, hit.point, Color.red);
                    dir += hit.normal * 100;
                }
            }

            if (Physics.Raycast(rightR, transform.forward, out hit, 20))
            {
                if (hit.transform != transform)
                {
                    Debug.DrawLine(rightR, hit.point, Color.yellow);
                    dir -= hit.normal * 50;
                }

            }

            My_Body.Movement(dir); 

        }
 
    }
}
