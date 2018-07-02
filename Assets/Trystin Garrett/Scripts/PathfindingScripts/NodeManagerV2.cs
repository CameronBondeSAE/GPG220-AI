using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
namespace Trystin
{
    public class NodeManagerV2 : MonoBehaviour
    {
        [Header("Input Variables")]
        public Vector2 NodeRatio = new Vector2(1, 1);
        public float NodeElevationFromCollider = 0.3f;
        public float HighestReachableYValue;

        [Space]
        [Header("Node/Obsticale Removal Variables")]
        public float ObsticleOverlapPercentageAllowance = 0.1f;

        [Space]
        [Header("Obsticale Traverse Interval Start Heights")]
        public float JumpIntervalHeight = 0.3f;
        public float ClimbIntervalHeight = 1.1f;
        public float UnScalableIntervalHeight = 2.1f;

        [Space]
        [Header("Incline Movement Angles")]
        public float InclineConnectionMin = 0.05f;
        public float InclineConnectionMax = 0.3f;


        [Space]
        [Header("Grid Setup")]
        public GridNavigationType SelectedNavType = GridNavigationType.SingleLayer;
        public GridCreationMethod SelectedMethod = GridCreationMethod.GridSizeIndependent;

        [Space]
        [Header("Floor Variables")]
        public float FloorXLength; //
        public float FloorYLength; //
        public float FloorElevation;
        public Collider FloorCollider;

        [Space]
        [Header("Node Variables")]
        public Vector3 GridStartPos;
        public float NodeHight;

        [Space]
        [Header("Grid Variables")]
        public int GridXLength;
        public int GridYLength;
        public float GridXScale;
        public float GridYScale;
        public Vector2 NodePositionGridScaleOffset;
        public Node[,] NodeGrid;

        [Space]
        [Header("Setup Status")]
        public Thread NodeSetupThread; // Maybe keep track of the active thread
        public ProgressState CurrentGridState = ProgressState.Inactive;
        public ProgressState CurrentNeighbourState = ProgressState.Inactive;

        [Space]
        [Header("NodeRemoval/Relocaton Variables")]
        public float FloorCheckDistance;
        public float CheckOverLap;
        public float CombinedCheckDistance;

        [Space]
        [Header("Debugging")]
        public bool VisualDebugging = false;
        public Node FoundRandNode;

        public void GetFloorGridDimentions()
        {

            FloorCollider = this.gameObject.GetComponent<Collider>();
            Bounds ColBounds = FloorCollider.bounds;
            FloorXLength = ColBounds.size.x;
            FloorYLength = ColBounds.size.z;

            if (HighestReachableYValue != 0)
            {
                FloorElevation = HighestReachableYValue;
                FloorCheckDistance = HighestReachableYValue * 2;
            }
            else
            {
                FloorElevation = ColBounds.size.y;
                HighestReachableYValue = FloorElevation;
                FloorCheckDistance = ColBounds.size.y;
            }
            GridStartPos = new Vector3((ColBounds.center.x - ColBounds.extents.x), (FloorElevation + NodeElevationFromCollider), (ColBounds.center.z - ColBounds.extents.z));

            GridXScale = NodeRatio.x;
            GridYScale = NodeRatio.y;
            NodePositionGridScaleOffset = new Vector2((GridXScale / 2), (GridYScale / 2));
            CheckOverLap = 1f - ObsticleOverlapPercentageAllowance;

            NodeHight = (NodeElevationFromCollider - 0.05f) * 2;
            CombinedCheckDistance = FloorCheckDistance + NodeElevationFromCollider;

            switch (SelectedMethod)
            {
                case GridCreationMethod.GridSizeDependent:
                    break;
                case GridCreationMethod.GridSizeIndependent:
                    GridXLength = (int)(FloorXLength / GridXScale);
                    GridYLength = (int)(FloorYLength / GridYScale);
                    break;
            }
        }

