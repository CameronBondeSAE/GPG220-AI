using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class NullStateTable : StateTable
    {
        public static NullStateTable Instance = new NullStateTable();

        public override State ReturnState(object _StateEnum)
        {
            return NullState.Instance;
        }

        public override void SetUpStates(StateMachine _SM)
        {

        }
    }
}
