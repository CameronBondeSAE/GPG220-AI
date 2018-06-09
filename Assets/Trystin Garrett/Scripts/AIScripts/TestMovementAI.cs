using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tystin.NodeUtility;

public class TestMovementAI : MonoBehaviour {

    public enum PathRequestStatus
    {
        NoneRequested,
        RequestedAndWaiting,
        Recived
    }

    public PathRequestStatus CurrentPathStatus = PathRequestStatus.NoneRequested;
    public List<Node> WayPoints;
    public bool TogglePathFound = false;

    //
    public void RequestRandomPath()
    {
        if (CurrentPathStatus != PathRequestStatus.RequestedAndWaiting)
        {
            CurrentPathStatus = PathRequestStatus.RequestedAndWaiting;
            WayPoints = null;
            PathFinderManager.Instance.RequestRandomPathFromVec3(transform.position, this);
        }
    }

    //
    public void RequestPath(Node _StartNode, Node _TargetNode)
    {
        if(CurrentPathStatus == PathRequestStatus.NoneRequested)
        {
            CurrentPathStatus = PathRequestStatus.RequestedAndWaiting;
            WayPoints = null;
            PathFinderManager.Instance.RequestPathFromNodes(_StartNode, _TargetNode, this);
        }

    }

    //
    public void RecivePath(List<Node> _NodePath)
    {
        WayPoints = _NodePath;
        CurrentPathStatus = PathRequestStatus.Recived;
        Debug.Log(gameObject.name + " Recived a Path! It is " + WayPoints.Count + " Nodes Long");
    }

    private void OnDrawGizmos()
    {
        if (TogglePathFound == false)
            return;
        if (WayPoints != null)
        {
            float WireSphereSize = WayPoints[0].TileSize.x / 4;
            for (int NodeIndex = 0; NodeIndex < WayPoints.Count; ++NodeIndex)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(WayPoints[NodeIndex].WorldPosition, WireSphereSize);
            }
        }
    }
}
