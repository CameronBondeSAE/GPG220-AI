using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class SpotterStateTable : StateTable
    {
        //Implment instantations of each state here.

        public override State ReturnState(object _StateEnum)
        {
            //Casting to an enum not the besting thing...
            SpotterSubRole State = (SpotterSubRole)_StateEnum;

            switch (State)
            {
                case SpotterSubRole.Idle:
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
