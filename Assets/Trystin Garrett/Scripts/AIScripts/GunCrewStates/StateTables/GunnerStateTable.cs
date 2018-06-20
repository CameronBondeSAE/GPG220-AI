using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunnerStateTable : StateTable
    {
        //Implment instantations of each state here.

        public override State ReturnState(object _StateEnum)
        {
            //Casting to an enum not the besting thing...
            GunnerState State = (GunnerState)_StateEnum;

            switch (State)
            {
                case GunnerState.Idle:
                    return null;
            }
            return NullState.Instance;
        }

        public override void SetUpStates(StateMachine _SM)
        {
            //Setup each state here
        }
    }
}
