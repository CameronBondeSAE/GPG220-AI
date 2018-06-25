using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class GunCrewMember : CharacterBase
    {

        [Header("AI Components")]
        public List<Command> ActiveOrders = new List<Command>();
        public Command RoleCommand;
        public MovementCommand MovementOrder;

        [Space]
        [Header("Crew Variables")]
        public Artillery OwnerGC;
        public GameObject Mesh;
        public Node OccupiedNode;
        public AIStatus CurrentStatus = AIStatus.InActive;
        public ArtilleryCrewRole CrewRole = ArtilleryCrewRole.UnAssigned;
        public float OrderDelay = 1.25f;

        // Update is called once per frame
        void Update()
        {
            if (OccupiedNode == null)
                OccupiedNode = NodeManager.Instance.FindNodeFromWorldPosition(gameObject.transform.position);

        }
        #region Setup Methods

        //Calls the spawn in and allows the GCM to do it's own thing.
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
                OwnerGC.CurrentSpawnStatus = ArtillerySpawnStatus.CrewLanded;
        }

        // Setsup Components
        public void SetupGunCrewMember(Artillery _OwnerGC)
        {
            OwnerGC = _OwnerGC;
        }
        #endregion
    }
}