        public void CallCreateGrid()
        {
            if (NodeGrid != null || CurrentGridState != ProgressState.Inactive)
                return;

            CurrentGridState = ProgressState.InProgress;
            Thread GridCreationThread = new Thread(CreateGrindThreadCall);
            GridCreationThread.Start();
        }
        void CreateGrindThreadCall()
        {
            int CurrentNodeCount = 0;

            Node[,] NewTileGrid = new Node[GridXLength, GridYLength];
            for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
            {
                for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                {
                    ++CurrentNodeCount;
                    Node NewNodeScript = new Node();
                    NewNodeScript.GridPosition = new Vector2Int(XIndex, YIndex);
                    NewNodeScript.WorldPosition = new Vector3(((NewNodeScript.GridPosition.X * GridXScale) + NodePositionGridScaleOffset.x), NodeElevationFromCollider, ((NewNodeScript.GridPosition.Y * GridYScale) + NodePositionGridScaleOffset.y)) + GridStartPos;
                    NewNodeScript.TileSize = new Vector3(GridXScale - 0.01f, 0.1f, GridYScale - 0.01f);
                    NewTileGrid[XIndex, YIndex] = NewNodeScript;
                    NewNodeScript.GridPosition = new Vector2Int(XIndex, YIndex);
                }
            }
            NodeGrid = NewTileGrid;
            CurrentGridState = ProgressState.Complete;
        }


        public void RemoveNonFloorTiles()
        {

            RaycastHit LocalHit;
            Vector3 HalfExents = (new Vector3(GridXScale - 0.01f, 0.1f, GridYScale - 0.01f) * CheckOverLap) / 2;
            for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
            {
                for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                {
                    if (Physics.Raycast(NodeGrid[XIndex, YIndex].WorldPosition, transform.TransformDirection(Vector3.down), out LocalHit, CombinedCheckDistance))
                    {
                        RepositionNode(NodeGrid[XIndex, YIndex], LocalHit);

                        //Does not like pro builder...
                        //if (Physics.CheckBox(NodeGrid[XIndex, YIndex].WorldPosition, HalfExents))
                        //    NodeGrid[XIndex, YIndex] = null;
                    }
                    else
                        NodeGrid[XIndex, YIndex] = null;
                }
            }
        }

        //
        void RepositionNode(Node _Node, RaycastHit _LocalHit)
        {
            float VerticalDistanceFromWorldPoint = _LocalHit.distance - NodeElevationFromCollider;

            _Node.NodeAngle = _LocalHit.normal;
            Vector3 PosDifference = new Vector3(0, -VerticalDistanceFromWorldPoint, 0);
            _Node.WorldPosition += PosDifference;
        }

        //
        public void NeighbourSetThreadCall()
        {
            if (CurrentGridState != ProgressState.Complete)
                return;

            CurrentNeighbourState = ProgressState.InProgress;
            NodeSetupThread = new Thread(SetTileNeighbours);
            NodeSetupThread.Start();
        }
        void SetTileNeighbours()
        {
            if (NodeGrid == null)
            {
                CurrentNeighbourState = ProgressState.Inactive;
                return;
            }

            for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
                for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                    if (NodeGrid[XIndex, YIndex] != null)
                    {
                        SetNeighbouringNodeTiles(NodeGrid[XIndex, YIndex]);
                    }


            CurrentNeighbourState = ProgressState.Complete;
        }

