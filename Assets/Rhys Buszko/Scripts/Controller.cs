using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Rhys
{
   
    

    public class Controller : ControllerBase
    {
        public GameObject target;
        public GameObject LeftEye;
        public GameObject RightEye;

        public RaycastHit hitForward;
        public RaycastHit hitLeft;
        public RaycastHit hitRight;

        Health My_Health;
        Body My_Body;

        public float Speed_Mult = 1;
        public Vector3 dir;

        // Use this for initialization
        void Start()
        {
            My_Health = GetComponent<Health>();
            My_Health.OnDeathEvent += Death;
            My_Body = GetComponent<Body>();
            My_Body.Initalize_Me();
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
            Speed_Mult = 1;

            if (Physics.Raycast(transform.position, transform.forward, out hitForward, 20))
            {
                if (hitForward.transform != transform)
                {
                    Debug.DrawLine(transform.position, hitForward.point, Color.blue);
                    dir += hitForward.normal * 200;
                    Speed_Mult = 0.5f;
                }
            }


            if (Physics.Raycast(transform.position, LeftEye.transform.forward, out hitLeft, 30))
            {
               
                if (hitLeft.transform != transform)
                {
                    Debug.DrawLine(transform.position, hitLeft.point, Color.red);
                    dir += -hitLeft.normal * 50;
                    Speed_Mult = 1;
                }
            }

            if (Physics.Raycast(transform.position, RightEye.transform.forward, out hitRight, 30))
            {
                if (hitRight.transform != transform)
                {
                    Debug.DrawLine(transform.position, hitRight.point, Color.yellow);
                    dir += hitRight.normal * 50;
                    Speed_Mult = 1;
                }

            }

            My_Body.Movement(dir, Speed_Mult); 

        }

        public Vector3 Direction(RaycastHit forward, RaycastHit right, RaycastHit left)
        {
            Vector3 direction = new Vector3(0,0,0);

            


           return direction;


        }
 
    }
}
