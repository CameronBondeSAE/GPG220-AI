using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunCrewSpawnManager : MonoBehaviour
    {
        public static GunCrewSpawnManager Instance;

        public enum GunSpawnStatus
        {
            UnSpawned,
            SpawnRequested,
            GunDropPointFound,
            GunDroppingIn,
            GunLanded,
            DroppingCrew,
            CrewLanded,
            Ready
        }

        public enum GCSMStatus
        {
            Inactive,
            SearchingForDropZone,
            SpawningGun,
            SpawningCrew,
            CleaningUp
        }

        [Header("Status")]
        public GCSMStatus CurrentStatus = GCSMStatus.Inactive;

        [Space]
        [Header("Prefabs")]
        public GameObject GunPrefab;
        public GameObject GunCrewMemberPrefab;

        [Space]
        [Header("Search Parameters")]
        public int GunCrewAreaX = 3;
        public int GunCrewAreaY = 3;
        public int DepthOfScan = 10;

        [Space]
        [Header("Operational Variables")]
        public Queue<GunCrew> CallRequests = new Queue<GunCrew>();
        public GunCrew CurrentRequestee;

        //Temp Variables
        Node DropPointLocation;
        Node DropPointLocationSpawnCentre;
        List<Node> CrewMemberSpawnLocations;
        List<Node> PosibleEdgeDropSections;

        [Space]
        [Header("Visual Debugging")]
        public bool VisualDebugging = false;



        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            CheckForRequests();
        }

        //
        public void RequestSpawn(GunCrew _Requestee)
        {
            if(_Requestee != null)
                CallRequests.Enqueue(_Requestee);
        }

        //
        void CheckForRequests()
        {
            if(CallRequests.Count > 0)
            {
                if (CurrentStatus != GCSMStatus.Inactive || CurrentRequestee != null)
                    return;

                StartCoroutine(ProcessSpaqnRequest(CallRequests.Dequeue()));
            }
        }

        //Yeah could be moved into a switch and then a shift call could be made to move onto the next step...
        IEnumerator ProcessSpaqnRequest(GunCrew _Requestee)
        {
            CurrentRequestee = _Requestee;
            CurrentStatus = GCSMStatus.SearchingForDropZone;
            ScanForDropZone();
            yield return new WaitUntil(() => CurrentRequestee.CurrentSpawnStatus ==  GunSpawnStatus.GunDropPointFound);

            if (DropPointLocation != null && DropPointLocationSpawnCentre != null)
            {
                CurrentStatus = GCSMStatus.SpawningGun;
                DropInGun();
                yield return new WaitUntil(() => CurrentRequestee.CurrentSpawnStatus == GunSpawnStatus.GunLanded);
            }
            if(CurrentRequestee.CurrentSpawnStatus == GunSpawnStatus.GunLanded)
            {
                CurrentStatus = GCSMStatus.SpawningCrew;
                DropInGunCrew(_Requestee);
                yield return new WaitUntil(() => CurrentRequestee.CurrentSpawnStatus == GunSpawnStatus.CrewLanded);
            }
            if(CheckSpawnIsCompleate())
            {
                CurrentStatus = GCSMStatus.CleaningUp;
                CurrentRequestee.CurrentSpawnStatus = GunSpawnStatus.Ready;
                ResetSpawner();
            }
        }

        //
        void ScanForRandomDropZone()
        {

        }

        //
        void ScanForDropZone()
        {
            DropPointLocation = ScanForMapEdgesForDropZone(GunCrewAreaX, GunCrewAreaY, DepthOfScan);
            if (DropPointLocationSpawnCentre != null)
            {
                CurrentRequestee.SpawnNode = DropPointLocationSpawnCentre;
                CurrentRequestee.CurrentSpawnStatus = GunSpawnStatus.GunDropPointFound;
            }
        }

        //
        Node ScanForMapEdgesForDropZone(int _GunCrewAreaX, int _GunCrewAreaY, int _DepthOfScanBox)
        {
            int RequiredXLengh = _GunCrewAreaX + 2;
            int RequiredYLengh = _GunCrewAreaY + 2;

            //Aquires possible spawn locations based on scan distance to scan through.
            if (PosibleEdgeDropSections == null)
            {
                List<Node> NodeSections = GrabNodeScanSections(_DepthOfScanBox);
                PosibleEdgeDropSections = NodeSections;
            }
            int ListIndex = PosibleEdgeDropSections.Count;

            //For Random SpawnLocations along edge
            for (int SectionIndex = ListIndex; SectionIndex > 0; --SectionIndex)
            {
                int RanIndex = Random.Range(0, PosibleEdgeDropSections.Count);
                Node NV2I = PosibleEdgeDropSections[RanIndex];
                Node[] SectionNodes = ReturnScanAreaNodes(NV2I.GridPostion, _DepthOfScanBox, RequiredXLengh, RequiredYLengh).ToArray();

                for (int SectionNodeIndex = 0; SectionNodeIndex < SectionNodes.Length; ++SectionNodeIndex)
                {
                    bool DropLocationFound = ScanNodeLocation(RequiredXLengh, RequiredYLengh, SectionNodes[SectionNodeIndex], out DropPointLocationSpawnCentre, out CrewMemberSpawnLocations);
                    if (DropLocationFound == true)
                    {
                        PosibleEdgeDropSections.RemoveAt(RanIndex);
                        return SectionNodes[SectionNodeIndex];
                    }
                    PosibleEdgeDropSections.RemoveAt(RanIndex);
                }
            }
            // For Systematic spawn locations
            //for (int SectionIndex = 0; SectionIndex < NodeSections.Count; ++SectionIndex)
            //{
            //    Node NV2I = NodeSections[SectionIndex];
            //    Node[] SectionNodes = ReturnScanAreaNodes(NV2I.GridPostion, _DepthOfScanBox, RequiredXLengh, RequiredYLengh).ToArray();

            //    for(int SectionNodeIndex = 0; SectionNodeIndex < SectionNodes.Length; ++SectionNodeIndex)
            //    {
            //        bool DropLocationFound = ScanNodeLocation(_GunCrewAreaX, _GunCrewAreaY, SectionNodes[SectionNodeIndex]);
            //        if(DropLocationFound == true)
            //        {
            //            return SectionNodes[SectionNodeIndex];
            //        }
            //    }
            //}
            return null;
        }

        //
        List<Node> GrabNodeScanSections(int _DepthOfScan)
        {
            List<Node> ListOfNodesToQueue = new List<Node>();

            for (int XIndex = 0; XIndex < NodeManager.Instance.GridXLength; XIndex += _DepthOfScan)
            {
                if (XIndex > NodeManager.Instance.GridXLength - 1)
                    continue;
                if (NodeManager.Instance.NodeGrid[XIndex, 0] != null)
                    ListOfNodesToQueue.Add(NodeManager.Instance.NodeGrid[XIndex, 0]);
                if (NodeManager.Instance.NodeGrid[XIndex, NodeManager.Instance.GridYLength - 1] != null)
                    ListOfNodesToQueue.Add(NodeManager.Instance.NodeGrid[XIndex, NodeManager.Instance.GridYLength - 1]);
            }
            for (int YIndex = 0; YIndex < NodeManager.Instance.GridYLength; YIndex += _DepthOfScan)
            {
                if (YIndex > NodeManager.Instance.GridYLength - 1)
                    continue;
                if (NodeManager.Instance.NodeGrid[0, YIndex] != null)
                    ListOfNodesToQueue.Add(NodeManager.Instance.NodeGrid[0, YIndex]);
                if (NodeManager.Instance.NodeGrid[NodeManager.Instance.GridXLength - 1, YIndex] != null)
                    ListOfNodesToQueue.Add(NodeManager.Instance.NodeGrid[NodeManager.Instance.GridXLength - 1, YIndex]);
            }

            return ListOfNodesToQueue;
        }

        //
        List<Node> ReturnScanAreaNodes(Vector2Int _AreaScanOriginPoint, int _DepthOfScanBox, int _GunCrewAreaX, int _GunCrewAreaY)
        {
            List<Node> ListOfNodesToScan = new List<Node>();
            bool StartingXIterationValue = true;
            bool StartingYIterationValue = true;

            if (_AreaScanOriginPoint.X > (NodeManager.Instance.GridXLength / 2 + _GunCrewAreaX))
                StartingXIterationValue = false;

            if (_AreaScanOriginPoint.Y > (NodeManager.Instance.GridYLength / 2 + _GunCrewAreaY))
                StartingYIterationValue = false;

            for (int XIndex = 0; XIndex < _DepthOfScanBox; ++XIndex)
            {
                for (int YIndex = 0; YIndex < _DepthOfScanBox; ++YIndex)
                {
                    int ModIndexX = XIndex;
                    int ModIndexY = YIndex;
                    if (!StartingXIterationValue)
                        ModIndexX = -ModIndexX;
                    if (!StartingYIterationValue)
                        ModIndexY = -ModIndexY;

                    if (NodeManager.Instance.NodeGrid[(_AreaScanOriginPoint.X + ModIndexX), (_AreaScanOriginPoint.Y + ModIndexY)] != null)
                        if (!NodeManager.Instance.NodeGrid[(_AreaScanOriginPoint.X + ModIndexX), (_AreaScanOriginPoint.Y + ModIndexY)].IsOccupied)
                            ListOfNodesToScan.Add(NodeManager.Instance.NodeGrid[(_AreaScanOriginPoint.X + ModIndexX), (_AreaScanOriginPoint.Y + ModIndexY)]);
                }
            }
            return ListOfNodesToScan;
        }

        //
        bool ScanNodeLocation(int _GunCrewAreaX, int _GunCrewAreaY, Node _Node, out Node _SpawnCentre, out List<Node> _CrewLocations)
        {
            bool StartingXIterationValue = true;
            bool StartingYIterationValue = true;

            _SpawnCentre = null;
            int CentreXIndex = Mathf.CeilToInt(_GunCrewAreaX / 2);
            int CentreYIndex = Mathf.CeilToInt(_GunCrewAreaY / 2);

            List<Node> CrewSpawnLocations = new List<Node>();
            _CrewLocations = CrewSpawnLocations;

            if (_Node.GridPostion.X > (NodeManager.Instance.GridXLength / 2 + _GunCrewAreaX))
                StartingXIterationValue = false;

            if (_Node.GridPostion.Y > (NodeManager.Instance.GridYLength / 2 + _GunCrewAreaY))
                StartingYIterationValue = false;

            for (int XIndex = 0; XIndex < _GunCrewAreaX; ++XIndex)
            {
                for (int YIndex = 0; YIndex < _GunCrewAreaY; ++YIndex)
                {
                    int ModIndexX = XIndex;
                    int ModIndexY = YIndex;
                    if (!StartingXIterationValue)
                        ModIndexX = -ModIndexX;
                    if (!StartingYIterationValue)
                        ModIndexY = -ModIndexY;

                    if (NodeManager.Instance.NodeGrid[(_Node.GridPostion.X + ModIndexX), (_Node.GridPostion.Y + ModIndexY)] == null)
                        return false;
                    if (NodeManager.Instance.NodeGrid[(_Node.GridPostion.X + ModIndexX), (_Node.GridPostion.Y + ModIndexY)].IsOccupied == true)
                        return false;

                    if (XIndex == CentreXIndex && YIndex == CentreYIndex)
                        _SpawnCentre = NodeManager.Instance.NodeGrid[(_Node.GridPostion.X + ModIndexX), (_Node.GridPostion.Y + ModIndexY)];

                    if (XIndex == 0 || XIndex == _GunCrewAreaX - 1)
                        CrewSpawnLocations.Add(NodeManager.Instance.NodeGrid[(_Node.GridPostion.X + ModIndexX), (_Node.GridPostion.Y + ModIndexY)]);
                    if (YIndex == 0 || YIndex == _GunCrewAreaY - 1)
                        CrewSpawnLocations.Add(NodeManager.Instance.NodeGrid[(_Node.GridPostion.X + ModIndexX), (_Node.GridPostion.Y + ModIndexY)]);
                }
            }
            return true;
        }

        //
        void DropInGun()
        {
            if (DropPointLocation != null && DropPointLocationSpawnCentre != null)
            {
                CurrentRequestee.SpawnNode = DropPointLocationSpawnCentre;
                CurrentRequestee.Gun = Instantiate(GunPrefab, DropPointLocationSpawnCentre.WorldPosition, Quaternion.identity, CurrentRequestee.transform);
                CurrentRequestee.CallAnimateGunSpawnIn();
            }
        }

        //
        void DropInGunCrew(GunCrew _requestee)
        {
            _requestee.CrewMembers = new GunCrewMember[4];
            if (_requestee.CurrentSpawnStatus == GunSpawnStatus.GunLanded)
            {
                for (int CrewMemberIndex = 0; CrewMemberIndex < _requestee.CrewMembers.Length; ++CrewMemberIndex)
                {
                    int RandInt = Random.Range(0, CrewMemberSpawnLocations.Count);
                    Node SpawnNode = CrewMemberSpawnLocations[RandInt];

                    GunCrewMember NewCrew = Instantiate(GunCrewMemberPrefab,_requestee.transform).GetComponent<GunCrewMember>();
                    _requestee.CrewMembers[CrewMemberIndex] = NewCrew;
                    NewCrew.CallAnimateSpawnIn(_requestee, SpawnNode);
                    CrewMemberSpawnLocations.RemoveAt(RandInt);
                }
                _requestee.AssignCrewRoles();
            }
        }

        bool CheckSpawnIsCompleate()
        {
            return true;
        }

        //
        void ResetSpawner()
        {
            CurrentRequestee = null;
            DropPointLocation = null;
            DropPointLocationSpawnCentre = null;
            CrewMemberSpawnLocations = null;
            CurrentStatus = GCSMStatus.Inactive;
        }

        //These Visual Debuggings are really really costly and inefficent!!
        private void OnDrawGizmos()
        {
            if (VisualDebugging)
            {
                if (!NodeManager.Instance.SetupCompletate)
                    return;

                if (DropPointLocation != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(DropPointLocation.WorldPosition, DropPointLocation.TileSize / 3);
                }
                if (DropPointLocationSpawnCentre != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(DropPointLocationSpawnCentre.WorldPosition, DropPointLocationSpawnCentre.TileSize / 3);
                }

                if (CrewMemberSpawnLocations != null)
                {
                    for (int XIndex = 0; XIndex < CrewMemberSpawnLocations.Count; XIndex++)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireCube(CrewMemberSpawnLocations[XIndex].WorldPosition, CrewMemberSpawnLocations[XIndex].TileSize / 2);
                    }
                }

                if (PosibleEdgeDropSections != null)
                    for (int XIndex = 0; XIndex < PosibleEdgeDropSections.Count; XIndex++)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawWireCube(PosibleEdgeDropSections[XIndex].WorldPosition, PosibleEdgeDropSections[XIndex].TileSize / 2);
                    }
            }
        }
    }
}