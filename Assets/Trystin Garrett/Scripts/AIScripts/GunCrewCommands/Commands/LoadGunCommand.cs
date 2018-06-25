using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Trystin
{
    public class LoadGunCommand : Command
    {
        [Header("Refrence Variables")]
        public GunCrewMember ThisGCM;
        public FieldGun ThisFieldGun;

        public GunLoaderState CurrentState = GunLoaderState.Idle;


        public override void Start()
        {

        }

        public override void OnEnterCommand()
        {
            ThisGCM.RoleCommand = this;
            ThisGCM.CurrentStatus = AIStatus.Active;
            ThisGCM.CrewRole = ArtilleryCrewRole.GunOperator;
            ThisFieldGun.Loader = ThisGCM;
            StatusCheck();
        }

        public override void Update()
        {

        }

        public override void FixedUpdate()
        {
            switch (CurrentState)
            {
                case GunLoaderState.Idle:
                    break;
                case GunLoaderState.RequestingPathToBreach:
                    if (ThisGCM.MovementOrder != null)
                        if (ThisGCM.MovementOrder.CurrentPathStatus == PathRequestStatus.RecivedAndValid)
                            CurrentState = GunLoaderState.MovingToBreach;
                    break;
                case GunLoaderState.MovingToBreach:
                    if (CheckIfAtGunBreach())
                        CurrentState = GunLoaderState.AtBreach;
                    break;
                case GunLoaderState.AtBreach:
                    StatusCheck();
                    break;
                case GunLoaderState.LoadingShell:
                    break;
                case GunLoaderState.LoadingComplete:
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
            if (CheckIfAtGunBreach() == false)
            {
                if(CurrentState == GunLoaderState.AtBreach || CurrentState == GunLoaderState.Idle)
                    StartCoroutine(RequestPathToBreach());
            }
            else
                CurrentState = GunLoaderState.AtBreach;
        }

        //
        bool CheckIfAtGunBreach()
        {
            if (ThisGCM.OccupiedNode == ThisFieldGun.BreachNode)
                return true;
            else
                return false;       
        }

        //
        IEnumerator RequestPathToBreach()
        {
            float RanTime = Random.Range(0.25f, ThisGCM.OrderDelay);
            yield return new WaitForSeconds(RanTime);
            CurrentState = GunLoaderState.RequestingPathToBreach;
            CommandManager.Instance.IssueMovementCommand(ThisGCM, ThisFieldGun.BreachNode);
        }

        void SubToEvents()
        {
            ThisFieldGun.GunPositionsHaveChanged += StatusCheck;
        }

        public override void DecommissionCommand()
        {
            ThisGCM.ActiveOrders.Remove(this);
            ThisGCM.RoleCommand = null;
            ThisGCM.CurrentStatus = AIStatus.InActive;
            ThisGCM.CrewRole = ArtilleryCrewRole.UnAssigned;
            ThisFieldGun.Loader = null;
            Destroy(this);
        }
    }
}
