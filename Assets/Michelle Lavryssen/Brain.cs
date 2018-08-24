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
        public GameObject Bonewall;
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
        public float BonewallTimer;
        public float Distance;
        public Vector3 TargetEnemyPos;
        public float retargetTimer;
        public GameObject standinobject;




        override public void Start()
        {

            defaultTarget = Instantiate(rightEye);
            MoveDefaultTarget();
            currentTarget = defaultTarget;
            rigid = GetComponent<Rigidbody>();
            BonewallTimer = 0;

        }

        void Update()
        {
            myPosition = transform.position;
            BonewallTimer -= Time.deltaTime;
            if (targetEnemy != null)
            {
                TargetEnemyPos = targetEnemy.transform.position;
            }
            retargetTimer -= Time.deltaTime;
            Distance = Vector3.Distance(TargetEnemyPos, myPosition);
            if (Vector3.Distance(myPosition, defaultTarget.transform.position) < 5f)
            {
                MoveDefaultTarget();

            }

            if (currentTarget == null)
            {
                currentTarget = defaultTarget;
            }

            if (isEnemyNear == true)

            {
                if (gameObject.GetComponent<Health>().Amount <= 50 && gameObject.GetComponent<Energy>().Amount >= 25)
                {
                    Ability1(); // siphon health

                }
                if (gameObject.GetComponent<Health>().Amount >= 50 && gameObject.GetComponent<Energy>().Amount > 70)
                {

                    Ability2(); // spawn undead

                }
                if (gameObject.GetComponent<Health>().Amount <= 25 && gameObject.GetComponent<Energy>().Amount >= 10)
                {
                    if (BonewallTimer <= 0.9)
                    {
                        Ability3(); //make bone wall and flee
                    }
                }
            }

        }

        private void FixedUpdate()
        {
            Collisions();
            if (targetEnemy != null)
            {
                if (targetEnemy.GetComponent<Health>().Amount > 0 && retargetTimer <= 0)
                {
                    currentTarget = targetEnemy;
                    retargetTimer = 10;
                }
            }


        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name != "Necromancer" && other.gameObject.name != "undead(Clone)" && other.gameObject.name != "Bonewall(Clone)" && other.gameObject.name != "Right(Clone)")
            {
                if (retargetTimer <= 0)
                {
                    targetEnemy = other.gameObject;
                    isEnemyNear = true;
                    currentTarget = other.gameObject;
                    retargetTimer = 10;
                }
            }
            if (targetEnemy == null && other.gameObject.name == "Right(Clone)")
            {
                currentTarget = other.gameObject;
            }

        }
        public void OnTriggerStay(Collider other)
        {
            if (other.gameObject.name != "Necromancer" && other.gameObject.name != "undead(Clone)" && other.gameObject.name != "Bonewall(Clone)" && other.gameObject.name != "Right(Clone)")
            {

                isEnemyNear = true;
                if (targetEnemy == null && retargetTimer <= 0)
                {
                    targetEnemy = other.gameObject;

                }

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
            print("Im siphoning health");
            SiphonHealth();
        }

        public override void Ability2()
        {
            base.Ability2();
            print("Im raising undead");
            instantiateUndead();


        }

        public override void Ability3()
        {
            base.Ability3();
            print("Im raising a bone wall and running away");
            Flee();
        }



        public void instantiateUndead()
        {

            for (int i = 0; i < 5; i++)
            {


                // Vector3 UndeadSpawnPosition = new Vector3(myPosition.x + Random.Range(-0.5f, 0.5f), myPosition.y, myPosition.z + Random.Range(-0.5f, 0.5f));
                Vector3 UndeadSpawnPosition;
                UndeadSpawnPosition.x = myPosition.x + ((TargetEnemyPos.x - myPosition.x) / 2);
                UndeadSpawnPosition.y = myPosition.y;
                UndeadSpawnPosition.z = myPosition.z + ((TargetEnemyPos.z - myPosition.z) / 2);

                UndeadSpawnPosition.x += Random.Range(-0.5f, 0.5f);
                UndeadSpawnPosition.y += Random.Range(-0.5f, 0.5f);
                UndeadSpawnPosition.z += Random.Range(-0.5f, 0.5f);

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

        public void Flee()
        {

            //this makes the wall

            //Vector3 Direction = TargetEnemyPos - myPosition;
            Vector3 wallSpawnPosition;

            wallSpawnPosition.x = myPosition.x + ((TargetEnemyPos.x - myPosition.x) / 2);
            wallSpawnPosition.y = myPosition.y + ((TargetEnemyPos.y - myPosition.y) / 2);
            wallSpawnPosition.z = myPosition.z + ((TargetEnemyPos.z - myPosition.z) / 2);

            //Vector3 BonewallSpawnPosition = (Direction / Distance) * (Distance / 2);

            GameObject NecroWall = Instantiate(Bonewall, wallSpawnPosition, Quaternion.identity);
            NecroWall.transform.LookAt(targetEnemy.transform);
            BonewallTimer = 60;

            gameObject.GetComponent<Energy>().Amount -= 10;


            //this makes him flee
            Vector3 fleePosition;

            fleePosition.x = myPosition.x + ((TargetEnemyPos.x - myPosition.x) * -2);
            fleePosition.y = 0;
            fleePosition.z = myPosition.z + ((TargetEnemyPos.z - myPosition.z) * -2);
            defaultTarget.transform.position = fleePosition;

            GameObject standin = Instantiate(standinobject, new Vector3(0, 0, 0), Quaternion.identity);
            targetEnemy = standin;
            Destroy(standin);
            currentTarget = defaultTarget;
        }


        public void MoveDefaultTarget()
        {
            defaultTarget.transform.position = new Vector3(Random.Range(2, 300), 0, Random.Range(2, 300));

        }

    }
}
