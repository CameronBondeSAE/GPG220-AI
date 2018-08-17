using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : CharacterBase {

    public float Speed = 10.0f;


    

    public override void Ability1()
    {
        base.Ability1();
        print("I'm Shooting");
        debugText = "I'm Shooting";
    }

    public override void Ability2()
    {
        base.Ability2();
        gameObject.transform.position = gameObject.transform.position + new Vector3(5, 0, 0);
        print("I Teleported");
        debugText = "I Teleported";
    }

    public override void Ability3()
    {
        base.Ability3();
        print("I'm Eatting");
        debugText = "I'm Eatting";
    }

    public override void Ability4()
    {
        base.Ability4();
        print("Corpse Attack");
        debugText = "I'm using big attack";
    }


    public void OnDeath()
    {
        print("I Died");
        debugText = "I Died";
        Destroy(gameObject);
    }

    public void Movement(Vector3 dir, float multiplier, Rigidbody Rigid)
    {
        Quaternion rot = Quaternion.LookRotation(dir);
        
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8);
        Rigid.velocity = transform.forward * (Speed * multiplier);

    }

}
