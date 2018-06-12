using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tystin;
using Tystin.NodeUtility;

namespace Tystin
{
    namespace NodeUtility
    {

        public struct NodeUtility
        {

            //
            public Trystin.Node FindNodeFromWorldPosition(Vector3 _WorldPos, Trystin.NodeManager _NM)
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

                Trystin.Node ReturnNode = _NM.NodeGrid[XIntIndex, YIntIndex];
                if (ReturnNode != null)
                {
                    return ReturnNode;
                }
                else
                    return null;
            }

            //
            public Trystin.Node GetBetweenNodes()
            {

                return null;
            }

            //
            public int GetDistanceBetweenNode(Trystin.Node _NodeA, Trystin.Node _NodeB)
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
            public static Trystin.Node GetRandNode(Trystin.NodeManager _NM)
            {
                int Ranx = Random.Range(0, _NM.GridXLength);
                int RanY = Random.Range(0, _NM.GridYLength);
                Vector3 RandVec = new Vector3(Ranx, 0, RanY);
                Trystin.Node Node = _NM.FindNodeFromWorldPosition(RandVec);

                return Node;
            }
        }














    }
}
