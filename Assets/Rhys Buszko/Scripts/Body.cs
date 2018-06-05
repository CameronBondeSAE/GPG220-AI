﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : CharacterBase {

    public override void Ability1()
    {
        base.Ability1();
        gameObject.transform.position = gameObject.transform.position + new Vector3(5, 0, 0);
        print("I Teleported");
        debugText = "I Teleported";
    }

    public override void Ability2()
    {
        base.Ability2();
        print("I'm Eatting");
        debugText = "I'm Eatting";
    }

    public override void Ability3()
    {
        base.Ability3();
        print("Corpse Attack");
        debugText = "I'm using big attack";
    }


    public void OnDeath()
    {
        print("I Died");
        debugText = "I Died";
    }

}
