using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
	public Vector3 RelativeOffset = new Vector3(0.0f, 1.0f, -2.0f);
	public float PositionLerpSpeed = 4.0f;
	public float RotationLerpSpeed = 1.0f;
	private Player playerInstance;
	private Transform actualObjectToTrack;

	private void Awake()
	{
		playerInstance = FindObjectOfType<Player>();
		//actualObjectToTrack = playerInstance.GetComponentInChildren<MeshRenderer>().transform;
		actualObjectToTrack = playerInstance.transform;
	}

	void Start ()
	{
		
	}

	void LateUpdate()
	{
		if (playerInstance != null)
		{
			Vector3 targetPosition = actualObjectToTrack.position + actualObjectToTrack.TransformDirection(RelativeOffset);
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * PositionLerpSpeed);

			Quaternion targetRotation = actualObjectToTrack.rotation;
			transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * RotationLerpSpeed);
		}
	}
}
