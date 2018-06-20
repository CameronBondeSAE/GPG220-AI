using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunCrewMember : CharacterBase
    {
        public enum GunCrewRole
        {
            UnAssigned,
            Spotter,
            GunOperator
        }

        [Space]
        [Header("AI Components")]
        public TestMovementAI MovementScript;
        public TestVisionAI VistionScript;
        public GameObject Mesh;

        [Space]
        [Header("Crew Variables")]
        public GunCrew OwnerGC;
        public GunCrewRole CrewRole = GunCrewRole.UnAssigned;
        public bool HasSpawnCompleated = false;

        // Update is called once per frame
        void Update()
        {

        }

        //
        public void CallChangeCrewRole(GunCrewRole _NewRole)
        {
            switch(_NewRole)
            {
                case GunCrewRole.GunOperator:
                    CrewRole = GunCrewRole.GunOperator;
                    break;
                case GunCrewRole.Spotter:
                    CrewRole = GunCrewRole.Spotter;
                    break;
            }
        }

        //
        public void CallAnimateSpawnIn(GunCrew _OwnerGC, Node _SpawnLocation)
        {
            SetupGunCrewMember(_OwnerGC);
            if (_SpawnLocation != null && OwnerGC != null)
            {
                gameObject.SetActive(true);
                StartCoroutine(AnimateSpawnIn(_SpawnLocation));
            }

        }
        IEnumerator AnimateSpawnIn(Node _SpawnLocation)
        {
            transform.position = _SpawnLocation.WorldPosition;
            float RanTimeWait = Random.Range(0f, 1.5f);
            yield return new WaitForSeconds(RanTimeWait);

            //Placeholderspawn animation
            yield return new WaitForSeconds(3);
            Mesh.SetActive(true);

            HasSpawnCompleated = true;
            ++OwnerGC.CrewSetupCounter;
            if (OwnerGC.CrewSetupCounter == OwnerGC.CrewMembers.Length)
                OwnerGC.CurrentSpawnStatus = GunCrewSpawnManager.GunSpawnStatus.CrewLanded;
        }

        // Setsup Components
        public void SetupGunCrewMember(GunCrew _OwnerGC)
        {
            MovementScript = GetComponent<TestMovementAI>();
            VistionScript = GetComponent<TestVisionAI>();
            OwnerGC = _OwnerGC;
        }
    }
}
