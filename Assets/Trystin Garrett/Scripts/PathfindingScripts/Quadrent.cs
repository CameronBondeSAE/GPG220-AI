using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Trystin
{
    public class Quadrent
    {
        public Direction QuadrentPostion;
        public Vector2Int GridPosition;
        public Vector2 QuadrentStart;
        public Vector2 QuadrentEnd;
        public Vector3 WorldPosition;
        public Vector3 QuadrentSize;
        public Quadrent[] NeighbouringTiles;


        //public Quadrent(Vector2Int _GridPosition, Vector3 _WorldPosition, Vector2 _QuadrentStart, Vector2 _QuadrentEnd)
        //{
        //    GridPosition = _GridPosition;
        //    WorldPosition = _WorldPosition;
        //    QuadrentStart = _QuadrentStart;
        //    QuadrentEnd = _QuadrentEnd;
        //    QuadrentSize = new Vector3(_QuadrentEnd.x - _QuadrentStart.x, 0.3f, _QuadrentEnd.y - _QuadrentStart.y); 
        //}
    }
}
