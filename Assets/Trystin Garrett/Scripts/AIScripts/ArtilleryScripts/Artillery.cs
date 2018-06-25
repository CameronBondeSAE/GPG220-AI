using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class Artillery : MonoBehaviour
    {
        [Header("Components")]
        public FieldGun FieldGun;
        public GunCrewMember[] CrewMembers;
        public Node SpawnNode;

        [Space]
        [Header("Setup Variables")]
        public ArtillerySpawnStatus CurrentSpawnStatus = ArtillerySpawnStatus.UnSpawned;
        public int CrewSetupCounter = 0;
        public bool HasPreDefinedLocation = false;

        [Space]
        [Header("Operational Variables")]
        public ArtilleryOrderStatus CurrentArtOrderStatus = ArtilleryOrderStatus.InActive;

        [Space]
        [Header("Debugging")]
        public bool VisualDebugging = false;

        private void Awake()
        {
            CheckForManagers();
        }

        // Use this for initialization
        void Start()
        {
            RequestSpawnIn();
        }

        // Update is called once per frame
        void Update()
        {
            DetermineOrders();
        }

        //
        public void RequestSpawnIn()
        {
            ArtillerySpawnManager.Instance.RequestSpawn(this);
            CurrentSpawnStatus = ArtillerySpawnStatus.SpawnRequested;
        }


        void DetermineOrders()
        {
            switch (CurrentArtOrderStatus)
            {
                case ArtilleryOrderStatus.InActive:
                    if (CurrentSpawnStatus == ArtillerySpawnStatus.CrewLanded)
                        CurrentArtOrderStatus = ArtilleryOrderStatus.InitialOrders;
                    break;
                case ArtilleryOrderStatus.InitialOrders:
                    GiveInitialCommands();
                    CurrentArtOrderStatus = ArtilleryOrderStatus.Monitoring;
                    break;
                case ArtilleryOrderStatus.Monitoring:
                    break;
                case ArtilleryOrderStatus.Exploration:
                    break;
                case ArtilleryOrderStatus.FindingTarget:
                    break;
                case ArtilleryOrderStatus.ReOrganisingCrew:
                    break;
                default:
                    break;
            }
        }


        //
        public void GiveInitialCommands()
        {
            if (CrewMembers[0] != null)
            {
                CommandManager.Instance.IssueLoadGunCommand(CrewMembers[0], FieldGun);
            }
            if (CrewMembers[1] != null)
            {
                CommandManager.Instance.IssueSpotForGunCommand(CrewMembers[1], FieldGun);
            }
        }

        //
        void CheckForManagers()
        {
            bool NMExists = false;
            bool PMExists = false;
            bool GCSMExists = false;
            bool CMExists = false;

            if(NodeManager.Instance != null)
                NMExists = true;
            if (PathFinderManager.Instance != null)
                PMExists = true;
            if (ArtillerySpawnManager.Instance != null)
                GCSMExists = true;
            if (CommandManager.Instance != null)
                CMExists = true;

            if (!NMExists)
            {
                GameObject NewNManagerGO = new GameObject();
                NewNManagerGO.AddComponent<NodeManager>();
                NewNManagerGO.name = "GunCrewManagers";
            }
            if (!PMExists)
                NodeManager.Instance.gameObject.AddComponent<PathFinderManager>();
            if(!GCSMExists)
                NodeManager.Instance.gameObject.AddComponent<ArtillerySpawnManager>();
            if (!CMExists)
                NodeManager.Instance.gameObject.AddComponent<CommandManager>();
        }


        //These Visual Debuggings are really really costly and inefficent!!
        private void OnDrawGizmos()
        {
            if (VisualDebugging)
            {
            }
        }

    }
}
