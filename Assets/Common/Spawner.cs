using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
	public List<GameObject> prefabs;
	public int maxClones;
	public int arenaSize;

	public event Action<GameObject> OnSpawnedNewGameObject;

	public float timedSpawnInterval;

	// Use this for initialization
	void Awake()
	{
		foreach (GameObject item in prefabs)
		{
			for (int i = 0; i < maxClones; i++)
			{
				Spawn(item);
			}
		}

		if (timedSpawnInterval > 0)
		{
			InvokeRepeating("Spawn", timedSpawnInterval, timedSpawnInterval);
		}
	}

	void Spawn()
	{
		Spawn(prefabs[prefabs.Count-1]);
	}

	private void Spawn(GameObject item)
	{
		if (item != null)
		{
			var newGO = Instantiate(item, transform.position +
			                              new Vector3(Random.Range(-arenaSize, arenaSize), 0, Random.Range(-arenaSize, arenaSize)), Quaternion.identity);

			if (OnSpawnedNewGameObject != null)
				OnSpawnedNewGameObject(newGO as GameObject);
		}
	}
}
