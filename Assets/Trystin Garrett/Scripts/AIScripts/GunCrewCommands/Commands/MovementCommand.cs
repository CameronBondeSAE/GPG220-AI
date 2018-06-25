using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class MovementCommand : Command
    {
        [Header("Refrence Variables")]
        public GunCrewMember ThisGCM;
        public Rigidbody ThisRB;

        [Space]
        [Header("Status")]
        public MovementStatus CurrentMovementStatus = MovementStatus.Idle;
        public PathRequestStatus CurrentPathStatus = PathRequestStatus.NoneRequested;

        [Space]
        [Header("Waypoint Variables")]
        public Node TargetNode;
        public List<Node> WayPoints;
        public Node TargetWaypoint;
        public Node CurrentWaypoint;
        public int CurrentWaypointIndex = 0;

        [Space]
        [Header ("Movement Variables")]
        public MovementSpeed CurrentMovementSpeed = MovementSpeed.Walking;
        public float Speed = 1f;
        public float RotationSpeed = 10f;
        public float WaypointTolerence = 0.4f;

        [Space]
        [Header("Debugging")]
        public bool VisualDebugging = false;
        public int TestInt;
  
        //public MovementCommand(Node _TargetNode, GunCrewMember _GCM)
        //{
        //    TargetNode = _TargetNode;
        //    ThisGCM = _GCM;
        //}
        public void PassInfo( GunCrewMember _GCM, Node _TargetNode)
        {
            TargetNode = _TargetNode;
            ThisGCM = _GCM;
            OnEnterCommand();
        }

        public override void Start()
        {

        }

        public override void OnEnterCommand()
        {
            ThisGCM.ActiveOrders.Add(this);
            ThisGCM.MovementOrder = this;
            ThisRB = GetComponent<Rigidbody>();
            VariableSetup();
            StopMovement();
            ThisGCM.OccupiedNode = NodeManager.Instance.FindNodeFromWorldPosition(ThisGCM.transform.position);
            RequestPath(ThisGCM.OccupiedNode, TargetNode, this);
        }

        public override void Update()
        {

        }

        public override void FixedUpdate()
        {
            MoveAlongPath();
        }

        public override void OnExitCommand()
        {
            DecommissionCommand();
        }

        //
        public void RequestPath(Node _StartNode, Node _TargetNode, MovementCommand _MovCom)
        {

            if (CurrentPathStatus == PathRequestStatus.NoneRequested)
            {
                CurrentPathStatus = PathRequestStatus.RequestedAndWaiting;
                PathFinderManager.Instance.RequestPathFromNodes(_StartNode, _TargetNode, _MovCom);
            }
        }

        //
        public void RecivePathRequest(PathRequest _CPR)
        {
            if (_CPR.PathIsFound == false)
            {
                CurrentPathStatus = PathRequestStatus.NoneRequested;
                StartCoroutine(Wait(0.25f));
                RequestPath(_CPR.StartingNode, _CPR.TargetNode, _CPR.Requestee);
            }
            else
            {
                WayPoints = _CPR.CompletedPath;
                CurrentWaypoint = WayPoints[0];
                CurrentPathStatus = PathRequestStatus.RecivedAndValid;
                CurrentMovementStatus = MovementStatus.MovingToNextWapoint;
            }
        }

        //
        void MoveAlongPath()
        {
            if (WayPoints == null || CurrentPathStatus != PathRequestStatus.RecivedAndValid)
                return;

            switch (CurrentMovementStatus)
            {
                case MovementStatus.Idle:
                    StopMovement();
                    break;

                case MovementStatus.MovingToNextWapoint:

                    switch(WaypointCheck(WaypointTolerence))
                    {
                        case WaypointStatus.AtTargetNode:
                            CurrentMovementStatus = MovementStatus.ArrivedAtTargetNode;
                            break;
                        case WaypointStatus.AtWaypoint:
                            Debug.Log("Changing Waypoints");
                            ThisGCM.OccupiedNode = CurrentWaypoint;

                            if(CheckUpComingNodesForObstruction(5))
                            {
                                CurrentMovementStatus = MovementStatus.Idle;
                                return;
                            }

                            CurrentWaypoint = WayPoints[CurrentWaypointIndex];
                            CurrentMovementStatus = MovementStatus.MovingToNextWapoint;
                            break;
                        case WaypointStatus.BetweenWaypoints:
                            MoveToWaypoint();
                            break;
                    }
                    break;

                case MovementStatus.ArrivedAtTargetNode:
                    StopMovement();
                    ThisGCM.OccupiedNode = CurrentWaypoint;
                    OnExitCommand();
                    break;
            }
        }

        //Scans to check if upcomming nodes have become occupied so there is no collisions
        bool CheckUpComingNodesForObstruction(int _NumNodes)
        {
            int CheckExtent = CurrentWaypointIndex + _NumNodes;
            if (CheckExtent > WayPoints.Count)
                CheckExtent = (WayPoints.Count);

            for (int NodeIndex = CurrentWaypointIndex; NodeIndex < CheckExtent; ++NodeIndex)
            {
                TestInt = NodeIndex;
                if (WayPoints[NodeIndex].IsOccupied)
                {
                    if (WayPoints[NodeIndex].Occupant is GunCrewMember)
                        continue;

                    else
                        return true;
                }
            }
            return false;
        }

        //Check to see if current positon is within range of Waypoint Node to change state
        WaypointStatus WaypointCheck(float _Tolerance)
        {
            float CurrentDisFromWaypoint = Vector3.Distance(ThisGCM.transform.position, CurrentWaypoint.WorldPosition);
            if (CurrentDisFromWaypoint < WaypointTolerence)
            {
                if (CurrentWaypoint == TargetNode)
                    return WaypointStatus.AtTargetNode;

                ++CurrentWaypointIndex;
                if (WayPoints[CurrentWaypointIndex] != null)
                    return WaypointStatus.AtWaypoint;
            }
            return WaypointStatus.BetweenWaypoints;
        }

        //Undertakes the applying of velocity ot the RB as well as the game object rotation
        void MoveToWaypoint()
        {
            Vector3 LookAtTarget = new Vector3(CurrentWaypoint.WorldPosition.x, 0.3f, CurrentWaypoint.WorldPosition.z);

            float step = RotationSpeed * Time.deltaTime;
            Vector3 targetDir = LookAtTarget - transform.position;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            ThisRB.velocity = transform.forward * Speed;
        }

        //Haults Movement
        void StopMovement()
        {
            ThisRB.velocity = Vector3.zero;
            ThisRB.isKinematic = true;
            transform.rotation = Quaternion.Euler(transform.forward);
            ThisRB.isKinematic = false;
        }


        //Waitsfor Seconds to stagger PathRequests
        IEnumerator Wait(float _WaitTime)
        {
            yield return new WaitForSeconds(_WaitTime);
        }

        //Changes Speeed of unit
        public void ChangeSpeed(MovementSpeed _NewSpeed)
        {
            switch (_NewSpeed)
            {
                case MovementSpeed.Walking:
                    Speed = 1;
                    CurrentMovementSpeed = MovementSpeed.Walking;
                    break;
                case MovementSpeed.Jogging:
                    Speed = 1.5f;
                    break;
                case MovementSpeed.Running:
                    Speed = 2f;
                    break;
                case MovementSpeed.Sprinting:
                    Speed = 3f;
                    break;
            }
        }

        // I donlt know why but it needs to decalre variable values otherwise anything hardcoded will not be set when Adding compoonet.... Will always revert to 0...
        void VariableSetup()
        {
            RotationSpeed = 10f;
            WaypointTolerence = 0.15f;
            ChangeSpeed(MovementSpeed.Walking); 
        }

        //For ease of use getting to destroy rather than recycle it... Could implement later but getting this all working first.
        public override void DecommissionCommand()
        {
            StopMovement();
            ThisGCM.ActiveOrders.Remove(this);
            ThisGCM.MovementOrder = null;
            Destroy(this);
        }

        //
        private void OnDrawGizmos()
        {
            if (VisualDebugging == false)
                return;
            if (WayPoints != null)
            {
                float WireSphereSize = WayPoints[0].TileSize.x / 4;
                for (int NodeIndex = 0; NodeIndex < WayPoints.Count; ++NodeIndex)
                {
                    Gizmos.color = Color.green;
                    if (WayPoints[NodeIndex] == CurrentWaypoint)
                        Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(WayPoints[NodeIndex].WorldPosition, WireSphereSize);

                }
            }
        }
    }
}
