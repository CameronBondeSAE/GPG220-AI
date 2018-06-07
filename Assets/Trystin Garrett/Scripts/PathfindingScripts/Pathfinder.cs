using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour {

    public NodeManager NM;
    public Node StartNode;
    public Node EndNode;

    
    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            EndNode = GetRandNode();
            while(EndNode == null)
                EndNode = GetRandNode();

            StartNode = GetRandNode();
            while (StartNode == null)
                StartNode = GetRandNode();

            if(StartNode != null && EndNode != null)
            {
                FindPath(StartNode.WorldPosition, EndNode.WorldPosition);
            }

        }
    }

    Node GetRandNode()
    {
        int Ranx = Random.Range(0, NM.GridXLength);
        int RanY = Random.Range(0, NM.GridYLength);
        Vector3 RandVec = new Vector3(Ranx, 0, RanY);
        Node Node = NM.FindNodeFromWorldPosition(RandVec);

        return Node;
    }


    void FindPath(Vector3 _StartPos, Vector3 _TargetPos)
    {
        Debug.Log("StartingPathFinding");

        Node StartNode = NM.FindNodeFromWorldPosition(_StartPos);
        Node TargetNode = NM.FindNodeFromWorldPosition(_TargetPos);

        List<Node> OpenSet = new List<Node>();
        HashSet<Node> CloasedSet = new HashSet<Node>();
        OpenSet.Add(StartNode);

        while(OpenSet.Count > 0)
        {
            Node CurrentNode = OpenSet[0];
            for(int OSIndex = 1; OSIndex < OpenSet.Count; ++OSIndex)
            {
                if(OpenSet[OSIndex].FCost< CurrentNode.FCost || OpenSet[OSIndex].FCost == CurrentNode.FCost && OpenSet[OSIndex].HCost < CurrentNode.HCost)
                {
                    CurrentNode = OpenSet[OSIndex];
                }
            }
            OpenSet.Remove(CurrentNode);
            CloasedSet.Add(CurrentNode);

            if(CurrentNode == TargetNode)
            {
                Debug.Log("Found Path!!!");
                RetraceNodePath(StartNode, TargetNode);
                return;
            }

            for(int NeighbourIndex = 0; NeighbourIndex < CurrentNode.NeighbouringTiles.Length; ++NeighbourIndex)
            {
                Node NeighbourRef = CurrentNode.NeighbouringTiles[NeighbourIndex];

                if (NeighbourRef.IsOccupied == true || CloasedSet.Contains(NeighbourRef))
                {
                    continue;
                }

                int NewMovCostToNeighbour = CurrentNode.GCost + NM.GetDistanceBetweenNode(CurrentNode, NeighbourRef);

                if(NewMovCostToNeighbour < NeighbourRef.GCost || !OpenSet.Contains(NeighbourRef))
                {
                    NeighbourRef.GCost = NewMovCostToNeighbour;
                    NeighbourRef.HCost = NM.GetDistanceBetweenNode(NeighbourRef, TargetNode);
                    NeighbourRef.ParentNode = CurrentNode;

                    if(!OpenSet.Contains(NeighbourRef))
                    {
                        OpenSet.Add(NeighbourRef);
                    }


                }
            }
        }
        
    }

    void RetraceNodePath(Node _StartNode, Node _EndNode)
    {
        List<Node> Path = new List<Node>();
        Node CurrentNode = _EndNode;

        while(CurrentNode != _StartNode)
        {
            Path.Add(CurrentNode);
            CurrentNode = CurrentNode.ParentNode;
        }

        Path.Reverse();
        NM.PathFound = Path;
    }
}
