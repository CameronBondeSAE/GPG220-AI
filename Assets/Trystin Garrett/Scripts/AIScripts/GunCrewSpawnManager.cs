using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunCrewSpawnManager : MonoBehaviour
    {

        public enum GunSpawnStatus
        {
            UnSpawned,
            DroppingIn,
            Landed,
            DroppingCrew,
            Ready
        }

        public bool HasPreDefinedLocation = false;

        public GameObject GunPrefab;
        public GameObject Gun;
        public GameObject GunCrewMemberPrefabs;


        public GunCrewMember[] CrewMembers;
        public Vector3 GunCrewArea = new Vector3(3, 3, 3);
        //public GameObject GunCrewPrefab;

        public int GunCrewAreaX;
        public int GunCrewAreaY;
        public int DepthOfScan = 10;


        public Node DropPointLocation;
        public Node DropPointLocationSpawnCorner;
        public List<Node> CrewMemberSpawnLocations;
        public List<Node> PosibleDropSections;



        public bool VisualDebugging = false;
        public int PossibleDropZonePoints;

        private void Awake()
        {
            CheckForManagers();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void ScanForRandomDropZone()
        {

        }
        public void TestScanForDropZone()
        {
            if (NodeManager.Instance.SetupCompletate == true)
            {
                DropPointLocation = ScanForMapEdgesForDropZone(GunCrewAreaX, GunCrewAreaY, DepthOfScan);
                if (DropPointLocation != null && DropPointLocationSpawnCorner != null)
                {
                    Gun = Instantiate(GunPrefab, DropPointLocationSpawnCorner.WorldPosition, Quaternion.identity, transform);
                }
            }
        }

        Node ScanForMapEdgesForDropZone(int _GunCrewAreaX, int _GunCrewAreaY, int _DepthOfScanBox)
        {
            int RequiredXLengh = _GunCrewAreaX + 2;
            int RequiredYLengh = _GunCrewAreaY + 2;

            if (PosibleDropSections == null)
            {
                List<Node> NodeSections = GrabNodeScanSections(_DepthOfScanBox);
                PosibleDropSections = NodeSections;
            }


            int ListIndex = PosibleDropSections.Count;

            //For Random SpawnLocations
            for (int SectionIndex = ListIndex; SectionIndex > 0; --SectionIndex)
            {
                int RanIndex = Random.Range(0, PosibleDropSections.Count);
                Node NV2I = PosibleDropSections[RanIndex];
                Node[] SectionNodes = ReturnScanAreaNodes(NV2I.GridPostion, _DepthOfScanBox, RequiredXLengh, RequiredYLengh).ToArray();

                for (int SectionNodeIndex = 0; SectionNodeIndex < SectionNodes.Length; ++SectionNodeIndex)
                {
                    bool DropLocationFound = ScanNodeLocation(RequiredXLengh, RequiredYLengh, SectionNodes[SectionNodeIndex], out DropPointLocationSpawnCorner, out CrewMemberSpawnLocations);
                    if (DropLocationFound == true)
                    {
                        PosibleDropSections.RemoveAt(RanIndex);
                        return SectionNodes[SectionNodeIndex];
                    }
                    PosibleDropSections.RemoveAt(RanIndex);
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


        void DropInGun()
        {
            if (NodeManager.Instance.SetupCompletate == true)
            {
                DropPointLocation = ScanForMapEdgesForDropZone(GunCrewAreaX, GunCrewAreaY, DepthOfScan);
            }

            if (DropPointLocation != null && DropPointLocationSpawnCorner != null)
            {
                Gun = Instantiate(GunPrefab, DropPointLocationSpawnCorner.WorldPosition, Quaternion.identity, transform);
            }
        }

        void DropInGunCrew()
        {
            CrewMembers = new GunCrewMember[4];
            //if ()
            //{

            //}
        }


        //
        void CheckForManagers()
        {
            bool NMExists = false;
            bool PMExists = false;

            if (NodeManager.Instance != null)
                NMExists = true;
            if (PathFinderManager.Instance != null)
                PMExists = true;

            if (!NMExists)
            {
                GameObject NewNManagerGO = new GameObject();
                NewNManagerGO.AddComponent<NodeManager>();
                NewNManagerGO.name = "GunCrewManagers";
            }
            if (!PMExists)
                NodeManager.Instance.gameObject.AddComponent<PathFinderManager>();
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
                if (DropPointLocationSpawnCorner != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(DropPointLocationSpawnCorner.WorldPosition, DropPointLocationSpawnCorner.TileSize / 3);
                }

                if (CrewMemberSpawnLocations != null)
                {
                    for (int XIndex = 0; XIndex < CrewMemberSpawnLocations.Count; XIndex++)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireCube(CrewMemberSpawnLocations[XIndex].WorldPosition, CrewMemberSpawnLocations[XIndex].TileSize / 2);
                    }
                }

                if (PosibleDropSections != null)
                    for (int XIndex = 0; XIndex < PosibleDropSections.Count; XIndex++)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawWireCube(PosibleDropSections[XIndex].WorldPosition, PosibleDropSections[XIndex].TileSize / 2);
                    }
            }
        }
    }
}