using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunCrew : MonoBehaviour
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

        public GunCrewMember[] CrewMembers;
        public Vector3 GunCrewArea = new Vector3(3,3,3);

        public List<Node> CurrentScanNodeList;
        public Vector2 CurrentNodePos;
        //public GameObject GunCrewPrefab;

        public int GunCrewAreaX;
        public int GunCrewAreaY;
        public int DepthOfScan = 5;

        public int CheckedNodeLength = 0;
        public bool VisualDebugging = false;

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
            if(NodeManager.Instance.SetupCompletate == true)
            {
                Node Location = NodeManager.Instance.FindNodeFromWorldPosition(transform.position);
                if (Location != null)
                    CurrentNodePos = new Vector2(Location.GridPostion.X, Location.GridPostion.Y);
            }
        }

        void ScanForRandomDropZone()
        {

        }
        public void TestScanForDropZone()
        {
            ScanForMapEdgesDropZones(GunCrewAreaX, GunCrewAreaY, DepthOfScan);
        }

        void ScanForMapEdgesDropZones(int _GunCrewAreaX, int _GunCrewAreaY, int _DepthOfScanBox)
        {
            int RequiredXLengh = _GunCrewAreaX + 2;
            int RequiredYLengh = _GunCrewAreaY + 2;
            Vector2Int NV2I = new Vector2Int((int)CurrentNodePos.x, (int)CurrentNodePos.y);

            CurrentScanNodeList = ReturnScanAreaNodes(NV2I, _DepthOfScanBox, RequiredXLengh, RequiredYLengh);
            CheckedNodeLength = CurrentScanNodeList.Count;
        }

        List<Node> ReturnScanAreaNodes(Vector2Int _AreaScanOriginPoint, int _DepthOfScanBox, int _GunCrewAreaX, int _GunCrewAreaY)
        {
            List<Node> ListOfNodesToScan = new List<Node>();
            bool StartingXIterationValue = true;
            bool StartingYIterationValue = true;

            if (_AreaScanOriginPoint.X > (NodeManager.Instance.GridXLength/2 + _GunCrewAreaX))
            {
                StartingXIterationValue = false;
                //if (_AreaScanOriginPoint.Y > (NodeManager.Instance.GridYLength / 2 + _GunCrewAreaY))
                //    StartingYIterationValue = false;
            }

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


        bool ScanNodeLocation(int _GunCrewAreaX, int _GunCrewAreaY, Node _Node)
        {
            for (int XIndex = 0; XIndex < _GunCrewAreaX; XIndex++)
            {
                for (int YIndex = 0; YIndex < _GunCrewAreaY; YIndex++)
                {
                    if (NodeManager.Instance.NodeGrid[_Node.GridPostion.X + XIndex , _Node.GridPostion.Y + YIndex] == null)
                        return false;

                    if (NodeManager.Instance.NodeGrid[_Node.GridPostion.X + XIndex, _Node.GridPostion.Y + YIndex].IsOccupied == true)
                        return false;

                }
            }
            return true;
        }


        void DropInGun()
        {

        }

        void DropInGunCrew()
        {

        }


        //
        void CheckForManagers()
        {
            bool NMExists = false;
            bool PMExists = false;

            if(NodeManager.Instance != null)
                NMExists = true;
            if (PathFinderManager.Instance != null)
                PMExists = true;

            if(!NMExists)
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
                if (!NodeManager.Instance.SetupCompletate || CurrentScanNodeList == null)
                    return;

                if (CurrentScanNodeList.Count == 0)
                    return;

                for (int XIndex = 0; XIndex < CurrentScanNodeList.Count; ++XIndex)
                {

                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(CurrentScanNodeList[XIndex].WorldPosition, CurrentScanNodeList[XIndex].TileSize / 2);
                }

                        

            }

        }

    }
}
