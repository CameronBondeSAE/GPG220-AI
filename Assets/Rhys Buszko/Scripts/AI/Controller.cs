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

        int damage = 5;

        public Manager manager;
        public bool attack, run, wander;

        List<Collider> Enemys = new List<Collider>();

        Health My_Health;
        Body My_Body;
        public Pathfinding My_Path;

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

            Defult.transform.position = new Vector3(Random.Range(-manager.gridWorldSize.x, manager.gridWorldSize.x), 1, Random.Range(-manager.gridWorldSize.y, manager.gridWorldSize.y));

            target = Defult;

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

            Retarget();

            if(attack == true)
            {
                My_Body.Ability1();
                target.GetComponent<Health>().Change(damage, My_Body);
            }

        }

        private void FixedUpdate()
        {
            Collisions();
            if (target == Defult)
            {
                float mesure = Mathf.Infinity;
                if (My_Path.foundpath == null)
                {
                    My_Path.FindPath(transform.position, target.transform.position);
                }
                else if (My_Path.foundpath.Count <= 1)
                {
                    Defult.transform.position = new Vector3(Random.Range(-manager.gridWorldSize.x, manager.gridWorldSize.x), 1, Random.Range(-manager.gridWorldSize.y, manager.gridWorldSize.y));
                    My_Path.FindPath(transform.position, target.transform.position);
                }
                else
                {
                    mesure = Vector3.Distance(transform.position, My_Path.foundpath[0].worldPosition);
                    Defult.transform.position = My_Path.foundpath[0].worldPosition;
                }

                if (mesure <= 1)
                {
                    My_Path.foundpath.RemoveAt(0);
                }
            }
        }

        private void Collisions()
        {
            dir = (target.transform.position - transform.position).normalized;
            Speed_Mult = 1;

            if (Physics.Raycast(transform.position, transform.forward, out hitForward, 2, 1, QueryTriggerInteraction.Ignore))
            {
                if (hitForward.transform != transform)
                {
                    Debug.DrawLine(transform.position, hitForward.point, Color.blue);
                    dir += hitForward.normal * 200;
                    Speed_Mult = 0.5f;
                }
            }


            if (Physics.Raycast(transform.position, LeftEye.transform.forward, out hitLeft, 1, 1, QueryTriggerInteraction.Ignore))
            {

                if (hitLeft.transform != transform)
                {
                    Debug.DrawLine(transform.position, hitLeft.point, Color.red);
                    dir += hitLeft.normal * 50;
                    Speed_Mult = 1;
                }
            }

            if (Physics.Raycast(transform.position, RightEye.transform.forward, out hitRight, 1, 1, QueryTriggerInteraction.Ignore))
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
   

        public void Wander()
        {
            
        }

        public void Retarget()
        {

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
                            attack = true;
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

        public void OnDrawGizmos()
        {
            if(My_Path == null)
                return;

            if (My_Path.foundpath == null)
                return;

            for (int PathIndex = 0; PathIndex < My_Path.foundpath.Count; PathIndex++)
            {
                Gizmos.DrawSphere(My_Path.foundpath[PathIndex].worldPosition, 0.5f);

            }
        }

    }
}
