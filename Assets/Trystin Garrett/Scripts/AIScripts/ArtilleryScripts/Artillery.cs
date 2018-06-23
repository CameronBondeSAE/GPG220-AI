﻿using System.Collections;
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
        public ArtillerySpawnManager.ArtillerySpawnStatus CurrentSpawnStatus = ArtillerySpawnManager.ArtillerySpawnStatus.UnSpawned;
        public int CrewSetupCounter = 0;
        public bool HasPreDefinedLocation = false;

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

        }

        // Update is called once per frame
        void Update()
        {

        }

        //
        public void RequestSpawnIn()
        {
            ArtillerySpawnManager.Instance.RequestSpawn(this);
            CurrentSpawnStatus = ArtillerySpawnManager.ArtillerySpawnStatus.SpawnRequested;
        }

        public IEnumerator ActivateCrew()
        {

            yield return new WaitForSeconds(0.5f);
            CrewMembers[0].CallChangeCrewRole(GunCrewRole.GunOperator, GunnerSubRole.Loader);
            CrewMembers[0].CurrentStatus = AIStatus.Active;
        }


        //
        void ReassignAllCrewRoles()
        {
            //This will need to be called Also on a charaterbase death and check if the characterbase is a crew emmber


        }




        //
        public void AssignInitialCrewRoles()
        {
            //for (int CrewIndex = 0; CrewIndex < CrewMembers.Length; CrewIndex++)
            //{ 
            //    if(CrewIndex >= CrewMembers.Length/2)
            //        CrewMembers[CrewIndex].CallChangeCrewRole(GunCrewMember.GunCrewRole.GunOperator);
            //    else
            //        CrewMembers[CrewIndex].CallChangeCrewRole(GunCrewMember.GunCrewRole.Spotter);
            //}
            CrewMembers[0].CallChangeCrewRole(GunCrewRole.GunOperator, GunnerSubRole.Loader);
            CrewMembers[1].CallChangeCrewRole(GunCrewRole.GunOperator, GunnerSubRole.Lookout);
            //CrewMembers[2].CallChangeCrewRole(GunCrewRole.Spotter, SpotterSubRole.Radio);
            //CrewMembers[3].CallChangeCrewRole(GunCrewRole.Spotter, SpotterSubRole.Runner);
            //for (int CrewIndex = 0; CrewIndex < CrewMembers.Length; CrewIndex++)
            //{
            //    CrewMembers[CrewIndex].HasSpawnCompleated = true;
            //}
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
            if (ArtillerySpawnManager.Instance != null)
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
                NodeManager.Instance.gameObject.AddComponent<ArtillerySpawnManager>();
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
