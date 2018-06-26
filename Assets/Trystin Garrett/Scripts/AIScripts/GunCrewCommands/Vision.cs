using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tystin;

namespace Trystin
{
    public class Vision : MonoBehaviour
    {
        [Header("Vision Variables")]
        public int VisionRange = 9;
        public List<Node> NodesInCurrentSight = new List<Node>();
        public bool[,] KnownNodes;
        public Vector2Int CurrentGridPos;

        [Space]
        [Header("Entity Variables")]
        public List<CharacterBase> NonCrewInVisionRange = new List<CharacterBase>();
        public List<GunCrewMember> CrewInVisionRange = new List<GunCrewMember>();

        [Space]
        [Header("Update Variables")]
        public float VisualUpdateTiming = 0;
        public float VisualUpdateTimingInterval = 0.5f;

        [Space]
        [Header("Visual Debugging")]
        public bool VisualKnowledgeDebugging = false;
        public bool VisualSightDebugging = false;

        // Use this for initialization
        void Start()
        {
            if (NodeManager.Instance != null)
                KnownNodes = new bool[NodeManager.Instance.GridXLength, NodeManager.Instance.GridYLength];
        }

        private void FixedUpdate()
        {
            if (!NodeManager.Instance.SetupCompletate)
                return;

            VisualUpdateTiming += Time.deltaTime;
            if (VisualUpdateTiming > VisualUpdateTimingInterval)
            {
                SightRangeScan();
                CheckForEntities();
                VisualUpdateTiming = 0;
            }
        }

        //
        public void SightRangeScan()
        {
            NodesInCurrentSight.Clear();
            Node InitialPos = NodeManager.Instance.FindNodeFromWorldPosition(transform.position);
            CurrentGridPos = InitialPos.GridPosition;

            //Toggles Bulk of Vision Area
            for (int DepthXIndex = -(VisionRange); DepthXIndex <= VisionRange; ++DepthXIndex)
            {
                for (int DepthYIndex = -(VisionRange); DepthYIndex <= VisionRange; ++DepthYIndex)
                {
                    if (Mathf.Abs(DepthXIndex) - Mathf.Abs(DepthYIndex) == 0 && (Mathf.Abs(DepthXIndex) == (VisionRange - 1) || Mathf.Abs(DepthYIndex) == (VisionRange - 1)))
                        continue;
                    if (Mathf.Abs(DepthXIndex) + Mathf.Abs(DepthYIndex) > (VisionRange + 1))
                        continue;

                    int GridXPlusOffset = CurrentGridPos.X + DepthXIndex;
                    int GridYPlusOffset = CurrentGridPos.Y + DepthYIndex;
                    GridXPlusOffset = Mathf.Clamp(GridXPlusOffset, 0, NodeManager.Instance.GridXLength -1);
                    GridYPlusOffset = Mathf.Clamp(GridYPlusOffset, 0, NodeManager.Instance.GridYLength -1 );

                    if (NodeManager.Instance.NodeGrid[GridXPlusOffset, GridYPlusOffset] == null)
                        continue;
                    else
                    {
                        KnownNodes[GridXPlusOffset, GridYPlusOffset] = true;
                        NodesInCurrentSight.Add(NodeManager.Instance.NodeGrid[GridXPlusOffset, GridYPlusOffset]);
                    }
                }
            }
        }

        //
        void CheckForEntities()
        {
            CrewInVisionRange.Clear();
            NonCrewInVisionRange.Clear();

            for (int NodeIndex = 0; NodeIndex < NodesInCurrentSight.Count; NodeIndex++)
                if(NodesInCurrentSight[NodeIndex].Occupant != null)
                {
                    CharacterBase OccRef = NodesInCurrentSight[NodeIndex].Occupant;

                    if (OccRef is GunCrewMember)
                        CrewInVisionRange.Add((GunCrewMember)OccRef);
                    else if (OccRef.gameObject.GetComponent<FieldGun>() != null)
                        continue;
                    else
                        NonCrewInVisionRange.Add(OccRef);
                }
        }

        //These Visual Debuggings are really really costly and inefficent!!
        private void OnDrawGizmos()
        {
            if (VisualKnowledgeDebugging)
            {
                if (!NodeManager.Instance.SetupCompletate)
                    return;

                for (int XIndex = 0; XIndex < NodeManager.Instance.GridXLength; ++XIndex)
                    for (int YIndex = 0; YIndex < NodeManager.Instance.GridYLength; ++YIndex)
                        if (KnownNodes[XIndex, YIndex] == true)
                        {
                            Gizmos.color = Color.magenta;
                            Gizmos.DrawWireCube(NodeManager.Instance.NodeGrid[XIndex, YIndex].WorldPosition, NodeManager.Instance.NodeGrid[XIndex, YIndex].TileSize / 2);
                        }

            }
            if (VisualSightDebugging)
            {
                if (!NodeManager.Instance.SetupCompletate)
                    return;

                for (int SightIndex = 0; SightIndex < NodesInCurrentSight.Count; ++SightIndex)
                {
                    if (NodesInCurrentSight[SightIndex] != null)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawWireCube(NodesInCurrentSight[SightIndex].WorldPosition, NodesInCurrentSight[SightIndex].TileSize / 4);
                    }
                }
            }
        }
    }
 }

