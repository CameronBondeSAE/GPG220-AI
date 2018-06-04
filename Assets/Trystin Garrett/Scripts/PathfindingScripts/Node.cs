using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public Vector2Int GridPostion;
    public Node[] NeighbouringTiles;
    public BoxCollider ThisBC;
    public bool IsCollided;
    public Collider CurrentCollider;
    public Rigidbody ThisRb;

    public void OnTriggerEnter(Collider other)
    {
        IsCollided = true;
        CurrentCollider = other;
        ToggleTileActive(true);
    }

    //
    public void OnTriggerExit(Collider other)
    {
        IsCollided = false;
        CurrentCollider = null;
        ToggleNeighbouringTiles();
    }

    //
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(ThisBC.size.x, ThisBC.size.y, ThisBC.size.z));
    }

    //
    public void ToggleTileActive(bool _Toggle)
    {


        switch (_Toggle)
        {
            case true:
                gameObject.SetActive(true);
                ThisBC.enabled = true;

                if (IsCollided)
                    for (int NIndex = 0; NIndex < NeighbouringTiles.Length; ++NIndex)
                    {
                        if(NeighbouringTiles[NIndex].gameObject.activeSelf == false)
                            NeighbouringTiles[NIndex].ToggleTileActive(true);
                    }

                break;
            case false:
                ThisBC.enabled = false;
                gameObject.SetActive(false);
                break;
        }
    }

    //
    public void ToggleNeighbouringTiles()
    {
        for (int NIndex = 0; NIndex < NeighbouringTiles.Length; ++NIndex)
        {
            bool HasNeighbouringTile = false;
            for (int NNIndex = 0; NNIndex < NeighbouringTiles[NIndex].NeighbouringTiles.Length; ++NNIndex)
            {
                if(NeighbouringTiles[NIndex].NeighbouringTiles[NNIndex].IsCollided)
                    HasNeighbouringTile = true;
            }
            if(!HasNeighbouringTile)
            {
                NeighbouringTiles[NIndex].ToggleTileActive(false);
            }
        }
    }

    //
    public void RemoveSelfFromNeighbours()
    {

        for (int NIndex = 0; NIndex < NeighbouringTiles.Length; ++NIndex)
        {
            if (NeighbouringTiles[NIndex] == null)
                return;

            List<Node> NodeList = new List<Node>(NeighbouringTiles[NIndex].NeighbouringTiles);
            if(NodeList.Contains(this))
            {
                NodeList.Remove(this);
                Node[] NewNA = NodeList.ToArray();
                NeighbouringTiles[NIndex].NeighbouringTiles = NewNA;
            }
        }
    }
}
