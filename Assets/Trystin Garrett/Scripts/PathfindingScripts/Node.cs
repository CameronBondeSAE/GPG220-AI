using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Trystin
{

    public class Node
    {

        public enum ColliderOwnerType
        {
            Null,
            AI,
            Object
        }

        public Vector2Int GridPosition;
        public Vector3 TileSize;
        public Vector3 WorldPosition;
        public Node[] NeighbouringTiles;
        public CharacterBase Occupant;

        public bool IsOccupied;
        public bool IsInProximity;
        public bool IsCorner;

        public int GCost;
        public int HCost;
        public int FCost { get { return (GCost + HCost); } }
        public Node ParentNode;

        //public Node(Vector2Int _GridPostion, Vector3 _TileSize, Vector3 _WorldPosition)
        //{

        //}

        //
        public ColliderOwnerType ColliderOverlapCheck(Node _Node, Collider _FloorCol)
        {
            ColliderOwnerType ColliderType = ColliderOwnerType.Null;

            Collider[] NC = Physics.OverlapBox(_Node.WorldPosition, _Node.TileSize / 2);

            if (NC == null)
                return ColliderOwnerType.Null;

            if (NC.Length == 1)
            {
                if(NC[0] == _FloorCol)
                    return ColliderOwnerType.Null;

                Occupant = NC[0].gameObject.GetComponent<CharacterBase>();
                if (Occupant == null)
                    Occupant = NC[0].gameObject.GetComponentInParent<CharacterBase>();
                if (Occupant != null)
                {
                    IsOccupied = true;
                    return ColliderOwnerType.AI;
                }
            }

            if (NC.Length > 1)
            {
                for (int index = 0; index < NC.Length; ++index)
                {
                    if (NC[index] == _FloorCol)
                        continue;
                    bool Check = CheckColliderDistance(NC[index], _Node);
                    if (Check)
                        ColliderType = ColliderOwnerType.Object;
                }
            }
            else if (NC.Length == 1)
            {
                if (NC[0] == _FloorCol)
                    return ColliderOwnerType.Null;
                bool Check = CheckColliderDistance(NC[0], _Node);
                if (Check)
                    ColliderType = ColliderOwnerType.Object;
            }

            return ColliderType;
        }

        //
        bool CheckColliderDistance(Collider _Col, Node _Node)
        {
            bool NoGo = false;
            Vector3 ContactPoints = _Col.ClosestPoint(_Node.WorldPosition);

            float XBounds = ContactPoints.x - _Node.WorldPosition.x;
            float YBounds = ContactPoints.z - _Node.WorldPosition.z;

            XBounds = Mathf.Abs(XBounds);
            YBounds = Mathf.Abs(YBounds);

            if (XBounds < 0.275f && XBounds > 0f && YBounds == 0f)
                NoGo = true;
            if (YBounds < 0.275f && YBounds > 0f && XBounds == 0f)
                NoGo = true;
            if (YBounds < 0.275f && XBounds < 0.275f)
                NoGo = true;
            return NoGo;
        }
    }
}