using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Trystin
{
    public class NodeManager : MonoBehaviour
    {
        public static NodeManager Instance;

        public enum ProgressState
        {
            Inactive,
            InProgress,
            Complete
        }

        public enum PingState
        {
            Inactive,
            Sweeping,
            Culling,
            Complete
        }

        [Header("Grid Variables")]
        public int GridXLength = 100;
        public int GridYLength = 100;
        public float GridXScale = 1;
        public float GridYScale = 1;
        public Vector3 WorldPositionOffset = new Vector3(-50, 0.3f, -50);

        public int NumberOfQuadrentsX = 3;
        public int NumberOfQuadrentsY = 3;
        public float QuadrentScaleX { get{ return GridXLength / NumberOfQuadrentsX; } }
        public float QuadrentScaleY { get { return GridYLength / NumberOfQuadrentsY; } }


        public Node[,] NodeGrid;
        public Quadrent[,] QuadrentNodeGrid = new Quadrent[3, 3];

        [Space]
        [Header("Setup Variables")]
        public int SweepInterval = 200;
        public int RemovalInterval = 50;
        public int DetectionFrameSkip = 3;
        int CurrentFrame = 0;
        public float SetupTotalTime = -1;
        public bool SetupCompletate;


        public Coroutine CurrentPing;
        Action<CharacterBase> AddEntirtesAction;
        Action<CharacterBase> RemoveEntitiesAction;


        [Space]
        [Header("Setup Status")]
        public ProgressState CurrentGridState = ProgressState.Inactive;
        public PingState CurrentPingState = PingState.Inactive;
        public ProgressState CurrentNeighbourState = ProgressState.Inactive;


        [Space]
        [Header("Monitoring Lists")]
        public List<Node> TerrainOccupiedNodes = new List<Node>();
        public List<CharacterBase> ActiveAICB = new List<CharacterBase>();
        public List<Node> AIOccupiedNodes = new List<Node>();

        [Space]
        [Header("Debugging")]
        public bool ToggleWireFrameGridNodes = false;
        public bool ToggleWireFrameQuadrants = false;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        private void Start()
        {
            CallInitialSetup();
        }

        private void Update()
        {
            if (SetupCompletate == false)
                SetupTotalTime += Time.deltaTime;

            if (SetupCompletate)
            {
                if (CurrentFrame == DetectionFrameSkip)
                {
                    DectectAIGridLocations();
                    CurrentFrame = 0;
                }
                else
                    ++CurrentFrame;
            }
        }

        #region Initial Setup Calls

        //
        public void CallInitialSetup()
        {
            if (CurrentGridState != ProgressState.Complete && NodeGrid == null)
                StartCoroutine(InitialSetup());
        }
        IEnumerator InitialSetup()
        {
            AddEntirtesAction = AddCreatedEntities;
            RemoveEntitiesAction = RemoveEntities;
            CharacterBase.OnSpawned += AddEntirtesAction;
            CharacterBase.OnDestroyed += RemoveEntitiesAction;

            yield return new WaitForSeconds(1f);

            CallCreateGrid();

            yield return new WaitUntil(() => CurrentGridState == ProgressState.Complete);
            CallPing(true);

            yield return new WaitUntil(() => CurrentPingState == PingState.Complete);
            NeighbourSetThreadCall();

            yield return new WaitUntil(() => CurrentNeighbourState == ProgressState.Complete);
            SetupCompletate = true;
            PathFinderManager.Instance.PathFinderIsActive = true;
        }
        void AddCreatedEntities(CharacterBase _Entity)
        {
            if (_Entity != null)
                ActiveAICB.Add(_Entity);
        }
        void RemoveEntities(CharacterBase _Entity)
        {
            if (_Entity != null)
                ActiveAICB.Remove(_Entity);
        }

        #endregion

        #region GridNodeSetup

        //
        void CallCreateGrid()
        {
            if (NodeGrid != null)
                return;

            CurrentGridState = ProgressState.InProgress;
            Thread GridCreationThread = new Thread(CreateGrindThreadCall);
            GridCreationThread.Start();
        }
        void CreateGrindThreadCall()
        {
            //Debug.Log("NM:    Grid Creation Started");
            int CurrentNodeCount = 0;

            Node[,] NewTileGrid = new Node[GridXLength, GridYLength];
            for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
            {
                for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                {
                    ++CurrentNodeCount;
                    Node NewNodeScript = new Node();
                    NewNodeScript.GridPosition = new Vector2Int(XIndex, YIndex);
                    NewNodeScript.WorldPosition = new Vector3((NewNodeScript.GridPosition.X * GridXScale), 0.3f, (NewNodeScript.GridPosition.Y * GridYScale)) + WorldPositionOffset;
                    NewNodeScript.TileSize = new Vector3(GridXScale - 0.01f, 0.1f, GridYScale - 0.01f);
                    NewTileGrid[XIndex, YIndex] = NewNodeScript;
                    NewNodeScript.GridPosition = new Vector2Int(XIndex, YIndex);
                }
            }
            NodeGrid = NewTileGrid;
            CurrentGridState = ProgressState.Complete;
            //Debug.Log("NM:    Grid Creation Finished");
        }

        //
        void NeighbourSetThreadCall()
        {
            CurrentNeighbourState = ProgressState.InProgress;
            Thread NeighbourSetThread = new Thread(SetTileNeighbours);
            NeighbourSetThread.Start();
        }
        void SetTileNeighbours()
        {
            //Debug.Log("NM:    Setting up Node Neighbours");

            if (NodeGrid == null)
                return;

            for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
                for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                    if (NodeGrid[XIndex, YIndex] != null)
                        NodeGrid[XIndex, YIndex].NeighbouringTiles = GetNeighbouringNodeTiles(NodeGrid[XIndex, YIndex]);

            CurrentNeighbourState = ProgressState.Complete;
            //Debug.Log("NM:    Node Neighbours setup compleate!!");
        }

        //
        public Node[] GetNeighbouringNodeTiles(Node _NodeTile)
        {
            List<Node> Neighbours = new List<Node>();

            if (_NodeTile == null)
                return Neighbours.ToArray();

            for (int XIndex = -1; XIndex <= 1; XIndex++)
            {
                for (int ZIndex = -1; ZIndex <= 1; ZIndex++)
                {
                    if (XIndex == 0 && ZIndex == 0)
                    {
                        continue;
                    }
                    int CheckX = _NodeTile.GridPosition.X + XIndex;
                    int CheckZ = _NodeTile.GridPosition.Y + ZIndex;

                    if (CheckX >= 0 && CheckX < GridXLength && CheckZ >= 0 && CheckZ < GridYLength)
                        if (NodeGrid[CheckX, CheckZ] != null)
                            Neighbours.Add(NodeGrid[CheckX, CheckZ]);
                }
            }
            if (Neighbours.Count == 7 || Neighbours.Count <= 4)
                _NodeTile.IsCorner = true;

            return Neighbours.ToArray();
        }

        //
        void SetupQuardrents()
        {
            float QuadrentNodeIntervalX = QuadrentScaleX;
            float QuadrentNodeIntervalY = QuadrentScaleY;
            int EnumCast = 0;

            //Initial Setup
            for (int XIndex = 0; XIndex < 3; XIndex++)
            {
                for (int YIndex = 0; YIndex < 3; YIndex++)
                {
                    Quadrent NewQuad = new Quadrent();
                    QuadrentNodeGrid[XIndex, YIndex] = NewQuad;
                    NewQuad.GridPosition = new Vector2Int(XIndex, YIndex);

                    NewQuad.WorldPosition = new Vector3(((QuadrentNodeIntervalX * (XIndex + 1)) - (QuadrentNodeIntervalX / 2)), 0.3f, ((QuadrentNodeIntervalY * (YIndex + 1)) - (QuadrentNodeIntervalY / 2)));
                    NewQuad.QuadrentSize = new Vector3(QuadrentNodeIntervalX, 0.3f, QuadrentNodeIntervalY);
                    NewQuad.QuadrentPostion = (Direction)EnumCast;
                    ++EnumCast;
                }
            }
            //Neighbour Setup
            for (int XIndex = 0; XIndex < 3; ++XIndex)
                for (int YIndex = 0; YIndex < 3; ++YIndex)
                    if (QuadrentNodeGrid[XIndex, YIndex] != null)
                    {
                        Quadrent CurrentQuad = QuadrentNodeGrid[XIndex, YIndex];
                        List<Quadrent> Neighbours = new List<Quadrent>();

                        for (int XNeighbourIndex = -1; XNeighbourIndex <= 1; XNeighbourIndex++)
                        {
                            for (int ZIndex = -1; ZIndex <= 1; ZIndex++)
                            {
                                if (XNeighbourIndex == 0 && ZIndex == 0)
                                {
                                    continue;
                                }
                                int CheckX = CurrentQuad.GridPosition.X + XNeighbourIndex;
                                int CheckZ = CurrentQuad.GridPosition.Y + ZIndex;

                                if (CheckX >= 0 && CheckX < 3 && CheckZ >= 0 && CheckZ < 3)
                                    if (QuadrentNodeGrid[CheckX, CheckZ] != null)
                                        Neighbours.Add(QuadrentNodeGrid[CheckX, CheckZ]);
                            }
                        }
                        CurrentQuad.NeighbouringTiles = Neighbours.ToArray();
                    }
        }

        #endregion

        #region GridPing

        //
        public void CallPing(bool _ObsticleCleanUp)
        {
            if (NodeGrid == null)
                return;
            CurrentPing = StartCoroutine(PingGrid(_ObsticleCleanUp));
        }
        IEnumerator PingGrid(bool _ObsticleCleanUp)
        {
            //Debug.Log("NM:    PingStarted");

            CallTerrainSweep();

            if (_ObsticleCleanUp)
            {
                yield return new WaitUntil(() => CurrentPingState == PingState.Inactive);
                CallCleanUpObsticleTiles();
            }

            yield return new WaitUntil(() => CurrentPingState == PingState.Complete);
            //Debug.Log("NM:    PingEnded");
        }

        //
        void CallTerrainSweep()
        {
            CurrentPingState = PingState.Sweeping;
            StartCoroutine(SweepTerrain());
        }
        IEnumerator SweepTerrain()
        {
            Collider FloorCollider = FindFloor();

            //Debug.Log("NM:    Sorting Terrain from AI");
            int CurrentNodeCount = 0;

            for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
            {
                for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                {
                    ++CurrentNodeCount;

                    switch (NodeGrid[XIndex, YIndex].ColliderOverlapCheck(NodeGrid[XIndex, YIndex], FloorCollider))
                    {
                        case Node.ColliderOwnerType.Null:

                            break;
                        case Node.ColliderOwnerType.AI:
                            ActiveAICB.Add(NodeGrid[XIndex, YIndex].Occupant);
                            AIOccupiedNodes.Add(NodeGrid[XIndex, YIndex]);
                            break;
                        case Node.ColliderOwnerType.Object:
                            TerrainOccupiedNodes.Add(NodeGrid[XIndex, YIndex]);
                            break;
                    }

                    if (CurrentNodeCount % SweepInterval == 0)
                        yield return null;
                }
            }
            CurrentPingState = PingState.Inactive;
            //Debug.Log("NM:    Sorting Terrain from AI Complete!");
        }

        //
        void CallCleanUpObsticleTiles()
        {
            CurrentPingState = PingState.Culling;
            StartCoroutine(CleanUpObsticlesTiles());
            SetupQuardrents(); 
        }
        IEnumerator CleanUpObsticlesTiles()
        {
            //Debug.Log("NM:    Cleaning Up obsticle Tiles");
            int CurrentTerrainRemovalCount = 0;
            Vector2Int TempV2I = new Vector2Int();

            for (int ObTiles = 0; ObTiles < TerrainOccupiedNodes.Count; ++ObTiles)
            {
                TempV2I = TerrainOccupiedNodes[ObTiles].GridPosition;
                NodeGrid[TempV2I.X, TempV2I.Y] = null;
                ++CurrentTerrainRemovalCount;

                if (CurrentTerrainRemovalCount % RemovalInterval == 0)
                    yield return null;
            }
            TerrainOccupiedNodes = null;
            CurrentPingState = PingState.Complete;
            //Debug.Log("NM:    Cleaning Up obsticle Tiles Compleate!");
        }

        //
        Collider FindFloor()
        {
            Collider[] NC = Physics.OverlapBox(Vector3.zero, new Vector3(10, 10, 10));
            for (int ColIndex = 0; ColIndex < NC.Length; ColIndex++)
            {
                Floor FoundFloor = NC[ColIndex].GetComponent<Floor>();
                if (FoundFloor != null)
                    return NC[ColIndex];
            }
            return null;
        }

        #endregion

        #region VisualDebugging

        //
        private void OnDrawGizmos()
        {
            if (ToggleWireFrameGridNodes == false)
                return;

            if (NodeGrid != null && CurrentGridState == ProgressState.Complete)
            {
                for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
                    for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                        if (NodeGrid[XIndex, YIndex] != null)
                        {
                            Gizmos.color = Color.cyan;
                            if (NodeGrid[XIndex, YIndex].IsCorner)
                                Gizmos.color = Color.blue;
                            Gizmos.DrawWireCube(NodeGrid[XIndex, YIndex].WorldPosition, NodeGrid[XIndex, YIndex].TileSize);
                        }
            }

            if (TerrainOccupiedNodes != null)
            {
                for (int index = 0; index < TerrainOccupiedNodes.Count; ++index)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireCube(TerrainOccupiedNodes[index].WorldPosition, TerrainOccupiedNodes[index].TileSize);
                }
            }
            if (AIOccupiedNodes != null)
            {
                for (int index = 0; index < AIOccupiedNodes.Count; ++index)
                {
                    if (AIOccupiedNodes[index] != null)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(AIOccupiedNodes[index].WorldPosition, AIOccupiedNodes[index].TileSize);
                    }
                }
            }

            if(ToggleWireFrameQuadrants && CurrentNeighbourState == ProgressState.Complete)
            {
                for (int XIndex = 0; XIndex < 3; ++XIndex)
                    for (int YIndex = 0; YIndex < 3; ++YIndex)
                        if (QuadrentNodeGrid[XIndex, YIndex] != null)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(QuadrentNodeGrid[XIndex, YIndex].WorldPosition, QuadrentNodeGrid[XIndex, YIndex].QuadrentSize);
                        }
            }

            //if(PathFound != null)
            //{
            //    for (int NodeIndex = 0; NodeIndex < PathFound.Count; ++NodeIndex)
            //    {
            //        Gizmos.color = Color.red;
            //        Gizmos.DrawWireCube(PathFound[NodeIndex].WorldPosition, PathFound[NodeIndex].TileSize);
            //    }
            //}
        }
        #endregion

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
        public Quadrent FindQuadrentFromWorldPosition(Vector3 _WorldPos)
        {
            float XIndex = _WorldPos.x / QuadrentScaleX;
            float YIndex = _WorldPos.z / QuadrentScaleY;

            int XIntIndex = Mathf.FloorToInt(XIndex);
            int YIntIndex = Mathf.FloorToInt(YIndex);

            if (XIntIndex > NumberOfQuadrentsX || YIntIndex > NumberOfQuadrentsY || XIntIndex < 0 || YIntIndex < 0)
            {
                return null;
            }

            Quadrent ReturnNode = QuadrentNodeGrid[XIntIndex, YIntIndex];
            if (ReturnNode != null)
            {
                return ReturnNode;
            }
            else
                return null;
        }

        //
        void DectectAIGridLocations()
        {
            for (int NodeIndex = 0; NodeIndex < AIOccupiedNodes.Count; ++NodeIndex)
            {
                if (AIOccupiedNodes[NodeIndex] != null)
                {
                    AIOccupiedNodes[NodeIndex].IsOccupied = false;
                    AIOccupiedNodes[NodeIndex].Occupant = null;
                }
            }
            AIOccupiedNodes.Clear();

            for (int AIIndex = 0; AIIndex < ActiveAICB.Count; ++AIIndex)
            {
                if (ActiveAICB[AIIndex] != null)
                {
                    Vector3 AIPos = ActiveAICB[AIIndex].transform.position;

                    Collider[] AIColArray = ActiveAICB[AIIndex].GetComponentsInChildren<Collider>();
                    if(AIColArray.Length > 1)
                        for(int ColIndex = 0; ColIndex < AIColArray.Length; ++ColIndex)
                        {
                            Node ColNode = FindNodeFromWorldPosition(AIColArray[ColIndex].bounds.center);
                            if (ColNode != null)
                            {
                                ColNode.IsOccupied = true;
                                ColNode.Occupant = ActiveAICB[AIIndex];
                                AIOccupiedNodes.Add(ColNode);
                            }
                        }

                    Collider AICol = ActiveAICB[AIIndex].GetComponent<Collider>();
                    if(AICol == null)
                        AICol = ActiveAICB[AIIndex].GetComponentInChildren<Collider>();

                    if (AICol == null)
                    {
                        continue;
                    }
                    Vector3 BoundsExtents = AICol.bounds.extents;
                    Vector3[] Corners = new Vector3[4];
                    Node CentreNode = FindNodeFromWorldPosition(AIPos);

                    float XExtends = (BoundsExtents.x * 0.70f);
                    float ZExtends = (BoundsExtents.z * 0.70f);

                    Corners[0] = new Vector3(AIPos.x + XExtends, AIPos.y, AIPos.z + ZExtends);
                    Corners[1] = new Vector3(AIPos.x - XExtends, AIPos.y, AIPos.z - ZExtends);
                    Corners[2] = new Vector3(AIPos.x + XExtends, AIPos.y, AIPos.z - ZExtends);
                    Corners[3] = new Vector3(AIPos.x - XExtends, AIPos.y, AIPos.z + ZExtends);
                    //////MIGHT NEED TO ADD MORE HERE FOR N,S,E,W Positions .. Or just add a method to return all node between 2 nodes....

                    for (int PointIndex = 0; PointIndex < Corners.Length; ++PointIndex)
                    {
                        Node PointNode = FindNodeFromWorldPosition(Corners[PointIndex]);
                        if (PointNode != null && CentreNode != null)
                        {
                            PointNode.IsOccupied = true;
                            PointNode.Occupant = CentreNode.Occupant;
                            AIOccupiedNodes.Add(PointNode);
                        }
                    }
                    AIOccupiedNodes.Add(CentreNode);
                }
            }
        }

        //
        Node[] GetBetweenNodes()
        {

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
