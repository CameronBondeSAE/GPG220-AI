using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour {

    public enum PingState
    {
        InActive,
        Sweeping,
        Returning,
        Culling
    }

    public int GridXLength = 100;
    public int GridYLength = 100;
    public float GridXScale;
    public float GridYScale;

    public Node[,] NodeGrid;
    private int DelelteTempXLength;
    private int DeleteTempYLength;

    public int PingInterval = 100;
    public int RemovalInterval = 10;

    public bool DoAIHaveRB = true;

    public Coroutine CurrentPing;
    public PingState CurrentPingState = PingState.InActive;

    public List<Node> TerrainOccupiedNodes = new List<Node>();

    public void Start()
    {
        CreateGrid();
        CallPing();
    }

    //SHOULD SEPERATE THIS INTO A COROUTINE TO SPACEIT OUT
    public void CreateGrid()
    {
        if (NodeGrid != null)
            DeleteMap();

        Node[,] NewTileGrid = new Node[GridXLength, GridYLength];
        for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
        {
            for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
            {
                GameObject NewNode = new GameObject();

                Node NewNodeScript = NewNode.AddComponent<Node>();
                NewNodeScript.GridPostion = new Vector2Int(XIndex, YIndex);
                BoxCollider NewBC = NewNode.AddComponent<BoxCollider>();
                NewBC.size = new Vector3(GridXScale - 0.01f, 0.2f, GridYScale - 0.01f);
                NewBC.isTrigger = true;
                NewNodeScript.ThisBC = NewBC;
                Rigidbody NewRB =  NewNodeScript.ThisRb = NewNode.AddComponent<Rigidbody>();
                NewRB.useGravity = false;

                NewNode.transform.position = new Vector3((NewNodeScript.GridPostion.X * GridXScale), 0f, (NewNodeScript.GridPostion.Y * GridYScale));


                NewTileGrid[XIndex, YIndex] = NewNodeScript;
                NewNode.name = NewNodeScript.GridPostion.X + " , " + NewNodeScript.GridPostion.Y;
                NewNodeScript.GridPostion = new Vector2Int(XIndex, YIndex);

                NewNode.transform.SetParent(this.transform);
                NewNodeScript.ToggleTileActive(false);
            }
        }
        NodeGrid = NewTileGrid;
        SetTileNeighbours();
        DelelteTempXLength = GridXLength;
        DeleteTempYLength = GridYLength;
    }

    //
    public void SetTileNeighbours()
    {
        if (NodeGrid == null)
            return;

        for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
            for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
                NodeGrid[XIndex, YIndex].NeighbouringTiles = GetNeighbouringNodeTiles(NodeGrid[XIndex, YIndex]);
    }

    //
    public Node[] GetNeighbouringNodeTiles(Node _NodeTile)
    {
        List<Node> Neighbours = new List<Node>();

        if (_NodeTile == null)
            return Neighbours.ToArray(); ;

        for (int XIndex = -1; XIndex <= 1; XIndex++)
        {
            for (int ZIndex = -1; ZIndex <= 1; ZIndex++)
            {
                if (XIndex == 0 && ZIndex == 0) 
                {
                    continue;
                }
                int CheckX = _NodeTile.GridPostion.X + XIndex;
                int CheckZ = _NodeTile.GridPostion.Y + ZIndex;

                if (CheckX >= 0 && CheckX < GridXLength && CheckZ >= 0 && CheckZ < GridYLength)
                    Neighbours.Add(NodeGrid[CheckX, CheckZ]);
            }
        }
        return Neighbours.ToArray();
    }

    //
    public void CallPing()
    {
        if (NodeGrid == null)
            return;

        CurrentPing = StartCoroutine(PingGrid());
    }
    IEnumerator PingGrid()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("PingStarted");

        CallInitialSweep();

        if(CurrentPingState != PingState.InActive)
        {
            yield return new WaitUntil(() => CurrentPingState == PingState.InActive);
            CallSortTerrainFromAI();
        }
        if (CurrentPingState != PingState.InActive)
        {
            yield return new WaitUntil(() => CurrentPingState == PingState.InActive);
            CallCleanUpObsticleTiles();
        }

        if (CurrentPingState != PingState.InActive)
        {
            yield return new WaitUntil(() => CurrentPingState == PingState.InActive);
            Debug.Log("PingEnded");
        }
    }


    //
    void CallInitialSweep()
    {
        CurrentPingState = PingState.Sweeping;
        StartCoroutine(InitialSweep());
    }
    IEnumerator InitialSweep()
    {
        Debug.Log("Initial Sweep Started");
        int CurrentNodeCount = 0;

        for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
        {
            for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
            {
                ++CurrentNodeCount;

                if (NodeGrid[XIndex, YIndex] != null)
                    NodeGrid[XIndex, YIndex].ToggleTileActive(true);

                if (CurrentNodeCount % PingInterval == 0)
                    yield return null;
            }
        }
        CurrentPingState = PingState.InActive;
        Debug.Log("Initial Sweep Completed!");
    }


    //Tag compare vs getcomponent
    void CallSortTerrainFromAI()
    {
        CurrentPingState = PingState.Returning;
        StartCoroutine(SortTerrainFromAI());
    }
    IEnumerator SortTerrainFromAI()
    {
        Debug.Log("Sorting Terrain from AI");
        int CurrentNodeCount = 0;

        for (int XIndex = 0; XIndex < GridXLength; ++XIndex)
        {
            for (int YIndex = 0; YIndex < GridYLength; ++YIndex)
            {
                ++CurrentNodeCount;

                if (NodeGrid[XIndex, YIndex].IsCollided == true && NodeGrid[XIndex, YIndex].CurrentCollider != null)
                {

                    //NEED TO IMPLEMENT IF THERE IS NO TERRAIN TO WALK ON CHECK
                    switch (NodeGrid[XIndex, YIndex].CurrentCollider.gameObject.tag)
                    {
                        case "Object":
                            TerrainOccupiedNodes.Add(NodeGrid[XIndex, YIndex]);
                            Destroy(NodeGrid[XIndex, YIndex].ThisRb);
                            break;
                        case "AI":
                            if (DoAIHaveRB)
                                Destroy(NodeGrid[XIndex, YIndex].ThisRb);
                            break;
                    }
                }
                else
                {
                    if (DoAIHaveRB)
                        Destroy(NodeGrid[XIndex, YIndex].ThisRb);
                    NodeGrid[XIndex, YIndex].ToggleTileActive(false);
                }

                if (CurrentNodeCount % PingInterval == 0)
                    yield return null;
            }
        }
        CurrentPingState = PingState.InActive;
        Debug.Log("Sorting Terrain from AI Complete!");
    }


    //
    void CallCleanUpObsticleTiles()
    {
        CurrentPingState = PingState.Culling;
        StartCoroutine(CleanUpObsticlesTiles());
    }
    IEnumerator CleanUpObsticlesTiles()
    {
        //Removing tiles that have obsticles, Themselves from their neighbours also (Maybe I should reallocate the Neighbouring nodes arrays to free up space and avoid future null refrence checks)
        Debug.Log("Cleaning Up obsticle Tiles");
        int CurrentTerrainRemovalCount = 0;
        Vector2Int TempV2I = new Vector2Int();

        for (int ObTiles = 0; ObTiles < TerrainOccupiedNodes.Count; ++ObTiles)
        {
            TempV2I = TerrainOccupiedNodes[ObTiles].GridPostion;
            TerrainOccupiedNodes[ObTiles].RemoveSelfFromNeighbours();
            Destroy(NodeGrid[TempV2I.X, TempV2I.Y].gameObject);
            NodeGrid[TempV2I.X, TempV2I.Y] = null;
            ++CurrentTerrainRemovalCount;

            if (CurrentTerrainRemovalCount % RemovalInterval == 0)
                yield return null;
        }
        TerrainOccupiedNodes = null;
        CurrentPingState = PingState.InActive;
        Debug.Log("Cleaning Up obsticle Tiles Compleate!");
    }

















    public void DeleteMap()
    {
        if (NodeGrid == null)
            return;

        for (int XIndex = 0; XIndex < DelelteTempXLength; ++XIndex)
        {
            for (int YIndex = 0; YIndex < DeleteTempYLength; ++YIndex)
            {
                if (NodeGrid[XIndex, YIndex] != null)
                {
                    if (Application.isEditor)
                    {
                        DestroyImmediate(NodeGrid[XIndex, YIndex].gameObject);
                        NodeGrid[XIndex, YIndex] = null;
                    }
                    else
                    {
                        Destroy(NodeGrid[XIndex, YIndex].gameObject);
                        NodeGrid[XIndex, YIndex] = null;
                    }
                }
            }
        }
        NodeGrid = null;
    }
}
