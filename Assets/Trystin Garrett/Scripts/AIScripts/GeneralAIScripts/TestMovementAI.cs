using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tystin.NodeUtility;
using Tystin;

namespace Trystin
{

    public class TestMovementAI : MonoBehaviour
    {

        public enum PathRequestStatus
        {
            NoneRequested,
            RequestedAndWaiting,
            Recived
        }

        public enum MovementStatus
        {
            Null,
            Idle,
            MovingToNextWapoint,
            ArrivedAtWaypoint,
            ArrivedAtTargetNode
        }

        public enum MovementSpeed
        {
            Walking,
            Jogging,
            Running,
            Sprinting
        }

        public GunCrewMember ThisGCM;
        public Rigidbody ThisRB;

        public MovementStatus CurrentMovementStatus = MovementStatus.Null;
        public PathRequestStatus CurrentPathStatus = PathRequestStatus.NoneRequested;
        public List<Node> WayPoints;

        public Node TargetWaypoint;
        public Node CurrentWaypoint;
        public int CurrentWaypointIndex = 0;

        public Vector3 RotationToNextWaypoint;


        public bool TogglePathFound = false;
        public MovementSpeed CurrentMovementSpeed = MovementSpeed.Walking;
        float Speed
        {
            get
            {
                switch (CurrentMovementSpeed)
                {
                    case MovementSpeed.Walking:
                        return 1;
                    case MovementSpeed.Jogging:
                        return 1.5f;
                    case MovementSpeed.Running:
                        return 2.5f;
                    case MovementSpeed.Sprinting:
                        return 3;
                }
                return 0;
            }
        }
        public float RotationSpeed;

        //
        private void Awake()
        {
            if (ThisGCM == null)
                ThisGCM = GetComponent<GunCrewMember>();

        }

        //
        private void FixedUpdate()
        {
            MoveAlongPath();
        }

        //
        public void RequestRandomPath()
        {
            if (CurrentPathStatus != PathRequestStatus.RequestedAndWaiting)
            {
                ResetPathing();
                CurrentPathStatus = PathRequestStatus.RequestedAndWaiting;
                PathFinderManager.Instance.RequestRandomPathFromVec3(transform.position, ThisGCM);
            }
        }

        //
        public void RequestPath(Node _StartNode, Node _TargetNode, GunCrewMember _GCM, bool _SkipDiagnals)
        {

            if (CurrentPathStatus == PathRequestStatus.NoneRequested)
            {
                ResetPathing();
                CurrentPathStatus = PathRequestStatus.RequestedAndWaiting;
                PathFinderManager.Instance.RequestPathFromNodes(_StartNode, _TargetNode, _GCM, _SkipDiagnals);
            }
        }

        //
        public void RecivePath(List<Node> _NodePath)
        {
            WayPoints = _NodePath;
            CurrentPathStatus = PathRequestStatus.Recived;
            //Debug.Log(gameObject.name + " Recived a Path! It is " + WayPoints.Count + " Nodes Long");
        }

        void ResetPathing()
        {
            WayPoints = null;
            CurrentWaypoint = null;
            TargetWaypoint = null;
            CurrentWaypointIndex = 0;
        }

        public void MoveAlongPath()
        {
            if (WayPoints == null)
                return;
            CurrentPathStatus = PathRequestStatus.NoneRequested;

            if (CurrentWaypoint == null)
                if (WayPoints[CurrentWaypointIndex] != null)
                    CurrentWaypoint = WayPoints[CurrentWaypointIndex];

            if (CurrentMovementStatus == MovementStatus.Idle && CurrentWaypoint != null)
                CurrentMovementStatus = MovementStatus.MovingToNextWapoint;

            switch (CurrentMovementStatus)
            {
                case MovementStatus.Null:
                    ThisRB.velocity = Vector3.zero;
                    break;

                case MovementStatus.Idle:
                    ThisRB.velocity = Vector3.zero;
                    break;

                case MovementStatus.MovingToNextWapoint:
                    float CurrentDisFromWaypoint = Vector3.Distance(transform.position, CurrentWaypoint.WorldPosition);
                    if (CurrentDisFromWaypoint < 0.3f)
                    {
                        if (CurrentWaypoint == TargetWaypoint)
                        {
                            CurrentMovementStatus = MovementStatus.ArrivedAtTargetNode;
                            return;
                        }

                        ++CurrentWaypointIndex;
                        if (WayPoints[CurrentWaypointIndex] != null)
                        {
                            CurrentWaypoint = WayPoints[CurrentWaypointIndex];
                            CurrentMovementStatus = MovementStatus.MovingToNextWapoint;
                        }
                    }

                    else
                    {
                        //THIS LOOK AT SHOULD BE 50% of the Collider height for the Y
                        Vector3 LookAtTarget = new Vector3(CurrentWaypoint.WorldPosition.x, 0.25f, CurrentWaypoint.WorldPosition.z);

                        float step = RotationSpeed * Time.deltaTime;
                        Vector3 targetDir = LookAtTarget - transform.position;
                        RotationToNextWaypoint = targetDir;
                        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                        transform.rotation = Quaternion.LookRotation(newDir);
                        ThisRB.velocity = transform.forward * Speed;
                    }
                    break;

                case MovementStatus.ArrivedAtWaypoint:

                    break;

                case MovementStatus.ArrivedAtTargetNode:
                    ThisRB.velocity = Vector3.zero;
                    transform.rotation = Quaternion.Euler(transform.forward);
                    //RequestRandomPath();
                    CurrentMovementStatus = MovementStatus.Idle;
                    break;
            }


        }






        private void OnDrawGizmos()
        {
            if (TogglePathFound == false)
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
