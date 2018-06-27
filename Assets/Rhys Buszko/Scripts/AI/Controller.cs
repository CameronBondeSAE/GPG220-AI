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
        public GameObject Defult;

        public RaycastHit hitForward;
        public RaycastHit hitLeft;
        public RaycastHit hitRight;

        public Manager manager;

        List<Collider> Enemys;

        Health My_Health;
        Body My_Body;
        private Pathfinding My_Path;

        public float Speed_Mult = 1;
        public Vector3 dir;

        // Use this for initialization
        void Start()
        {
            My_Health = GetComponent<Health>();
            My_Health.OnDeathEvent += Death;
            My_Body = GetComponent<Body>();
            My_Body.Initalize_Me();
            manager = Manager.Instance;
            My_Path = GetComponent<Pathfinding>();

            Defult = Instantiate(RightEye);


        }

        // Update is called once per frame

        private void Death()
        {
            My_Body.OnDeath();
        }

        private void OnTriggerEnter(Collider other)
        {
           
      
            if (other.gameObject.GetComponent<Health>() != null)
            {
                if (other != null)
                {
                   
                        Enemys.Add(other);
                    
                }
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (Enemys != null)
            {
                foreach (Collider n in Enemys)
                {
                    if (n == other)
                    {
                        Enemys.Remove(n);
                    }
                }

                if (target == other.gameObject)
                {
                    target = null;
                }
            }
        }

        

        void Update()
        {
            if(target == null)
            {
                target = Defult;
            }

            Collisions();
        }

        private void FixedUpdate()
        {
            Retarget();
        }

        private void Collisions()
        {
            dir = (target.transform.position - transform.position).normalized;
            Speed_Mult = 1;

            if (Physics.Raycast(transform.position, transform.forward, out hitForward, 5, 1, QueryTriggerInteraction.Ignore))
            {
                if (hitForward.transform != transform)
                {
                    Debug.DrawLine(transform.position, hitForward.point, Color.blue);
                    dir += hitForward.normal * 200;
                    Speed_Mult = 0.5f;
                }
            }


            if (Physics.Raycast(transform.position, LeftEye.transform.forward, out hitLeft, 7, 1, QueryTriggerInteraction.Ignore))
            {

                if (hitLeft.transform != transform)
                {
                    Debug.DrawLine(transform.position, hitLeft.point, Color.red);
                    dir += hitLeft.normal * 50;
                    Speed_Mult = 1;
                }
            }

            if (Physics.Raycast(transform.position, RightEye.transform.forward, out hitRight, 5, 1, QueryTriggerInteraction.Ignore))
            {
                if (hitRight.transform != transform)
                {
                    Debug.DrawLine(transform.position, hitRight.point, Color.yellow);
                    dir += hitRight.normal * 50;
                    Speed_Mult = 1;
                }

            }
            
            Direction(hitForward, hitRight, hitLeft);

            My_Body.Movement(dir, Speed_Mult);


        }
    

        public Vector3 Direction(RaycastHit forward, RaycastHit right, RaycastHit left)
        {
            Vector3 direction = new Vector3(0,0,0);
            Vector3 m_Size, m_Min, m_Max;

            List<RaycastHit> hit = new List<RaycastHit>
            {
                forward,
                right,
                left
            };

            foreach(RaycastHit n in hit)
            {
                if(n.collider != null)
                {
                    direction = n.collider.bounds.center;
                    m_Size = n.collider.bounds.size;
                    m_Min = n.collider.bounds.min;
                    m_Max = n.collider.bounds.max;
                    //OutputData(direction, m_Size, m_Min, m_Max);
                   // Debug.Log("Hit Local : " + n.point);                 
                }
            }

            return direction;


        }


        void OutputData(Vector3 m_Center, Vector3 m_Size, Vector3 m_Min, Vector3 m_Max)
        {
            //Output to the console the center and size of the Collider volume
            Debug.Log("Collider Center : " + m_Center);
            Debug.Log("Collider Size : " + m_Size);
            Debug.Log("Collider bound Minimum : " + m_Min);
            Debug.Log("Collider bound Maximum : " + m_Max);
        }

        public void Wander()
        {
            
        }

        public void Retarget()
        {
            //My_Path.FindPath(transform.position,target.transform.position);
            float dist = Mathf.Infinity;
            if (Enemys != null)
            {
                foreach (Collider n in Enemys)
                {
                    if (n != null)
                    {
                        float mesure = Vector3.Distance(transform.position, n.transform.position);
                        if (mesure < dist)
                        {
                            target = n.gameObject;
                        }
                    }
                }
            }

            if(target == null)
            {
                target = Defult;
            }

        }

        public void Attack()
        {

        }

        public void Run()
        {

        }

    }
}
