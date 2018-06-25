using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public abstract class StateTable
    {
        //protected StateTableType StateTableType;
        public static State NullStateInstance = NullState.Instance;

        //public StateTableType GetStateTableType()
        //{
        //    return StateTableType;
        //}

        public abstract void SetUpStates(StateMachine _SM);

        public abstract State ReturnState(object _StateEnum);
    }
}
