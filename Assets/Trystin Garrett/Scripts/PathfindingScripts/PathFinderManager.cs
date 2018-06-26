using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tystin.NodeUtility;
namespace Trystin
{
    public class PathFinderManager : MonoBehaviour
    {
        public static PathFinderManager Instance;

        public NodeManager NM;
        public Pathfinder[] PathFinders = new Pathfinder[2];
        public bool PathFinderIsActive = false;

        public Queue<PathRequest> PathRequests = new Queue<PathRequest>();
        public Queue<PathRequest> CompleatedRequests = new Queue<PathRequest>();

        public bool FrameToggle;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            NM = NodeManager.Instance;
            for (int PFIndex = 0; PFIndex < PathFinders.Length; ++PFIndex)
                PathFinders[PFIndex] = new Pathfinder();

        }

        private void Update()
        {
            if (!PathFinderIsActive)
                return;

            if (FrameToggle == false)
            {
                DesignateRequestsToPathfinder();
                FrameToggle = true;
            }
            else if (FrameToggle == true)
            {
                DistributePaths();
                FrameToggle = false;
            }
        }

        //This should be queued to be handled when ready to rather than a immediate method call
        public void RequestPathFromNodes(Node _StartingNode, Node _TargetNode, MovementCommand _Requestee)
        {
            PathRequest PR = new PathRequest(_StartingNode, _TargetNode, _Requestee);
            PathRequests.Enqueue(PR);
        }

        //This should be queued to be handled when ready to rather than a immediate method call
        public void RequestRandomPathFromVec3(Vector3 _CurrentPos, MovementCommand _Requestee)
        {
            Node StartingNode = NM.FindNodeFromWorldPosition(_CurrentPos);
            Node TargetNode = NM.GetRandNode();

            int RandAttempts = 0;

            if (TargetNode == null)
            {
                while (TargetNode == null)
                {
                    ++RandAttempts;
                    TargetNode = NM.GetRandNode();
                    if (RandAttempts > 20)
                        break;
                }

            }

            PathRequest PR = new PathRequest(StartingNode, TargetNode, _Requestee);
            PathRequests.Enqueue(PR);
        }

        //
        public void RequestPathFromVec3s(Vector3 _CurrentPos, Vector3 _TargetPos, MovementCommand _Requestee)
        {
            Node StartingNode = NM.FindNodeFromWorldPosition(_CurrentPos);
            Node TargetNode = NM.FindNodeFromWorldPosition(_TargetPos);

            PathRequest PR = new PathRequest(StartingNode, TargetNode, _Requestee);
            PathRequests.Enqueue(PR);
        }

        //
        void DesignateRequestsToPathfinder()
        {
            if (PathRequests.Count == 0)
                return;
            for (int PFIndex = 0; PFIndex < PathFinders.Length; ++PFIndex)
            {
                if (PathFinders[PFIndex].CurrentStatus == Pathfinder.PathfinderStatus.Incative)
                {
                    PathRequest PR = PathRequests.Dequeue();
                    PathFinders[PFIndex].SubmitFindPath(PR, this, NM);
                    break;
                }
            }
        }

        //
        void DistributePaths()
        {
            if (CompleatedRequests.Count == 0)
                return;

            for (int CRIndex = 0; CRIndex < CompleatedRequests.Count; ++CRIndex)
            {
                PathRequest CPR = CompleatedRequests.Dequeue();
                CPR.Requestee.RecivePathRequest(CPR);
            }
        }
        //
        void RetryPath(PathRequest _CPR)
        {
            Debug.Log("Retrying Path");
            _CPR.IsBeingProcessed = false;
            _CPR.CompletedPath = null;

            PathRequests.Enqueue(_CPR);
        }

        //To return a reachable node within a radius and direction    TODO: Check against occupied nodes perhaps...
        public Node ReturnDirectionalSearchNode(Node _OriginNode, int _Radius, Direction _Direction)
        {
            Vector2Int Direction = ReturnAdditonValues(_Direction);
            int MaxRadius = _Radius - ClampRadius(_Direction, _Radius);

            for (int RadiusIndex = MaxRadius; RadiusIndex > 0; --RadiusIndex)
            {
                int XINdex = (Direction.X * RadiusIndex);
                int YINdex = (Direction.Y * RadiusIndex);

                int XRef = XINdex + _OriginNode.GridPosition.X;
                int YRef = YINdex + _OriginNode.GridPosition.Y;

                if (XRef < 0 || XRef > (NM.GridXLength - 1) || YRef < 0 || YRef > (NM.GridYLength - 1))
                    continue;

                if (NM.NodeGrid[XRef, YRef] != null)
                    return NM.NodeGrid[XRef, YRef];
            }
            return null;
        }

        //Returns Directional Grid multiplier     TODO: Make this ugly code elegant...
        Vector2Int ReturnAdditonValues(Direction _Direction)
        {
            switch (_Direction)
            {
                case Direction.North:
                    return new Vector2Int(1,0);
                case Direction.NorthEast:
                    return new Vector2Int(1, 1);
                case Direction.East:
                    return new Vector2Int(0, 1);
                case Direction.SouthEast:
                    return new Vector2Int(-1, 1);
                case Direction.South:
                    return new Vector2Int(-1, 0);
                case Direction.SouthWest:
                    return new Vector2Int(-1, -1);
                case Direction.West:
                    return new Vector2Int(0, -1);
                case Direction.NorthWest:
                    return new Vector2Int(1, -1);
            }
            return new Vector2Int(0, 0);
        }
        // This allows search Radius to keep to the Vision Shape within 4 - 13 nodes.    TODO: Make this ugly code elegant
        int ClampRadius(Direction _Direction, int _Radius)
        {
            int Ajustmnet = 2;
            if (_Radius > 5)
            {
                if (_Radius == 6 || _Radius == 7)
                    ++Ajustmnet;
                else if (_Radius == 8 || _Radius == 9)
                    Ajustmnet += 2;
                else if (_Radius == 10 || _Radius == 11)
                    Ajustmnet += 3;
                else if (_Radius == 12 || _Radius == 13)
                    Ajustmnet += 4;
            }
            switch (_Direction)
            {
                case Direction.NorthEast:
                    return Ajustmnet;
                case Direction.SouthEast:
                    return Ajustmnet;
                case Direction.SouthWest:
                    return Ajustmnet;
                case Direction.NorthWest:
                    return Ajustmnet;
            }
            return 0;
        }

        //
        //public Direction DetermineQuadrant(Node _CurrentNode)
        //{
        //    int XPercentage = (_CurrentNode.GridPostion.X / NodeManager.Instance.GridXLength) * 100;
        //    int YPercentage = (_CurrentNode.GridPostion.Y / NodeManager.Instance.GridYLength) * 100;
        //    int SectionExtents = NodeManager.Instance.GridYLength / 3;

        //    if (XPercentage <= SectionExtents && YPercentage <= SectionExtents)
        //        return Direction.SouthWest;

        //}
    }

    //Hold Path request information to use later
    public class PathRequest
    {
        public PathRequest(Node _StartingNode, Node _TargetNode, MovementCommand _Requestee)
        {
            StartingNode = _StartingNode;
            TargetNode = _TargetNode;
            Requestee = _Requestee;
            PathIsFound = false;
            IsBeingProcessed = false;
            CompletedPath = null;
        }

        public bool PathIsFound;
        public bool IsBeingProcessed;
        public Node StartingNode;
        public Node TargetNode;
        public MovementCommand Requestee;
        public List<Node> CompletedPath;
    }
}
