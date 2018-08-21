using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
	public Camera cam;
	private float normalFov;
	public float zoomFov;
	public float zoomSpeed;

	// Use this for initialization
	void Start()
	{
		normalFov = cam.fieldOfView;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomFov, zoomSpeed * Time.deltaTime);
		}
		else
		{
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFov, zoomSpeed * Time.deltaTime);
		}

	}
}
