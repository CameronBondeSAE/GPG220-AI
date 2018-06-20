using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class NullState : State
    {
        public static NullState Instance = new NullState();

        public override void OnStateEnter(GunCrewMember _CrewMember)
        {

        }

        public override void OnStateExit(GunCrewMember _CrewMember)
        {

        }

        public override void OnStateUpdate(GunCrewMember _CrewMember)
        {

        }
    }
}