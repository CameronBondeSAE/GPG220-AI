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

        [Space]
        [Header("Variables")]
        public Artillery OwnerGC;
        public bool HasSpawnCompleated = false;

        [Space]
        [Header("Debugging")]
        public bool VisualDebugging = false;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

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
            OwnerGC.CurrentSpawnStatus = ArtillerySpawnManager.ArtillerySpawnStatus.GunDroppingIn;
            transform.position = _SpawnLocation.WorldPosition;

            //Placeholderspawn animation
            yield return new WaitForSeconds(3);
            Mesh.SetActive(true);

            HasSpawnCompleated = true;
            OwnerGC.CurrentSpawnStatus = ArtillerySpawnManager.ArtillerySpawnStatus.GunLanded;
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