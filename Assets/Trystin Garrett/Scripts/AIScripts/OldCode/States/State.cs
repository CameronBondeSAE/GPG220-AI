using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public abstract class State
    {

        ///Might be a good idea to create a sate manager that allows you to create each entities states to use so you can store variables.
        public abstract void OnStateEnter(GunCrewMember _CrewMember);

        public abstract void OnStateUpdate(GunCrewMember _CrewMember);

        public abstract void OnStateExit(GunCrewMember _CrewMember);
    }
}
