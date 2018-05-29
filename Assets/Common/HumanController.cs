using UnityEngine;

public class HumanController : MonoBehaviour
{
	private ControllerBase TargetController;
	public CharacterBase TargetModel;
	[SerializeField] private KeyCode ability1Key = KeyCode.LeftShift;
	[SerializeField] private KeyCode ability2Key = KeyCode.LeftAlt;
	[SerializeField] private KeyCode ability3Key = KeyCode.Q;
	[SerializeField] private KeyCode ability4Key = KeyCode.E;
	[SerializeField] private KeyCode ability5Key = KeyCode.F;

	public Camera mainCamera;
	private CharacterBase lastCharacterBase;

	// Use this for initialization
	public void Start()
	{
		mainCamera = FindObjectOfType<Camera>();

		if (TargetModel)
		{
			Possess(TargetModel);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (TargetModel != null)
		{
			TargetModel.Move(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));

			if (Input.GetKeyDown(ability1Key))
				TargetModel.Ability1();
			if (Input.GetKeyDown(ability2Key))
				TargetModel.Ability2();
			if (Input.GetKeyDown(ability3Key))
				TargetModel.Ability3();
			if (Input.GetKeyDown(ability4Key))
				TargetModel.Ability4();
			if (Input.GetKeyDown(ability5Key))
				TargetModel.Ability5();

			// Mouse position target
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			RaycastHit hitInfo;
			Physics.Raycast(ray, out hitInfo);
			Debug.DrawLine(hitInfo.point, hitInfo.point + Vector3.up * 10, Color.red);

			TargetModel.Target = hitInfo.point;
		}
	}

	public void Possess(CharacterBase characterBase)
	{
		// Turn BACK on the last character's controller
		if (lastCharacterBase)
		{
			var controllerBase = lastCharacterBase.GetComponent<ControllerBase>();
			if (controllerBase)
			{
				controllerBase.enabled = true;
			}
		}

		// Turn OFF the currently selected character
		TargetModel = characterBase;
		TargetController = characterBase.GetComponent<ControllerBase>();
		if (TargetController != null) TargetController.enabled = false;

		if (TargetModel != null) lastCharacterBase = TargetModel;
	}
}
