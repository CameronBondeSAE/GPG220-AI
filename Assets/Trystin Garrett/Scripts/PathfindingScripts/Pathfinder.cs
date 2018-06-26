using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Tystin.NodeUtility;

namespace Trystin
{

    public class Pathfinder
    {
        public enum PathfinderStatus
        {
            Incative,
            Active
        }

        public NodeManager CurrentNM;

        public PathRequest CurrentPR;
        public Thread AvaliableThread;
        public PathFinderManager PFM;
        public PathfinderStatus CurrentStatus = PathfinderStatus.Incative;
        public int SkipDiagnalIndex = 0;

        private void Update()
        {

        }

        public void SubmitFindPath(PathRequest _PR, PathFinderManager _PFM, NodeManager _NM)
        {
            CurrentStatus = PathfinderStatus.Active;
            //Debug.Log("PF:  Request Recived by Pathfinder!");

            CurrentPR = _PR;
            PFM = _PFM;
            if (CurrentPR == null)
            {
                //Debug.Log("PF:  Incompleate Request, Null PR!");
                CurrentStatus = PathfinderStatus.Incative;
                return;
            }

            if (CurrentPR.PathIsFound == true)
            {
                //Debug.Log("PF:  Path Request has already been solved!!");
                _PFM.CompleatedRequests.Enqueue(CurrentPR);
                CurrentStatus = PathfinderStatus.Incative;
                return;
            }

            if (CurrentPR.StartingNode == null || CurrentPR.TargetNode == null || CurrentPR.Requestee == null)
            {
                //Debug.Log("PF:  Incompleate Request, Null Node/Requetee!");
                CurrentStatus = PathfinderStatus.Incative;
                return;
            }
            else if (CurrentPR.IsBeingProcessed == false && CurrentPR.CompletedPath == null)
            {
                CurrentNM = _NM;
                CurrentPR.IsBeingProcessed = true;
                CallFindPath();
            }
        }

        //
        void CallFindPath()
        {
            AvaliableThread = new Thread(FindPath);
            AvaliableThread.Start();
        }

        //
        void FindPath()
        {
            //Debug.Log("PF:  StartingPathFinding");
            Node StartNode = CurrentPR.StartingNode;
            Node TargetNode = CurrentPR.TargetNode;


            List<Node> OpenSet = new List<Node>();
            HashSet<Node> CloasedSet = new HashSet<Node>();
            OpenSet.Add(StartNode);

            while (OpenSet.Count > 0)
            {
                Node CurrentNode = OpenSet[0];
                for (int OSIndex = 1; OSIndex < OpenSet.Count; ++OSIndex)
                {
                    if (OpenSet[OSIndex].FCost < CurrentNode.FCost || OpenSet[OSIndex].FCost == CurrentNode.FCost && OpenSet[OSIndex].HCost < CurrentNode.HCost)
                    {
                        CurrentNode = OpenSet[OSIndex];
                    }
                }

                OpenSet.Remove(CurrentNode);
                CloasedSet.Add(CurrentNode);


                if (CurrentNode == TargetNode)
                {
                    List<Node> RetracedPath = RetraceNodePath(StartNode, TargetNode);
                    CurrentPR.CompletedPath = RetracedPath;
                    CurrentPR.PathIsFound = true;
                    CurrentPR.IsBeingProcessed = false;
                    EjectPath();
                    break;
                }

                List<Node> ToAddToOpenSet = new List<Node>();
                Node CornorNode = null;

                for (int NeighbourIndex = 0; NeighbourIndex < CurrentNode.NeighbouringTiles.Length; ++NeighbourIndex)
                {
                    Node NeighbourRef = CurrentNode.NeighbouringTiles[NeighbourIndex];

                    if (NeighbourRef.IsOccupied == true || CloasedSet.Contains(NeighbourRef))
                        continue;

                    int DistanceBetweenNodes = GetDistanceBetweenNode(CurrentNode, NeighbourRef);

                    //if (CurrentPR.SkipDiagnals)
                    //{
                    //    //if (DistanceBetweenNodes == 14 && NeighbourRef != TargetNode)
                    //    //    continue;
                    //}

                    int NewMovCostToNeighbour = CurrentNode.GCost + DistanceBetweenNodes;

                    if (NewMovCostToNeighbour < NeighbourRef.GCost || !OpenSet.Contains(NeighbourRef))
                    {
                        NeighbourRef.GCost = NewMovCostToNeighbour;
                        NeighbourRef.HCost = GetDistanceBetweenNode(NeighbourRef, TargetNode);
                        NeighbourRef.ParentNode = CurrentNode;

                        //
                        if (NeighbourRef.IsCorner)
                            CornorNode = NeighbourRef;

                        if (!OpenSet.Contains(NeighbourRef))
                        {
                            ToAddToOpenSet.Add(NeighbourRef);
                            //OpenSet.Add(NeighbourRef);
                        }
                    }
                }
                //
                if (CornorNode != null && !OpenSet.Contains(CornorNode))
                {
                    OpenSet.Add(CornorNode);
                    CornorNode = null;
                }
                else
                    for (int ToAddIndex = 0; ToAddIndex < ToAddToOpenSet.Count; ++ToAddIndex)
                        OpenSet.Add(ToAddToOpenSet[ToAddIndex]);

            }
            CurrentPR.IsBeingProcessed = false;
            EjectPath();
        }

        //
        public int GetDistanceBetweenNode(Node _NodeA, Node _NodeB)
        {
            int DistX = Mathf.Abs(_NodeA.GridPosition.X - _NodeB.GridPosition.X);
            int DistY = Mathf.Abs(_NodeA.GridPosition.Y - _NodeB.GridPosition.Y);

            if (DistX > DistY)
                return 14 * DistY + 10 * (DistX - DistY);
            else
                return 14 * DistX + 10 * (DistY - DistX);
        }

        //
        List<Node> RetraceNodePath(Node _StartNode, Node _EndNode)
        {
            List<Node> Path = new List<Node>();
            Node CurrentNode = _EndNode;

            while (CurrentNode != _StartNode)
            {
                Path.Add(CurrentNode);
                CurrentNode = CurrentNode.ParentNode;
            }

            Path.Reverse();
            return Path;
        }

        //
        void EjectPath()
        {
            if (CurrentPR.PathIsFound == false)
            {
                //Debug.Log("PF:  Failed To Find Path");
                PFM.CompleatedRequests.Enqueue(CurrentPR);
            }
            else if (CurrentPR.PathIsFound == true)
            {
                //Debug.Log("PF:  Path Found");
                PFM.CompleatedRequests.Enqueue(CurrentPR);
                ResetPathFinder();
            }
        }

        //
        void ResetPathFinder()
        {
            CurrentPR = null;
            CurrentStatus = PathfinderStatus.Incative;
            AvaliableThread.Abort();
            AvaliableThread = null;
            CurrentNM = null;
            PFM = null;
        }
    }
}