using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tystin;

namespace Trystin
{
    public class TestVisionAI : MonoBehaviour
    {

        public bool[,] KnownNodes;
        public int VisionRange = 4;

        // Use this for initialization
        void Start()
        {
            if (NodeManager.Instance != null)
            {
                KnownNodes = new bool[NodeManager.Instance.GridXLength, NodeManager.Instance.GridYLength];
            }
        }

        // Update is called once per frame
        void Update()
        {

        }


        //Maybe instead of calling as foreach loop... perhaps do a scan based on a function that retuns a array list of nodes based on the radius of vision... so 10...
        void InitialSightRangeScan()
        {
            Node InitialPos = NodeManager.Instance.FindNodeFromWorldPosition(transform.position);
            Vector2Int InitialGridPos = InitialPos.GridPostion;

            //Toggles Bulk of Vision Area
            for (int DepthXIndex = -(VisionRange - 1); DepthXIndex < VisionRange - 1; ++DepthXIndex)
            {
                for (int DepthYIndex = -(VisionRange - 1); DepthYIndex < VisionRange - 1; ++DepthYIndex)
                {
                    int GridXPlusOffset = InitialGridPos.X + DepthXIndex;
                    int GridYPlusOffset = InitialGridPos.Y + DepthYIndex;
                    KnownNodes[GridXPlusOffset, GridYPlusOffset] = true;
                }
            }

            //Vector2Int TopLeft =
    





        for (int DepthIndex = 0; DepthIndex < VisionRange; ++DepthIndex)
            {
                if (DepthIndex != VisionRange - 1)
                {

                }
                if (DepthIndex == VisionRange - 1)
                {

                }
            }
        }

    }


}