        //
        public void SetNeighbouringNodeTiles(Node _NodeTile)
        {
            if (_NodeTile == null)
                return;

            //List<Node> Neighbours = new List<Node>();
            //List<NodeNeighbourConnection> NeighbourConnections = new List<NodeNeighbourConnection>();
            //List<bool> IsCornerTurn = new List<bool>();
            Node[] Neighbours = new Node[8];
            NodeNeighbourConnection[] NeighbourConnections = new NodeNeighbourConnection[8];
            bool[] IsCornerTurn = new bool[8];
            int NeighbourIndex = 0;

            for (int XIndex = -1; XIndex <= 1; XIndex++)
            {
                for (int ZIndex = -1; ZIndex <= 1; ZIndex++)
                {
                    bool IsDiagnal = false;
                    if (XIndex == 0 && ZIndex == 0)
                    {
                        continue;
                    }
                    if ((XIndex == 1 && ZIndex == 1) || (XIndex == -1 && ZIndex == -1) || (XIndex == -1 && ZIndex == 1) || (XIndex == -1 && ZIndex == -1))
                        IsDiagnal = true;

                    int CheckX = _NodeTile.GridPosition.X + XIndex;
                    int CheckZ = _NodeTile.GridPosition.Y + ZIndex;

                    if (CheckX >= 0 && CheckX < GridXLength && CheckZ >= 0 && CheckZ < GridYLength)
                    {
                        if (NodeGrid[CheckX, CheckZ] != null)
                        {
                            Neighbours[NeighbourIndex] = NodeGrid[CheckX, CheckZ];
                            NeighbourConnections[NeighbourIndex] = DetermineConnectionRelationship(_NodeTile, NodeGrid[CheckX, CheckZ], IsDiagnal);
                            //Neighbours.Add(NodeGrid[CheckX, CheckZ]);
                            //NeighbourConnections.Add(DetermineConnectionRelationship(_NodeTile, NodeGrid[CheckX, CheckZ], IsDiagnal));
                        }
                        else
                        {
                            //Neighbours.Add(null);
                            Neighbours[NeighbourIndex] = null;
                            NeighbourConnections[NeighbourIndex] = NodeNeighbourConnection.None;
                        }
                        ++NeighbourIndex;
                    }
                }
            }
            IsCornerTurn = DetermineCorners(Neighbours, NeighbourConnections);

            _NodeTile.NeighbouringTiles = Neighbours;
            _NodeTile.NeighbourConnectionsType = NeighbourConnections;
            _NodeTile.ConnectionCorners = IsCornerTurn;

            //int WalkableIndex = 0;
            //for(int NeighIndex = 0; NeighIndex < Neighbours.Count; ++NeighIndex)
            //{
            //    if()
            //}

            //if (Neighbours.Count == 7 || Neighbours.Count <= 4)
            //    _NodeTile.IsCorner = true;

            //1.414214
            //return Neighbours.ToArray();
        }

        //
        NodeNeighbourConnection DetermineConnectionRelationship(Node _CentreNode, Node _NeighbourNode, bool _IsDiagnal)
        {
            float AJDistance = GridXScale;
            float HypDistance = Vector3.Distance(_CentreNode.WorldPosition, _NeighbourNode.WorldPosition);
            float HeightDifference = (_NeighbourNode.WorldPosition.y - _CentreNode.WorldPosition.y);
            float IsNegative = Mathf.Sign(HeightDifference);
            HeightDifference = Mathf.Abs(HeightDifference);
            if (_IsDiagnal)
                AJDistance = GridXScale * 1.4f;

            float AngleBetweenNodes = Mathf.Sin(HeightDifference / HypDistance);
            if (AngleBetweenNodes >= InclineConnectionMin && AngleBetweenNodes <= InclineConnectionMax)
                return NodeNeighbourConnection.Incline;

            if ((HeightDifference <= 0.05f))
                return NodeNeighbourConnection.Flat;
            if ((HeightDifference > 0.05f) && (HeightDifference < JumpIntervalHeight))
                    return NodeNeighbourConnection.Hop;

            if ((HeightDifference >= JumpIntervalHeight) && (HeightDifference < ClimbIntervalHeight))
                    return NodeNeighbourConnection.Jump;

            if ((HeightDifference >= ClimbIntervalHeight) && (HeightDifference < UnScalableIntervalHeight))
            {
                if(IsNegative == -1)
                    return NodeNeighbourConnection.Drop;
                else
                    return NodeNeighbourConnection.Climb;
            }
            if ((HeightDifference >= UnScalableIntervalHeight))
                return NodeNeighbourConnection.UnScaleable;

            return NodeNeighbourConnection.None;
        }

