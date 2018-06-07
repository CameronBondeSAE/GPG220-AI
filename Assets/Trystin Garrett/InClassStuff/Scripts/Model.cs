using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class Model : CharacterBase
    {

        public override void Start()
        {
            Debug.Log("Spotter Here");
        }

        public override void Ability1()
        {
            Debug.Log("Obtain Knowledge of Local Area");
            Debug.Log("Finding Hiding Locations");
            Debug.Log("Searching for targets");
        }

        public override void Ability2()
        {
            Debug.Log("Call in Strike");
        }

        public override void Ability3()
        {
            Debug.Log("Replacing Gun Crew");
        }

        public override void OnDestroy()
        {
            Debug.Log("Spotter KIA");
        }
    }
}

