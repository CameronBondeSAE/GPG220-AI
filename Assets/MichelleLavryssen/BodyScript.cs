using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michelle
{
public class BodyScript : CharacterBase
{

        public override void Ability1()
        {
            base.Ability1();
            debugText = "I'm syphoning {Target}'s health!";
        }

        public override void Ability2()
        {
            base.Ability2();
            debugText = "I'm raising 5 undead!";
        }

        public override void Ability3()
        {
            base.Ability3();
            debugText = "I'm creating bone barriers to shield me!";
        }

    }
}