        // DODO: Need to include edge tiles as corners too!!!! DOn't know if this is working
        bool[] DetermineCorners(Node[] _Neighbours, NodeNeighbourConnection[] _NeighbourConnections)
        {
            bool[] Corners = new bool[_NeighbourConnections.Length];

            if (_Neighbours[0] != null)
                if ((int)_NeighbourConnections[1] >= 3 || (int)_NeighbourConnections[3] >= 3)
                    Corners[0] = true;

            if (_Neighbours[2] != null)
                if ((int)_NeighbourConnections[1] >= 3 || (int)_NeighbourConnections[4] >= 3)
                    Corners[2] = true;

            if (_Neighbours[5] != null)
                if ((int)_NeighbourConnections[6] >= 3 || (int)_NeighbourConnections[3] >= 3)
                    Corners[5] = true;

            if (_Neighbours[7] != null)
                if ((int)_NeighbourConnections[6] >= 3 || (int)_NeighbourConnections[4] >= 3)
                    Corners[7] = true;
            //for (int NeighIndex = 0; NeighIndex < _Neighbours.Length; ++NeighIndex)
            //{
            //    if(_Neighbours[NeighIndex] != null)
            //    {
            //        if(NeighIndex == 0 || NeighIndex == 2 || NeighIndex == 5|| NeighIndex == 7)
            //        {
            //            int IncreasingIndex = NeighIndex + 1;
            //            int DecreasingIndex = NeighIndex - 1;
            //            if (IncreasingIndex > _Neighbours.Length - 1)
            //                IncreasingIndex = 0;
            //            if (DecreasingIndex < 0)
            //                DecreasingIndex = _Neighbours.Length - 1;

            //            if ((int)_NeighbourConnections[IncreasingIndex] >= 2 || (int)_NeighbourConnections[DecreasingIndex] >= 2)
            //                Corners[NeighIndex] = true;
            //        }
            //    }
            //}
            return Corners;
        }








        //
        public void TestTile()
        {
            FoundRandNode = null;
            Node RandNode = GetRandNode();
            if (RandNode == null)
            {
                Debug.Log("No Node Found");
                return;
            }

            if(RandNode != null)
            {
                FoundRandNode = RandNode;
                Debug.Log(RandNode.GridPosition.X + "/" + RandNode.GridPosition.Y);

                //if(RandNode.NeighbouringTiles[0] == null)
                //    Debug.Log("NO Neighbour");
                //if (RandNode.NeighbourConnectionsType[0] == null)
                //    Debug.Log("NO Connections");
                //if (RandNode.ConnectionCorners[0] == null)
                //    Debug.Log("NO Corners");
            }
        }


        public void ResetGrid()
        {
            if(NodeSetupThread != null)
                NodeSetupThread.Abort();
            NodeGrid = null;
            CurrentGridState = ProgressState.Inactive;
            CurrentNeighbourState = ProgressState.Inactive;
            FloorXLength = 0;
            FloorYLength = 0;
            FloorElevation = 0;
            FloorCollider = null;
            NodeHight = 0;
            GridXLength = 0;
            GridYLength = 0;
            GridXScale = 0;
            GridYScale = 0;
            FloorCheckDistance = 0;
            CheckOverLap = 0;
            CombinedCheckDistance = 0;
            GridStartPos = Vector3.zero;
            FoundRandNode = null;
    }



        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + new Vector3(0, NodeElevationFromCollider + FloorElevation, 0), new Vector3(FloorXLength, NodeHight, FloorYLength));

