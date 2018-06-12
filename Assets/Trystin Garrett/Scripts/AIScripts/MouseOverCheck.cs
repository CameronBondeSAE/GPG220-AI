using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{

    public class MouseOverCheck : MonoBehaviour
    {

        public Vector2 GridIndex;
        public bool IsCorner;
        public int NumberOfNeighbours;
        public int GCost;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Ray ray;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Node _Node = NodeManager.Instance.FindNodeFromWorldPosition(hit.point);
                if (_Node != null)
                {
                    GridIndex = new Vector2(_Node.GridPostion.X, _Node.GridPostion.Y);
                    IsCorner = _Node.IsCorner;
                    NumberOfNeighbours = _Node.NeighbouringTiles.Length;
                    GCost = _Node.GCost;

                }
            }



        }


    }
}
