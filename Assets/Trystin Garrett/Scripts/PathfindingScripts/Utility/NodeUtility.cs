using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tystin
{
    namespace NodeUtility
    {
        public struct NodeUtility
        {

            //
            public Node FindNodeFromWorldPosition(Vector3 _WorldPos, NodeManager _NM)
            {
                float XIndex = _WorldPos.x * _NM.GridXScale;
                float YIndex = _WorldPos.z * _NM.GridYScale;

                int XIntIndex = Mathf.RoundToInt(XIndex);
                int YIntIndex = Mathf.RoundToInt(YIndex);

                if (XIntIndex > _NM.GridXLength || YIntIndex > _NM.GridYLength || XIntIndex < 0 || YIntIndex < 0)
                {
                    Debug.Log("Node does not exist or player is out of bounds");
                    return null;
                }

                Node ReturnNode = _NM.NodeGrid[XIntIndex, YIntIndex];
                if (ReturnNode != null)
                {
                    return ReturnNode;
                }
                else
                    return null;
            }

            //
            public Node GetBetweenNodes()
            {

                return null;
            }

            //
            public int GetDistanceBetweenNode(Node _NodeA, Node _NodeB)
            {
                int DistX = Mathf.Abs(_NodeA.GridPostion.X - _NodeB.GridPostion.X);
                int DistY = Mathf.Abs(_NodeA.GridPostion.Y - _NodeB.GridPostion.Y);

                if (DistX > DistY)
                {
                    return 14 * DistY + 10 * (DistX - DistY);
                }
                else
                {
                    return 14 * DistX + 10 * (DistY - DistX);
                }
            }

            //
            public static Node GetRandNode(NodeManager _NM)
            {
                int Ranx = Random.Range(0, _NM.GridXLength);
                int RanY = Random.Range(0, _NM.GridYLength);
                Vector3 RandVec = new Vector3(Ranx, 0, RanY);
                Node Node = _NM.FindNodeFromWorldPosition(RandVec);

                return Node;
            }
        }














    }
}
