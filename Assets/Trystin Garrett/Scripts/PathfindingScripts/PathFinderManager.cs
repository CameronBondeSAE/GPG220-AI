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



        public Node TestDirectionList;
        public SearchDirection SearchDirection = SearchDirection.North;
        public int RadiusOfSearch = 5;
        public bool VisualDebugging = false;
        public Transform TestTransform;




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
            //_Requestee.MovementScript.TargetWaypoint = _TargetNode;
            PathRequests.Enqueue(PR);
        }

        //This should be queued to be handled when ready to rather than a immediate method call
        public void RequestRandomPathFromVec3(Vector3 _CurrentPos, MovementCommand _Requestee)
        {
            //Debug.Log("PFM:   Random Request Made");
            Node StartingNode = NM.FindNodeFromWorldPosition(_CurrentPos);
            Node TargetNode = NM.GetRandNode();

            int RandAttempts = 0;

            if (TargetNode == null)
            {
                while (TargetNode == null)
                {
                    ++RandAttempts;
                    //Debug.Log("PFM:   Retrying Random Node Find... Attempt: " + RandAttempts);
                    TargetNode = NM.GetRandNode();

                    if (RandAttempts > 20)
                        break;
                }

            }

            //Debug.Log("PFM:   Found Random Node! Starting Node is: " + StartingNode.GridPostion.X + "/" + StartingNode.GridPostion.Y + " To " + TargetNode.GridPostion.X + "/" + TargetNode.GridPostion.Y);
            PathRequest PR = new PathRequest(StartingNode, TargetNode, _Requestee);
            //_Requestee.MovementScript.TargetWaypoint = TargetNode;
            PathRequests.Enqueue(PR);
        }

        //
        public void RequestPathFromVec3s(Vector3 _CurrentPos, Vector3 _TargetPos, MovementCommand _Requestee)
        {
            Node StartingNode = NM.FindNodeFromWorldPosition(_CurrentPos);
            Node TargetNode = NM.FindNodeFromWorldPosition(_TargetPos);

            PathRequest PR = new PathRequest(StartingNode, TargetNode, _Requestee);
            //_Requestee.MovementScript.TargetWaypoint = TargetNode;
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
                    //Debug.Log("PFM:   Request Designated from " + PR.Requestee.gameObject.name);
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

                //if (CPR.PathIsFound == false)
                //    RetryPath(CPR);
                //else
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

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.25f);
        }

        //
        public Node ReturnDirectionalSearchNode(Node _OriginNode, int _Radius, SearchDirection _Direction)
        {
            TestDirectionList = null;
            Vector2Int Direction = ReturnAdditonValues(_Direction);
            int MaxRadius = _Radius - ClampRadius(_Direction, _Radius);

            for (int RadiusIndex = MaxRadius; RadiusIndex > 0; --RadiusIndex)
            {
                Debug.Log("Bam");
                int XINdex = (Direction.X * RadiusIndex);
                int YINdex = (Direction.Y * RadiusIndex);

                int XRef = XINdex + _OriginNode.GridPostion.X;
                int YRef = YINdex + _OriginNode.GridPostion.Y;

                if (XRef < 0 || XRef > (NM.GridXLength - 1) || YRef < 0 || YRef > (NM.GridYLength - 1))
                    continue;

                if (NM.NodeGrid[XRef, YRef] != null)
                    return NM.NodeGrid[XRef, YRef];
            }
            return null;
        }

        //
        Vector2Int ReturnAdditonValues(SearchDirection _Direction)
        {
            switch (_Direction)
            {
                case SearchDirection.North:
                    return new Vector2Int(1,0);
                case SearchDirection.NorthEast:
                    return new Vector2Int(1, 1);
                case SearchDirection.East:
                    return new Vector2Int(0, 1);
                case SearchDirection.SouthEast:
                    return new Vector2Int(-1, 1);
                case SearchDirection.South:
                    return new Vector2Int(-1, 0);
                case SearchDirection.SouthWest:
                    return new Vector2Int(-1, -1);
                case SearchDirection.West:
                    return new Vector2Int(0, -1);
                case SearchDirection.NorthWest:
                    return new Vector2Int(1, -1);
            }
            return new Vector2Int(0, 0);
        }
        // This allows search Radius to keep to the Vision Shape
        int ClampRadius(SearchDirection _Direction, int _Radius)
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
                case SearchDirection.NorthEast:
                    return Ajustmnet;
                case SearchDirection.SouthEast:
                    return Ajustmnet;
                case SearchDirection.SouthWest:
                    return Ajustmnet;
                case SearchDirection.NorthWest:
                    return Ajustmnet;
            }
            return 0;
        }

        //
        private void OnDrawGizmos()
        {
            if (VisualDebugging == false)
                return;
            if (TestDirectionList != null)
            {
                float WireSphereSize = TestDirectionList.TileSize.x / 4;
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(TestDirectionList.WorldPosition, WireSphereSize);
            }
        }
    }

    //
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
            //SkipDiagnals = _SkipDiagnals;
        }

        public bool PathIsFound;
        public bool IsBeingProcessed;
        public bool SkipDiagnals;
        public Node StartingNode;
        public Node TargetNode;
        public MovementCommand Requestee;
        public List<Node> CompletedPath;
    }
}
