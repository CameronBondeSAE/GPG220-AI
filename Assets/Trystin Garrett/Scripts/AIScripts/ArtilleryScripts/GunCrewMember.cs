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

        [Header("AI Components")]
        public StateMachine ThisStateMachine;
        public StateTableType AITableType = StateTableType.Null;
        public TestMovementAI MovementScript;
        public TestVisionAI VistionScript;


        [Space]
        [Header("Crew Variables")]
        public Artillery OwnerGC;
        public GameObject Mesh;
        public GunCrewRole CrewRole = GunCrewRole.UnAssigned;
        public bool HasSpawnCompleated = false;


        // Update is called once per frame
        void Update()
        {
            //ThisStateMachine.UpdateSM();
        }


        //
        public void CallChangeCrewRole(GunCrewRole _NewRole)
        {
            switch(_NewRole)
            {
                case GunCrewRole.GunOperator:
                    CrewRole = GunCrewRole.GunOperator;
                    ThisStateMachine.SetupStateMachine(this);
                    break;
                case GunCrewRole.Spotter:
                    CrewRole = GunCrewRole.Spotter;
                    ThisStateMachine.SetupStateMachine(this);
                    break;
            }
        }

        #region Setup Methods

        //
        private void SetUpSM()
        {
            ThisStateMachine = new StateMachine();
            ThisStateMachine.SetupStateMachine(this);
        }

        //
        public void CallAnimateSpawnIn(Artillery _OwnerGC, Node _SpawnLocation)
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
                OwnerGC.CurrentSpawnStatus = ArtillerySpawnManager.ArtillerySpawnStatus.CrewLanded;
        }

        // Setsup Components
        public void SetupGunCrewMember(Artillery _OwnerGC)
        {
            MovementScript = GetComponent<TestMovementAI>();
            VistionScript = GetComponent<TestVisionAI>();
            OwnerGC = _OwnerGC;
            SetUpSM();
        }
        #endregion
    }
}
