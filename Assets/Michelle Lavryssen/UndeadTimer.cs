using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Remoting.Messaging;


public class UndeadTimer : CharacterBase
{
    public bool isEnemyNear;
    public float targetTime;
    public GameObject necro;
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
    public float attacktimer;


    // Use this for initialization
    public void randomizePositions()
    {
        gameObject.transform.position += new Vector3(Random.Range(necro.transform.position.x - 0.5f, necro.transform.position.x + 0.5f), 0, Random.Range(necro.transform.position.z - 0.5f, necro.transform.position.z + 0.5f));
    }

    void Start()
    {
        defaultTarget = Instantiate(rightEye);
        defaultTarget.transform.position = new Vector3(Random.Range(2, 300), 1, Random.Range(2, 300));
        currentTarget = defaultTarget;
        rigid = GetComponent<Rigidbody>();
        randomizePositions();
        currentTarget = necro;
        attacktimer = 5;


    }

    // Update is called once per frame
    void Update()
    {
        myPosition = transform.position;
        targetTime -= Time.deltaTime;
        attacktimer -= Time.deltaTime;
        if (targetEnemy != null)
        {
            currentTarget = targetEnemy;

        }

        if (targetTime <= 0)
        {

            Destroy(defaultTarget, 1);
            Destroy(gameObject, 1);


        }
       
    }

    public void FixedUpdate()
    {

        Collisions();

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Necromancer" && other.gameObject.name != "undead(Clone)")
        {
            if (other.gameObject == targetEnemy)
            {
                Ability1();

            }

        }
    }



    public override void Ability1()
    {
        base.Ability1();
        print("im attacking" + targetEnemy);

        if (attacktimer <= 0)
        {
            baseattack();
            attacktimer = 5;
        }
    }

    public void baseattack()
    {
        currentTarget.GetComponent<Health>().Amount -= 5;
    }




}



