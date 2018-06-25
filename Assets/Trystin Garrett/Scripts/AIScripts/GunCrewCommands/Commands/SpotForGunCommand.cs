using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Trystin
{
    public class SpotForGunCommand : Command
    {
        [Header("Refrence Variables")]
        public GunCrewMember ThisGCM;
        public FieldGun ThisFieldGun;
        public int ChosenSpottingNodeIndex;

        public GunSpotterState CurrentState = GunSpotterState.Idle;

        public override void Start()
        {

        }

        public override void OnEnterCommand()
        {
            ThisGCM.RoleCommand = this;
            ThisGCM.CurrentStatus = AIStatus.Active;
            ThisGCM.CrewRole = ArtilleryCrewRole.Spotter;
            ThisFieldGun.Spotter = ThisGCM;
            ChosenSpottingNodeIndex = ChooseSpottingNode();
            StatusCheck();
        }

        public override void Update()
        {

        }

        public override void FixedUpdate()
        {
            switch (CurrentState)
            {
                case GunSpotterState.Idle:
                    break;
                case GunSpotterState.RequestingPathToSpotterPos:
                    if (ThisGCM.MovementOrder != null)
                        if (ThisGCM.MovementOrder.CurrentPathStatus == PathRequestStatus.RecivedAndValid)
                            CurrentState = GunSpotterState.MovingToSpotterPos;
                    break;
                case GunSpotterState.MovingToSpotterPos:
                    if (CheckIfAtSpotterPos())
                        CurrentState = GunSpotterState.AtSpotterPos;
                    break;
                case GunSpotterState.AtSpotterPos:
                    StatusCheck();
                    break;
                case GunSpotterState.ScanningForTargets:
                    break;
                case GunSpotterState.TargetFound:
                    break;
                case GunSpotterState.CallingInStrike:
                    break;
                default:
                    break;
            }
        }

        public override void OnExitCommand()
        {
            DecommissionCommand();
        }

        public void PassInfo(GunCrewMember _GCM, FieldGun _FieldGun)
        {
            ThisFieldGun = _FieldGun;
            ThisGCM = _GCM;
            SubToEvents();
            OnEnterCommand();
        }

        //
        void StatusCheck()
        {
            if (CheckIfAtSpotterPos() == false)
            {
                if (CurrentState == GunSpotterState.AtSpotterPos || CurrentState == GunSpotterState.Idle)
                    StartCoroutine(RequestPathToSpotterPos());
            }
            else
                CurrentState = GunSpotterState.AtSpotterPos;
        }

        //
        int ChooseSpottingNode()
        {
            int RanBool = Random.Range(0,2);
            return RanBool;
        }

        //
        bool CheckIfAtSpotterPos()
        {
            if (ThisGCM.OccupiedNode == ThisFieldGun.GunSpotterNodes[ChosenSpottingNodeIndex])
                return true;
            else
                return false;
        }

        //
        IEnumerator RequestPathToSpotterPos()
        {
            float RanTime = Random.Range(0.25f, ThisGCM.OrderDelay);
            yield return new WaitForSeconds(RanTime);
            CurrentState = GunSpotterState.RequestingPathToSpotterPos;
            CommandManager.Instance.IssueMovementCommand(ThisGCM, ThisFieldGun.GunSpotterNodes[ChosenSpottingNodeIndex]);
        }

        //
        void SubToEvents()
        {
            ThisFieldGun.GunPositionsHaveChanged += StatusCheck;
        }

        //
        public override void DecommissionCommand()
        {
            ThisGCM.ActiveOrders.Remove(this);
            ThisGCM.RoleCommand = null;
            ThisGCM.CurrentStatus = AIStatus.InActive;
            ThisGCM.CrewRole = ArtilleryCrewRole.UnAssigned;
            ThisFieldGun.Spotter = null;
            Destroy(this);
        }
    }
}