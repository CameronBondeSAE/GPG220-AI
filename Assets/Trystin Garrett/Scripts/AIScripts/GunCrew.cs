using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunCrew : MonoBehaviour
    {
        public bool HasPreDefinedLocation = false;

        //Need to segragate Gun from GunCrew.... Wh
        public GameObject Gun;
        public GunCrewMember[] CrewMembers;

        public Node SpawnNode;
        public Transform[] GunSpotterPositions = new Transform[2];
        public Transform GunLoaderPosition;
        public int CrewSetupCounter = 0;

        public bool VisualDebugging = false;
        public GunCrewSpawnManager.GunSpawnStatus CurrentSpawnStatus = GunCrewSpawnManager.GunSpawnStatus.UnSpawned;

        private void Awake()
        {
            CheckForManagers();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }



        //
        public void RequestSpawnIn()
        {
            GunCrewSpawnManager.Instance.RequestSpawn(this);
            CurrentSpawnStatus = GunCrewSpawnManager.GunSpawnStatus.SpawnRequested;
        }
        public void CallAnimateGunSpawnIn()
        {
            if(CurrentSpawnStatus == GunCrewSpawnManager.GunSpawnStatus.GunDropPointFound && SpawnNode != null)
            {
                StartCoroutine(AnimateGunSpawnIn());
            }

        }
        IEnumerator AnimateGunSpawnIn()
        {
            CurrentSpawnStatus = GunCrewSpawnManager.GunSpawnStatus.GunDroppingIn;
            Gun.transform.position = SpawnNode.WorldPosition;

            //Placeholderspawn animation
            yield return new WaitForSeconds(3);
            Gun.gameObject.SetActive(true);

            CurrentSpawnStatus = GunCrewSpawnManager.GunSpawnStatus.GunLanded;
        }

        //
        void CheckForManagers()
        {
            bool NMExists = false;
            bool PMExists = false;
            bool GCSMExists = false;

            if(NodeManager.Instance != null)
                NMExists = true;
            if (PathFinderManager.Instance != null)
                PMExists = true;
            if (GunCrewSpawnManager.Instance != null)
                GCSMExists = true;

            if (!NMExists)
            {
                GameObject NewNManagerGO = new GameObject();
                NewNManagerGO.AddComponent<NodeManager>();
                NewNManagerGO.name = "GunCrewManagers";
            }
            if (!PMExists)
                NodeManager.Instance.gameObject.AddComponent<PathFinderManager>();
            if(!GCSMExists)
                NodeManager.Instance.gameObject.AddComponent<GunCrewSpawnManager>();
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
