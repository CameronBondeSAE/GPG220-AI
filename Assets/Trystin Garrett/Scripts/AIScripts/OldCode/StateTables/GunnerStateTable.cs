using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunnerStateTable : StateTable
    {
        //Implment instantations of each state here.
        GunnerStateLoader GSL = new GunnerStateLoader();


        public override State ReturnState(object _StateEnum)
        {
            //Casting to an enum not the besting thing...
            //GunnerSubRole State = (GunnerSubRole)_StateEnum;

            //switch (State)
            //{
            //    case GunnerSubRole.Idle:
            //        return null;
            //    case GunnerSubRole.Loader:
            //        return GSL;
            //    case GunnerSubRole.Lookout:

            //        break;
            //}
            return NullState.Instance;
        }

        public override void SetUpStates(StateMachine _SM)
        {
            //Setup each state here
            GSL.StateSetup(_SM);
        }
    }
}
