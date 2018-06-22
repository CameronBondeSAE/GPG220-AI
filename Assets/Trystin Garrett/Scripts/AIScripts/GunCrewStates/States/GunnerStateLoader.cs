using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunnerStateLoader : State
    {
        GunCrewMember Owner;
        FieldGun FGRef;
        StateMachine SM;
        GunnerStateTable GST;

        public GSLoaderState CurrretState = GSLoaderState.Idle;
        float CheckInterval = 0.5f;
        float CurrentIntervalTime = 0;

        public override void OnStateEnter(GunCrewMember _CrewMember)
        {
            if(_CrewMember.OccupiedNode != FGRef.BreachNode)
            {
                Debug.Log("Moving To Breach");
                CurrretState = GSLoaderState.MovingToBreach;
                _CrewMember.MovementScript.RequestPath(_CrewMember.OccupiedNode, FGRef.BreachNode, Owner, true);
            }
            else
                CurrretState = GSLoaderState.AtBreach;
        }

        public override void OnStateExit(GunCrewMember _CrewMember)
        {
            ResetState();
        }

        public override void OnStateUpdate(GunCrewMember _CrewMember)
        {
            switch(CurrretState)
            {
                case GSLoaderState.Idle:

                    break;
                case GSLoaderState.MovingToBreach:

                    if(_CrewMember.OccupiedNode == FGRef.BreachNode)
                    {
                        Debug.Log("Arrived at Breach");
                        CurrretState = GSLoaderState.AtBreach;
                    }

                    //THIS COULD BE CHANGED TO WORK ON A EVENT BASIS RATHER THAN A CHECK... NEED TO CHANGE THE TIMING OF THINGS WITH SPAWNING... ACTIVATE THINGS DIFFERENTLY SO IT TURNS ON CREW AFTER SETUP IS DONE
                    f
                    //else
                    //    if (CurrentIntervalTime > CheckInterval)
                    //    {
                    //        Debug.Log("Checking Path To Breach");
                    //        _CrewMember.MovementScript.RequestPath(_CrewMember.OccupiedNode, FGRef.BreachNode, Owner, true);
                    //        CurrentIntervalTime = 0;
                    //    }
                    //    else
                    //        CurrentIntervalTime += Time.deltaTime;

                    break;
                case GSLoaderState.AtBreach:

                    break;
                case GSLoaderState.LoadingShell:

                    break;
                case GSLoaderState.LoadingComplete:

                    break;

            }


            if(_CrewMember.OccupiedNode == FGRef.BreachNode)
                CurrretState = GSLoaderState.AtBreach;
            else
            {

            }
        }

        public void StateSetup(StateMachine _SM)
        {
            SM = _SM;
            GST = (GunnerStateTable)_SM.LoadedStateTable;
            Owner = _SM.SMOwner;
            FGRef = Owner.OwnerGC.FieldGun;
        }

        void ResetState()
        {
            CurrentIntervalTime = 0;
            CurrretState = GSLoaderState.Idle;
        }
    }
}
