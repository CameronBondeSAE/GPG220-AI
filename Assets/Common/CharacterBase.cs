using System;
using UnityEngine;
using System.Collections;

public class CharacterBase : MonoBehaviour
{
	// GLOBALLY accessible events. They DON'T below to individual Characters
	public static event Action<CharacterBase> OnSpawned;
	public static event Action<CharacterBase> OnDestroyed;

	public string characterName;
	public string debugText;

    [SerializeField]
	private float speedMultiplier = 1f;
	public virtual float SpeedMultiplier
	{
		get { return speedMultiplier; }
		set { speedMultiplier = value; }
	}

    [SerializeField]
    private float decelerationMultiplier = 0.9f;
    public virtual float DecelerationMultiplier
    {
        get { return decelerationMultiplier; }
        set { decelerationMultiplier = value; }
    }

    [SerializeField]
	private float damageMultiplier = 1f;
	public virtual float DamageMultiplier
	{
		get { return damageMultiplier; }
		set { damageMultiplier = value; }
	}

	public Vector3 Target
	{
		get { return target; }
		set { target = value; }
	}

	[SerializeField] private Vector3 target;


	public virtual void Start()
	{
		if (OnSpawned != null) OnSpawned(this);
	}

	public virtual void OnDestroy()
	{
		if (OnDestroyed != null) OnDestroyed(this);
	}


	//	public event Action<CharacterBase> OnDeathEvent;
	//
	//	/// This calls the "OnDeathEvent" event
	//	// Events can only be called from WITHIN the class that defined them (for sanity checking reasons)
	//	protected virtual void CallOnDeath(CharacterBase obj)
	//	{
	//		var handler = OnDeathEvent;
	//		if (handler != null) handler(obj);
	//	}
	public virtual void Ability1()
	{
//		print("CharacterBase: Ability1");
	}
	public virtual void Ability2()
	{
//		print("CharacterBase: Ability2");
	}
	public virtual void Ability3()
	{
//		print("CharacterBase: Ability3");
	}
	public virtual void Ability4()
	{
//		print("CharacterBase: Ability4");
	}
	public virtual void Ability5()
	{
//		print("CharacterBase: Ability5");
	}
	/// <summary>
	/// This is assumed to be WORLD direction + speed. NOT local speed
	/// </summary>
	/// <param name="speed"></param>
	public virtual void Move(Vector3 speedDirection)
	{
//		print("CharacterBase: Move");
	}

	[System.Obsolete]
	public virtual void SetMovementTarget(Vector3 position)
	{
//		print("CharacterBase: SetMovementTarget = "+position);
	}
	/// <param name="position"></param>
//	public virtual void SetTarget(Vector3 position)
//	{
//		target = position;
//	}
}
