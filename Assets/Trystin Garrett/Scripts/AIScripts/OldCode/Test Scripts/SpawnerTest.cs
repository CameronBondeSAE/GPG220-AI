using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trystin
{
    public class SpawnerTest : MonoBehaviour
    {
        [Header("To Spawn Prefabs")]
        public GameObject ArtilleryPiecePrefab;

        public void SpawnArtilleryPiecePrefab()
        {
            if(ArtilleryPiecePrefab != null)
                Instantiate(ArtilleryPiecePrefab);
        }
    }
}