            if (NodeGrid != null && CurrentGridState == ProgressState.Complete)
            {
                if(VisualDebugging)
                {
                    for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
                        for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                        {
                            if (NodeGrid[XIndex, YIndex] != null)
                            {
                                Gizmos.color = Color.cyan;
                                if (NodeGrid[XIndex, YIndex].IsCorner)
                                    Gizmos.color = Color.blue;
                                if (NodeGrid[XIndex, YIndex].NodeAngle == Vector3.up)
                                    Gizmos.DrawWireCube(NodeGrid[XIndex, YIndex].WorldPosition, NodeGrid[XIndex, YIndex].TileSize);
                                else
                                {
                                    Gizmos.DrawWireSphere(NodeGrid[XIndex, YIndex].WorldPosition, 0.5f);
                                }
                            }
                        }
                }

                if (FoundRandNode != null)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireCube(FoundRandNode.WorldPosition, FoundRandNode.TileSize);

                    for (int NeighIndex = 0; NeighIndex < FoundRandNode.NeighbouringTiles.Length; ++NeighIndex)
                    {
                        Gizmos.color = Color.clear;
                        if (FoundRandNode.NeighbourConnectionsType[NeighIndex] == NodeNeighbourConnection.Flat)
                            Gizmos.color = Color.cyan;
                        if (FoundRandNode.NeighbourConnectionsType[NeighIndex] == NodeNeighbourConnection.Hop)
                            Gizmos.color = Color.blue;
                        if (FoundRandNode.NeighbourConnectionsType[NeighIndex] == NodeNeighbourConnection.Incline)
                            Gizmos.color = Color.grey;
                        if (FoundRandNode.NeighbourConnectionsType[NeighIndex] == NodeNeighbourConnection.Jump)
                            Gizmos.color = Color.green;
                        if (FoundRandNode.NeighbourConnectionsType[NeighIndex] == NodeNeighbourConnection.Climb)
                            Gizmos.color = Color.yellow;
                        if (FoundRandNode.NeighbourConnectionsType[NeighIndex] == NodeNeighbourConnection.Drop)
                            Gizmos.color = Color.red;
                        if (FoundRandNode.NeighbourConnectionsType[NeighIndex] == NodeNeighbourConnection.UnScaleable)
                            Gizmos.color = Color.white;
                        if(FoundRandNode.NeighbouringTiles[NeighIndex] != null)
                            Gizmos.DrawWireCube(FoundRandNode.NeighbouringTiles[NeighIndex].WorldPosition, FoundRandNode.TileSize);

                        if(FoundRandNode.ConnectionCorners[NeighIndex] == true)
                        {
                            Gizmos.color = Color.magenta;
                            Gizmos.DrawWireSphere(FoundRandNode.NeighbouringTiles[NeighIndex].WorldPosition, 0.3f);
                        }
                    }
                }
            }
        }



        #region Utility Methods

        //
        public Node FindNodeFromWorldPosition(Vector3 _WorldPos)
        {
            float XIndex = _WorldPos.x * GridXScale;
            float YIndex = _WorldPos.z * GridYScale;

            int XIntIndex = Mathf.RoundToInt(XIndex);
            int YIntIndex = Mathf.RoundToInt(YIndex);

            if (XIntIndex > GridXLength || YIntIndex > GridYLength || XIntIndex < 0 || YIntIndex < 0)
            {
                return null;
            }

            Node ReturnNode = NodeGrid[XIntIndex, YIntIndex];
            if (ReturnNode != null)
            {
                return ReturnNode;
            }
            else
                return null;
        }

        //
        public Node GetRandNode()
        {
            int Ranx = UnityEngine.Random.Range(0, GridXLength);
            int RanY = UnityEngine.Random.Range(0, GridYLength);
            Vector3 RandVec = new Vector3(Ranx, 0, RanY);
            Node Node = FindNodeFromWorldPosition(RandVec);

            return Node;
        }

        #endregion
    }
}
