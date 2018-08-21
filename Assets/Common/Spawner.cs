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

	public bool checkForEmptySpace = false;
	public LayerMask layerMask;

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
			Vector3 randomPosition = new Vector3();

			// Check for things in the way and try again (Bail after 100 tries)

			if (checkForEmptySpace)
			{
				for (int i = 0; i < 100; i++)
				{
					randomPosition = transform.position + new Vector3(Random.Range(-arenaSize, arenaSize), 0,
										Random.Range(-arenaSize, arenaSize));
					if (!Physics.CheckSphere(randomPosition, 0.1f, layerMask, QueryTriggerInteraction.Ignore))
//					if (!Physics.Raycast(randomPosition, Vector3.up, 1f, layerMask, QueryTriggerInteraction.Ignore))
//					if (!Physics.Raycast(randomPosition, Vector3.up, 10f))
					{
						Debug.DrawLine(randomPosition, randomPosition + Vector3.up * 5f, Color.green, 3);
						Debug.Log("Spawner: Found empty spot");
						break;
					}
					else
					{
						Debug.DrawLine(randomPosition, randomPosition + Vector3.up*5f, Color.red, 3);
						Debug.Log("Spawner: Location blocked, trying again");
					}
				}
			}
			else
			{
				randomPosition = transform.position + new Vector3(Random.Range(-arenaSize, arenaSize), 0,
									Random.Range(-arenaSize, arenaSize));
			}

			var newGO = Instantiate(item, randomPosition, Quaternion.identity);

			if (OnSpawnedNewGameObject != null)
				OnSpawnedNewGameObject(newGO);
		}
	}
}
