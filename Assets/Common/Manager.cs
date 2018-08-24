using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
	public List<CharacterBase> CharacterBases;

	public GameObject DeadGameObject;

	[SerializeField]
	private float uiOffset;

	// Health UI (HACK: should probably go in a UIManager)
	public ResourceBar HealthEnergyUIPrefab;

	ResourceBar healthEnergyUI;

	/// <summary>
	/// Options:
	/// Make a new Instantiate
	/// 
	/// </summary>


	void Awake()
	{
		CharacterBase.OnSpawned += OnSpawned;
		CharacterBase.OnDestroyed += OnDestroyed;
	}

	private void OnDestroyed(CharacterBase charBase)
	{
		CharacterBases.Remove(charBase);
	}

	private void OnSpawned(CharacterBase charBase)
	{
		CharacterBases.Add(charBase);

		healthEnergyUI = Instantiate(HealthEnergyUIPrefab);
		healthEnergyUI.owner = charBase;
	}

	public Vector3 thing;

	// Update is called once per frame
	void Update()
	{
		thing += new Vector3(0,0,1);
	}
}
