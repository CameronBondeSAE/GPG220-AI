using System;
using UnityEngine;
using System.Collections;

public class Energy : MonoBehaviour {

	// Variables
	[SerializeField]
	private float amount = 100;

	public float Amount
	{
		get
		{
			return amount;
		}
		set
		{
			amount = value;
			Change(0);
		}
	}

    public Action getenegy { get; internal set; }

    [SerializeField]
	private float maxEnergy = 100;

    public float MaxEnergy
    {
        get
        {
            return maxEnergy;
        }
    }

    public float regenEnergySpeed = 1;


	// Event definitions
	public event Action OnNoEnergyEvent;
	public event Action OnReducingEvent;
	public event Action OnGainingEvent;


	private void Update()
	{
		Change(regenEnergySpeed * Time.deltaTime);
	}

	// Functions
	public void Change(float amountOfChange)
	{
		amount = amount + amountOfChange;

		if (amount > maxEnergy)
		{
			amount = maxEnergy;
		}

		// Fire off events
		if (amountOfChange > 0)
		{
			if (OnGainingEvent != null) OnGainingEvent();
		}
		else
		{
			if (OnReducingEvent != null) OnReducingEvent();
		}
		CheckForNoEnergy();
	}

	private void CheckForNoEnergy()
	{
		// Check if it’s smaller than OR EQUAL TO zero.
		if (amount <= 0)
		{
			RunOutOfEnergy();
		}
	}

	private void RunOutOfEnergy()
	{
		if (OnNoEnergyEvent != null) OnNoEnergyEvent();
	}

}
