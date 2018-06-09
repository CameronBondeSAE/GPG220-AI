using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tystin.NodeUtility;

public class PathFinderManager : MonoBehaviour {

    public static PathFinderManager Instance;

    public NodeManager NM;
    public Pathfinder[] PathFinders = new Pathfinder[2];

    public Queue<PathRequest> PathRequests = new Queue<PathRequest>();
    public Queue<PathRequest> CompleatedRequests = new Queue<PathRequest>();

    public bool FrameToggle;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        NM = NodeManager.Instance;
        for(int PFIndex = 0; PFIndex < PathFinders.Length; ++PFIndex)
            PathFinders[PFIndex] = new Pathfinder();

    }

    private void Update()
    {
        if(FrameToggle == false)
        {
            DesignateRequestsToPathfinder();
            FrameToggle = true;
        }
        else if(FrameToggle == true)
        {
            DistributePaths();
            FrameToggle = false;
        }
    }

    //
    public void RequestPathFromNodes(Node _StartingNode, Node _TargetNode, TestMovementAI _Requestee)
    {
        PathRequest PR = new PathRequest(_StartingNode, _TargetNode, _Requestee);
        PathRequests.Enqueue(PR);
    }
    
    //
    public void RequestRandomPathFromVec3(Vector3 _CurrentPos, TestMovementAI _Requestee)
    {
        Debug.Log("PFM:   Random Request Made");
        Node StartingNode = NM.FindNodeFromWorldPosition(_CurrentPos);
        Node TargetNode = NM.GetRandNode();

        int RandAttempts = 0;

        if(TargetNode == null)
        {
            while(TargetNode == null)
            {
                ++RandAttempts;
                Debug.Log("PFM:   Retrying Random Node Find... Attempt: " + RandAttempts);
                TargetNode = NM.GetRandNode();

                if(RandAttempts > 20)
                    break;
            }

        }

        Debug.Log("PFM:   Found Random Node! Starting Node is: " + StartingNode.GridPostion.X + "/" + StartingNode.GridPostion.Y + " To " + TargetNode.GridPostion.X + "/" + TargetNode.GridPostion.Y);
        PathRequest PR = new PathRequest(StartingNode, TargetNode, _Requestee);
        PathRequests.Enqueue(PR);
    }

    //
    public void RequestPathFromVec3s(Vector3 _CurrentPos, Vector3 _TargetPos, TestMovementAI _Requestee)
    {
        Node StartingNode = NM.FindNodeFromWorldPosition(_CurrentPos);
        Node TargetNode = NM.FindNodeFromWorldPosition(_TargetPos);

        PathRequest PR = new PathRequest(StartingNode, TargetNode, _Requestee);
        PathRequests.Enqueue(PR);
    }

    //
    void DesignateRequestsToPathfinder()
    {
        if (PathRequests.Count == 0)
            return;

        for(int PFIndex = 0; PFIndex < PathFinders.Length; ++PFIndex)
        {
            if(PathFinders[PFIndex].CurrentStatus == Pathfinder.PathfinderStatus.Incative)
            {
                PathRequest PR = PathRequests.Dequeue();
                Debug.Log("PFM:   Request Designated from " + PR.Requestee.gameObject.name);
                PathFinders[PFIndex].SubmitFindPath(PR, this, NM);
                break;
            }
        }
    }

    //
    void DistributePaths()
    {
        if (CompleatedRequests.Count == 0)
            return;

        for (int CRIndex = 0; CRIndex < CompleatedRequests.Count; ++CRIndex)
        {
            PathRequest CPR = CompleatedRequests.Dequeue();
            CPR.Requestee.RecivePath(CPR.CompletedPath);
        }
    }
}

//
public class PathRequest
{
    public PathRequest(Node _StartingNode, Node _TargetNode, TestMovementAI _Requestee)
    {
        StartingNode = _StartingNode;
        TargetNode = _TargetNode;
        Requestee = _Requestee;
        PathIsFound = false;
        IsBeingProcessed = false;
        CompletedPath = null;
    }

    public bool PathIsFound;
    public bool IsBeingProcessed;
    public Node StartingNode;
    public Node TargetNode;
    public TestMovementAI Requestee;
    public List<Node> CompletedPath;
}
