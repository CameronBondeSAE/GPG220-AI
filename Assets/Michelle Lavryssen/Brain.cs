using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michelle
{
    public class Brain : CharacterBase
    {

        public bool isEnemyNear;
        public int health;
        public int energy;

        public override void Ability1()
        {
            base.Ability1();
            debugText = "Im siphoning health";
        }

        public override void Ability2()
        {
            base.Ability2();
            debugText = "I'm raising undead";
        }
        public override void Ability3()
        {
            base.Ability3();
            debugText = "I'm creating a bone shield between myself and the enemy";
        }

    }
}
    