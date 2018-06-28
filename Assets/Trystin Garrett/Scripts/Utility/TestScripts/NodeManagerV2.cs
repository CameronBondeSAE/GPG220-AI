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
        public float NodeHight = 0.25f;

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

        [Space]
        [Header("NodeRemoval/Relocaton Variables")]
        public float FloorCheckDistance;
        [SerializeField] private float CombinedCheckDistance;
        List<Node> RemovedNodes = new List<Node>();

        [Space]
        [Header("Debugging")]
        public Vector3 RemovedNodeSize;


        public void GetFloorGridDimentions()
        {
            FloorCollider = this.gameObject.GetComponent<Collider>();
            Bounds ColBounds = FloorCollider.bounds;
            FloorXLength = ColBounds.size.x;
            FloorYLength = ColBounds.size.z;
            //FloorElevation = ColBounds.extents.y;
            FloorElevation = ColBounds.size.y;
            GridStartPos = new Vector3((ColBounds.center.x - ColBounds.extents.x), (FloorElevation + NodeElevationFromCollider), (ColBounds.center.z - ColBounds.extents.z));

            GridXScale = NodeRatio.x;
            GridYScale = NodeRatio.y;
            FloorCheckDistance = ColBounds.size.y;
            NodePositionGridScaleOffset = new Vector2((GridXScale/2), (GridYScale / 2));

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
            for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
            {
                for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                {
                    //Vector3 CheckCentre = new Vector3(NodeGrid[XIndex, YIndex].WorldPosition.x, (NodeGrid[XIndex, YIndex].WorldPosition.y - (FloorCheckDistance/2)), NodeGrid[XIndex, YIndex].WorldPosition.z);              
                    //bool Check = Physics.CheckBox(CheckCentre, FloorCastCheck);
                    if(Physics.Raycast(NodeGrid[XIndex, YIndex].WorldPosition, transform.TransformDirection(Vector3.down), out LocalHit, CombinedCheckDistance))
                    {
                        float VerticalDistanceFromWorldPoint = LocalHit.distance - NodeElevationFromCollider;
                        RepositionNode(NodeGrid[XIndex, YIndex], VerticalDistanceFromWorldPoint);
                        //if (VerticalDistanceFromWorldPoint > 0.5f)
                        //{
                        //    RepositionNode(NodeGrid[XIndex, YIndex], VerticalDistanceFromWorldPoint);
                        //}


                        //if (CheckColliderDistance(LocalHit.collider, NodeGrid[XIndex, YIndex], VerticalDistanceFromWorldPoint))
                        //{
                        //    RemovedNodes.Add(NodeGrid[XIndex, YIndex]);
                        //    NodeGrid[XIndex, YIndex] = null;
                        //}
                        //else
                        //{

                        //}
                    }
                    else
                    {
                        RemovedNodes.Add(NodeGrid[XIndex, YIndex]);
                        NodeGrid[XIndex, YIndex] = null;
                    }
                }
            }
        }

        //
        void RepositionNode(Node _Node, float _Distance)
        {
            Vector3 PosDifference = new Vector3(0,-_Distance,0);
            _Node.WorldPosition += PosDifference;
        }

        //
        void DetermineConnectionRelationship()
        {

        }


        //
        //bool CheckColliderDistance(Collider _Col, Node _Node, float _YDistanceOffSet)
        //{
        //    bool NoGo = false;
        //    Vector3 PointOfCheck = new Vector3(_Node.WorldPosition.x, (_Node.WorldPosition.y + _YDistanceOffSet), _Node.WorldPosition.z);
        //    Vector3 ContactPoints = _Col.ClosestPoint(PointOfCheck);

        //    float XBounds = ContactPoints.x - _Node.WorldPosition.x;
        //    float YBounds = ContactPoints.z - _Node.WorldPosition.z;

        //    XBounds = Mathf.Abs(XBounds);
        //    YBounds = Mathf.Abs(YBounds);

        //    if (XBounds < 0.275f && XBounds > 0f && YBounds == 0f)
        //        NoGo = true;
        //    if (YBounds < 0.275f && YBounds > 0f && XBounds == 0f)
        //        NoGo = true;
        //    if (YBounds < 0.275f && XBounds < 0.275f)
        //        NoGo = true;
        //    return NoGo;
        //}


        public void ResetGrid()
        {
            NodeGrid = null;
            CurrentGridState = ProgressState.Inactive;
        }



        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(GridStartPos, 0.5f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + new Vector3(0, NodeElevationFromCollider + FloorElevation, 0), new Vector3(FloorXLength, NodeHight, FloorYLength));

            if (NodeGrid != null && CurrentGridState == ProgressState.Complete)
            {
                for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
                    for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                        if (NodeGrid[XIndex, YIndex] != null)
                        {
                            Gizmos.color = Color.cyan;
                            //if (NodeGrid[XIndex, YIndex].IsCorner)
                            //    Gizmos.color = Color.blue;
                            Gizmos.DrawWireCube(NodeGrid[XIndex, YIndex].WorldPosition, NodeGrid[XIndex, YIndex].TileSize);
                        }
            }
            if (NodeGrid != null && CurrentGridState == ProgressState.Complete)
            {
                for (int Node = 0; Node < RemovedNodes.Count; Node++)
                {
                    if (RemovedNodes[Node] != null)
                    {
                        Gizmos.color = Color.red;
                        Vector3 CheckCentre = new Vector3(RemovedNodes[Node].WorldPosition.x, (RemovedNodes[Node].WorldPosition.y - (CombinedCheckDistance / 2)), RemovedNodes[Node].WorldPosition.z);
                        Gizmos.DrawWireCube(CheckCentre, RemovedNodeSize);
                    }
                }
            }
        }
    }
}
