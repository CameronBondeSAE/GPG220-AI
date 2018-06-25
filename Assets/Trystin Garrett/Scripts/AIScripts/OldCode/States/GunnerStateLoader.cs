using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunnerStateLoader : State
    {
        //GunCrewMember Owner;
        //FieldGun FGRef;
        ////StateMachine SM;
        //GunnerStateTable GST;

        public GunLoaderState CurrretState = GunLoaderState.Idle;
        public bool HasPathBeenRequested = false;
        //float CheckInterval = 0.5f;
        //float CurrentIntervalTime = 0;

        public override void OnStateEnter(GunCrewMember _CrewMember)
        {
            CurrretState = GunLoaderState.RequestingPathToBreach;
        }

        public override void OnStateExit(GunCrewMember _CrewMember)
        {
            ResetState();
        }

        public override void OnStateUpdate(GunCrewMember _CrewMember)
        {
            switch(CurrretState)
            {
                case GunLoaderState.Idle:

                    break;
                case GunLoaderState.RequestingPathToBreach:
                    RequestPathToBreach();


                    break;
                case GunLoaderState.MovingToBreach:

                    //if(_CrewMember.OccupiedNode == FGRef.BreachNode)
                    //{
                    //    Debug.Log("Arrived at Breach");
                    //    CurrretState = GunLoaderState.AtBreach;
                    //}
                    //else if (CurrentIntervalTime > CheckInterval)
                    //{
                    //    CheckPathToBreach();
                    //    CurrentIntervalTime = 0;
                    //}
                    //else
                    //    CurrentIntervalTime += Time.deltaTime;

                    break;
                case GunLoaderState.AtBreach:

                    break;
                case GunLoaderState.LoadingShell:

                    break;
                case GunLoaderState.LoadingComplete:

                    break;

            }
        }



        //
        void StateSwitchMoveToBreach()
        {
            CurrretState = GunLoaderState.RequestingPathToBreach;
        }

        //
        void RequestPathToBreach()
        {
            //if (!HasPathBeenRequested)
            //{
            //    Owner.MovementScript.RequestPath(Owner.OccupiedNode, FGRef.BreachNode, Owner, true);
            //    HasPathBeenRequested = true;
            //}
            //if (Owner.MovementScript.WayPoints != null)
            //{
            //    Owner.MovementScript.CurrentMovementStatus = TestMovementAI.MovementStatus.MovingToNextWapoint;
            //    CurrretState = GSLoaderState.MovingToBreach;
            //    HasPathBeenRequested = false;
            //}

            //if (Owner.OccupiedNode != FGRef.BreachNode)
            //{
            //    Debug.Log("Moving To Breach");
            //    CurrretState = GSLoaderState.MovingToBreach;
            //    Owner.MovementScript.RequestPath(Owner.OccupiedNode, FGRef.BreachNode, Owner, true);
            //}
            //else
            //    CurrretState = GSLoaderState.AtBreach;
        }
        void CheckPathToBreach()
        {
            //bool ObstructionExists = false;
            //if(Owner.MovementScript.WayPoints == null)
            //    return;

            //for(int WaypointIndex = Owner.MovementScript.CurrentWaypointIndex + 1; WaypointIndex < Owner.MovementScript.WayPoints.Count; ++WaypointIndex)
            //{
            //    if(Owner.MovementScript.WayPoints[WaypointIndex] != null)
            //        if (Owner.MovementScript.WayPoints[WaypointIndex].IsOccupied)
            //        {
            //            ObstructionExists = true;
            //            break;
            //        }
            //}

            //if(ObstructionExists)
            //{
            //    Owner.MovementScript.RequestPath(Owner.OccupiedNode, FGRef.BreachNode, Owner, true);
            //}
        }







        //
        void SubToRoleRequiredEvents()
        {
            //FGRef.GunPositionsHaveChanged += StateSwitchMoveToBreach;
        }

        //
        public void StateSetup(StateMachine _SM)
        {
            //SM = _SM;
            //GST = (GunnerStateTable)_SM.LoadedStateTable;
            //Owner = _SM.SMOwner;
            //FGRef = Owner.OwnerGC.FieldGun;
            //SubToRoleRequiredEvents();
        }

        //
        void ResetState()
        {
            //CurrentIntervalTime = 0;
            //CurrretState = GunLoaderState.Idle;
        }
    }
}
