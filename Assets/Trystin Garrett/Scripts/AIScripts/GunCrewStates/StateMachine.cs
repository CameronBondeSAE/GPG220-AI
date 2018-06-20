using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class StateMachine
    {
        //ooo AutoProperty.... Do they Work?
        public GunCrewMember SMOwner { get; private set; }

        public StateTable LoadedStateTable = NullStateTable.Instance;
        private SpotterStateTable SpotterStateTable;
        private GunnerStateTable GunnerStateTable;


        State CurrentState = NullState.Instance;
        State PreviousState = NullState.Instance;
        public State GetCurrentState { get { return CurrentState; } }
        public State GetPreviousState { get { return PreviousState; } }

        public void UpdateSM()
        {
            CurrentState.OnStateUpdate(SMOwner);
        }

        //
        public void ChangeState(State _ChangeToState)
        {
            PreviousState = CurrentState;
            CurrentState.OnStateExit(SMOwner);
            CurrentState = _ChangeToState;
            CurrentState.OnStateEnter(SMOwner);
        }

        //
        public void RevertToPreviousState()
        {
            ChangeState(PreviousState);
        }

        //
        public void SetupStateMachine(GunCrewMember _SMOwner)
        {
            SMOwner = _SMOwner;
            switch (SMOwner.AITableType)
            {
                case StateTableType.Null:

                    break;
                case StateTableType.Spotter:
                    LoadedStateTable = SpotterStateTable;
                    SpotterStateTable.SetUpStates(this);
                    ChangeState(LoadedStateTable.ReturnState(null));
                    break;
                case StateTableType.Gunner:
                    LoadedStateTable = GunnerStateTable;
                    GunnerStateTable.SetUpStates(this);
                    ChangeState(LoadedStateTable.ReturnState(null));
                    break;
            }

        }
    }
}
