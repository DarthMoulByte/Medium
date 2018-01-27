using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public Vector3 RelativeOffset = new Vector3(0.0f, 1.0f, -2.0f);
	public float PositionLerpSpeed = 4.0f;
	public float RotationLerpSpeed = 1.0f;
	private Player playerInstance;

	private void Awake()
	{
		playerInstance = FindObjectOfType<Player>();
	}

	void Start ()
	{
		
	}

	void LateUpdate()
	{
		if (playerInstance != null)
		{
			Vector3 targetPosition = playerInstance.transform.position + playerInstance.transform.TransformDirection(RelativeOffset);
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * PositionLerpSpeed);

			Quaternion targetRotation = playerInstance.transform.rotation;
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * RotationLerpSpeed);
		}
	}
}
