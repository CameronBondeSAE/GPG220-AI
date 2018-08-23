using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Michelle
{
    public class Brain : CharacterBase
    {

        public bool isEnemyNear;
        public GameObject Necromancer;
        public GameObject undead;
        public Vector3 myPosition;
        public GameObject rightEye;
        public GameObject leftEye;
        public RaycastHit forward;
        public RaycastHit left;
        public RaycastHit right;
        public Vector3 direction;
        public GameObject defaultTarget;
        public GameObject currentTarget;
        public Rigidbody rigid;
        public GameObject targetEnemy;
        public float syphonAmount;
  



        override public void Start()
        {
            defaultTarget = Instantiate(rightEye);
            defaultTarget.transform.position = new Vector3(Random.Range(2, 300), 1, Random.Range(2, 300));
            currentTarget = defaultTarget;
            rigid = GetComponent<Rigidbody>();
   
        }


        void Update()
        {
            myPosition = transform.position;

            if (isEnemyNear == true)

            {
                if (gameObject.GetComponent<Health>().Amount <= 50 && gameObject.GetComponent<Energy>().Amount >= 25)
                {
                    Ability1(); // siphon health
                    
                }
                if (gameObject.GetComponent<Health>().Amount > 50 && gameObject.GetComponent<Energy>().Amount > 70)
                {

                    Ability2(); // spawn undead
                    
                }
                if (gameObject.GetComponent<Health>().Amount <= 25 && gameObject.GetComponent<Energy>().Amount >= 10)
                {
                    Ability3(); //make bone wall and flee
                    
                }
            }

        }

        private void FixedUpdate()
        {
            Collisions();


        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name != "Necromancer" && other.gameObject.name != "undead(Clone)")
            {
                targetEnemy = other.gameObject;
                isEnemyNear = true;
                currentTarget = other.gameObject;
            }
        }
        public void OnTriggerExit(Collider other)
        {
            isEnemyNear = false;
            currentTarget = defaultTarget;
        }

        public void Collisions()
        {

            direction = (currentTarget.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, transform.forward, out forward, 10, 1))
            {
                if (forward.transform != transform)
                {

                    Debug.DrawLine(transform.position, forward.point, Color.red);
                    direction += forward.normal * 200;
                }
            }
            if (Physics.Raycast(transform.position, leftEye.transform.forward, out left, 10, 1))
            {
                if (left.transform != transform)
                {

                    Debug.DrawLine(transform.position, left.point, Color.red);
                    direction += left.normal * 50;
                }
            }
            if (Physics.Raycast(transform.position, rightEye.transform.forward, out right, 10, 1))
            {
                if (right.transform != transform)
                {

                    Debug.DrawLine(transform.position, right.point, Color.red);
                    direction += right.normal * 50;
                }
            }



            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8);

            rigid.velocity = transform.forward * 5;





        }

        public override void Ability1()
        {
            base.Ability1();
            print ("Im siphoning health");
            SiphonHealth();
        }

        public override void Ability2()
        {
            base.Ability2();
            print ("Im raising undead");
            instantiateUndead();


        }

        public override void Ability3()
        {
            base.Ability3();
            print ("Im raising a bone wall and running away");
        }



        public void instantiateUndead()
        {

            for (int i = 0; i < 5; i++)
            {
                Vector3 UndeadSpawnPosition = new Vector3(myPosition.x + Random.Range(-0.5f, 0.5f), myPosition.y, myPosition.z + Random.Range(-0.5f, 0.5f));
                GameObject clone = Instantiate(undead, UndeadSpawnPosition, Quaternion.identity);
                clone.GetComponent<UndeadTimer>().necro = gameObject;
                clone.GetComponent<UndeadTimer>().targetEnemy = targetEnemy;
            }

            //instantiates undead minions and then destroys them after 10 seconds

            gameObject.GetComponent<Energy>().Amount -= 70;
        }

        public void SiphonHealth()
        {
            gameObject.GetComponent<Energy>().Amount -= 25;
            targetEnemy.GetComponent<Health>().Amount -= syphonAmount;
            gameObject.GetComponent<Health>().Amount += syphonAmount;
            

        }


    }
}
