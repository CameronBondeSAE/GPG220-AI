using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunCrewMember : CharacterBase
    {
        public enum GunCrewRole
        {
            Spotter,
            GunOperator
        }

        public GunCrew OwnerGC;
        public TestMovementAI MovementScript;
        public TestVisionAI VistionScript;
        

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }




        public void SetupGunCrewMember(GunCrew _OwnerGC)
        {
            MovementScript = GetComponent<TestMovementAI>();
            VistionScript = GetComponent<TestVisionAI>();
            OwnerGC = _OwnerGC;
        }
    }
}
