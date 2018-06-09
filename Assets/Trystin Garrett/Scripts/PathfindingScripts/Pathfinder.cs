﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Tystin.NodeUtility;

public class Pathfinder {

    public enum PathfinderStatus
    {
        Incative,
        Active
    }

    public NodeManager CurrentNM;

    public PathRequest CurrentPR;
    public Thread AvaliableThread;
    public PathfinderStatus CurrentStatus = PathfinderStatus.Incative;

    private void Update()
    {

    }

    public void SubmitFindPath(PathRequest _PR, PathFinderManager _PFM, NodeManager _NM)
    {
        CurrentStatus = PathfinderStatus.Active;
        Debug.Log("PF:  Request Recived by Pathfinder!");

        CurrentPR = _PR;
        if (CurrentPR == null)
        {
            Debug.Log("PF:  Incompleate Request, Null PR!");
            CurrentStatus = PathfinderStatus.Incative;
            return;
        }

        if (CurrentPR.PathIsFound == true)
        {
            Debug.Log("PF:  Path Request has already been solved!!");
            _PFM.CompleatedRequests.Enqueue(CurrentPR);
            CurrentStatus = PathfinderStatus.Incative;
            return;
        }

        if(CurrentPR.StartingNode == null || CurrentPR.TargetNode == null || CurrentPR.Requestee == null)
        {
            Debug.Log("PF:  Incompleate Request, Null Node/Requetee!");
            CurrentStatus = PathfinderStatus.Incative;
            return;
        }
        else if (CurrentPR.IsBeingProcessed == false && CurrentPR.CompletedPath == null)
        {
            CurrentNM = _NM;
            CurrentPR.IsBeingProcessed = true;
            AvaliableThread = new Thread(FindPath);
            AvaliableThread.Start();

            //////THIS BELLOW NEEDS TO BE PUT INTO PATHFINDER THEN CALLED BY THE THREAD RATHER THAN THE CALL SCRIPTS, IT IS WAITING FOR THE PATH IN THE PFM

            while (CurrentPR.IsBeingProcessed == true)
            {
                Debug.Log("PF:  WorkingOnPath!!!");
            }
            if(CurrentPR.IsBeingProcessed == false)
            {
                if (CurrentPR.PathIsFound == false)
                {
                    Debug.Log("PF:  Failed To Find Path");
                    _PFM.CompleatedRequests.Enqueue(CurrentPR);
                }
                else if (CurrentPR.PathIsFound == true)
                {
                    Debug.Log("PF:  Path Found");
                    _PFM.CompleatedRequests.Enqueue(CurrentPR);
                    ResetPathFinder();
                }
            } 
        }
    }

    //
    void FindPath()
    {
        Debug.Log("PF:  StartingPathFinding");

        //Node StartNode = NM.FindNodeFromWorldPosition(CurrentRP.StartingNode.WorldPosition);
        //Node TargetNode = NM.FindNodeFromWorldPosition(CurrentRP.TargetNode.WorldPosition);

        //Node StartNode = CurrentNM.FindNodeFromWorldPosition(CurrentRP.StartingNode.WorldPosition);
        //Node TargetNode = CurrentNM.FindNodeFromWorldPosition(CurrentRP.TargetNode.WorldPosition);

        Node StartNode = CurrentPR.StartingNode;
        Node TargetNode = CurrentPR.TargetNode;


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
                List<Node> RetracedPath = RetraceNodePath(StartNode, TargetNode);
                CurrentPR.CompletedPath = RetracedPath;
                CurrentPR.PathIsFound = true;
                CurrentPR.IsBeingProcessed = false;
                break;
            }

            for(int NeighbourIndex = 0; NeighbourIndex < CurrentNode.NeighbouringTiles.Length; ++NeighbourIndex)
            {
                Node NeighbourRef = CurrentNode.NeighbouringTiles[NeighbourIndex];

                if (NeighbourRef.IsOccupied == true || CloasedSet.Contains(NeighbourRef))
                {
                    continue;
                }

                int NewMovCostToNeighbour = CurrentNode.GCost + CurrentNM.GetDistanceBetweenNode(CurrentNode, NeighbourRef);

                if(NewMovCostToNeighbour < NeighbourRef.GCost || !OpenSet.Contains(NeighbourRef))
                {
                    NeighbourRef.GCost = NewMovCostToNeighbour;
                    NeighbourRef.HCost = CurrentNM.GetDistanceBetweenNode(NeighbourRef, TargetNode);
                    NeighbourRef.ParentNode = CurrentNode;

                    if(!OpenSet.Contains(NeighbourRef))
                    {
                        OpenSet.Add(NeighbourRef);
                    }
                }
            }
        }
        CurrentPR.IsBeingProcessed = false; 
    }

    //
    List<Node> RetraceNodePath(Node _StartNode, Node _EndNode)
    {
        List<Node> Path = new List<Node>();
        Node CurrentNode = _EndNode;

        while(CurrentNode != _StartNode)
        {
            Path.Add(CurrentNode);
            CurrentNode = CurrentNode.ParentNode;
        }

        Path.Reverse();
        return Path;
    }

    void ResetPathFinder()
    {
        CurrentPR = null;
        CurrentStatus = PathfinderStatus.Incative;
        AvaliableThread.Abort();
        AvaliableThread = null;
        CurrentNM = null;
    }
}
