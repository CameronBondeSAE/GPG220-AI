using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Trystin
{
    public class FieldGun : MonoBehaviour
    {
        [Header("Components")]
        public GameObject Mesh;
        public Transform[] GunSpotterPositions = new Transform[2];
        public Transform GunLoaderPosition;
        public Node BreachNode;
        public Node[] GunSpotterNodes = new Node[2];

        [Space]
        [Header("Operational Variables")]
        public Artillery OwnerGC;
        public GunCrewMember Loader;
        public GunCrewMember Spotter;

        [Space]
        [Header("Tracking Variables")]
        public FieldGunState CurrentState = FieldGunState.Idle;
        public float TrackingInterval = 0.5f;
        public float CurrentTime = 0;

        [Space]
        [Header("Spawn Variables")]
        public bool HasSpawnCompleated = false;

        [Space]
        [Header("Debugging")]
        public bool VisualDebugging = false;

        //EVEnt setup drivent
        public event Action GunPositionsHaveChanged;

        // Use this for initialization
        void Start()
        {
            ReturnPositionNodes(); 
        }

        // Update is called once per frame
        void Update()
        {
            if (NodeManager.Instance.CurrentGridState != NodeManager.ProgressState.Complete)
                return;

            TrackPositionNodes();
        }

        //
        void TrackPositionNodes()
        {
            if (CurrentState != FieldGunState.Rotating)
                return;

            if (CurrentTime < TrackingInterval)
            {
                CurrentTime += Time.deltaTime;
                return;
            }
            else
                CurrentTime = 0;

            ReturnPositionNodes();
        }

        //
        void ReturnPositionNodes()
        {
            Node[] PotentalSpotterNodes = new Node[GunSpotterPositions.Length];
            Node PotentalBreachNode;

            for (int SpotterNodeIndex = 0; SpotterNodeIndex < GunSpotterPositions.Length; ++SpotterNodeIndex)
            {
                PotentalSpotterNodes[SpotterNodeIndex] = NodeManager.Instance.FindNodeFromWorldPosition(GunSpotterPositions[SpotterNodeIndex].position);
                if (PotentalSpotterNodes[SpotterNodeIndex] == null || PotentalSpotterNodes[SpotterNodeIndex].IsOccupied == true)
                    PotentalSpotterNodes[SpotterNodeIndex] = null;
            }

            PotentalBreachNode = NodeManager.Instance.FindNodeFromWorldPosition(GunLoaderPosition.position);
            if (PotentalBreachNode == null || PotentalBreachNode.IsOccupied == true)
                PotentalBreachNode = null;

            bool IsThereAChangeOfNodes = false;
            for(int SpotterNodeIndex = 0; SpotterNodeIndex < PotentalSpotterNodes.Length; ++SpotterNodeIndex)
            {
                if (PotentalSpotterNodes[SpotterNodeIndex] != null)
                {
                    if (PotentalSpotterNodes[SpotterNodeIndex] != GunSpotterNodes[SpotterNodeIndex])
                    {
                        IsThereAChangeOfNodes = true;
                        GunSpotterNodes[SpotterNodeIndex] = PotentalSpotterNodes[SpotterNodeIndex];
                    }
                }
                if(PotentalBreachNode != null)
                {
                    if (PotentalBreachNode != BreachNode)
                    {
                        IsThereAChangeOfNodes = true;
                        BreachNode = PotentalBreachNode;
                    }
                }
            }
            if(IsThereAChangeOfNodes)
            {
                if (GunPositionsHaveChanged != null)
                    GunPositionsHaveChanged();
            }
        }

        #region Setup Methods

        //
        public void CallAnimateGunSpawnIn(Artillery _OwnerGC, Node _SpawnLocation)
        {
            OwnerGC = _OwnerGC;
            if (_SpawnLocation != null && OwnerGC != null)
            {
                gameObject.SetActive(true);
                StartCoroutine(AnimateGunSpawnIn(_SpawnLocation));
            }

        }
        IEnumerator AnimateGunSpawnIn(Node _SpawnLocation)
        {
            OwnerGC.CurrentSpawnStatus = ArtillerySpawnStatus.GunDroppingIn;
            transform.position = _SpawnLocation.WorldPosition;

            //Placeholderspawn animation
            yield return new WaitForSeconds(3);
            Mesh.SetActive(true);

            HasSpawnCompleated = true;
            OwnerGC.CurrentSpawnStatus = ArtillerySpawnStatus.GunLanded;
        }

        #endregion

        //These Visual Debuggings are really really costly and inefficent!!
        private void OnDrawGizmos()
        {
            if (VisualDebugging)
            {
                if(BreachNode != null)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(BreachNode.WorldPosition, 0.25f);
                }

                for(int SpotterNodeIndex = 0; SpotterNodeIndex < GunSpotterNodes.Length; ++SpotterNodeIndex)
                {
                    if(GunSpotterNodes[SpotterNodeIndex] != null)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireSphere(GunSpotterNodes[SpotterNodeIndex].WorldPosition, 0.25f);
                    }
                }
            }
        }
    }
}