using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunCrewMember : CharacterBase
    {

        [Header("AI Components")]
        public List<Command> ActiveOrders = new List<Command>();
        public MovementCommand MovementOrder;

        public StateMachine ThisStateMachine;
        public TestMovementAI MovementScript;
        public TestVisionAI VistionScript;

        [Space]
        [Header("Crew Variables")]
        public Artillery OwnerGC;
        public GameObject Mesh;
        public Node OccupiedNode;
        public AIStatus CurrentStatus = AIStatus.InActive;
        public GunCrewRole CrewRole = GunCrewRole.UnAssigned;
        public GunnerSubRole GunnerSubRole = GunnerSubRole.Idle;
        public SpotterSubRole SpotterSubRole = SpotterSubRole.Idle;
        public bool HasSpawnCompleated = false;


        // Update is called once per frame
        void Update()
        {
            if(CurrentStatus == AIStatus.Active)
                ThisStateMachine.UpdateSM();
        }


        //
        public void CallChangeCrewRole(GunCrewRole _NewRole, GunnerSubRole _GunnerSubRole)
        {
            switch(_NewRole)
            {
                case GunCrewRole.GunOperator:
                    CrewRole = GunCrewRole.GunOperator;
                    GunnerSubRole = _GunnerSubRole;
                    SpotterSubRole = SpotterSubRole.Idle;
                    ThisStateMachine.SetupStateMachine(this);
                    break;
            }
        }
        public void CallChangeCrewRole(GunCrewRole _NewRole, SpotterSubRole _SpotterSubRole)
        {
            switch (_NewRole)
            {
                case GunCrewRole.Spotter:
                    CrewRole = GunCrewRole.Spotter;
                    GunnerSubRole = GunnerSubRole.Idle;
                    SpotterSubRole = _SpotterSubRole;
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
            OccupiedNode = _SpawnLocation;
            float RanTimeWait = Random.Range(0f, 1.5f);
            yield return new WaitForSeconds(RanTimeWait);

            //Placeholderspawn animation
            yield return new WaitForSeconds(3);
            Mesh.SetActive(true);

            ++OwnerGC.CrewSetupCounter;
            if (OwnerGC.CrewSetupCounter == OwnerGC.CrewMembers.Length)
                OwnerGC.CurrentSpawnStatus = ArtillerySpawnManager.ArtillerySpawnStatus.CrewLanded;
        }

        // Setsup Components
        public void SetupGunCrewMember(Artillery _OwnerGC)
        {
            //MovementScript = GetComponent<TestMovementAI>();
            //VistionScript = GetComponent<TestVisionAI>();
            OwnerGC = _OwnerGC;
            //SetUpSM();
        }
        #endregion
    }
}
