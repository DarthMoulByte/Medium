using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public bool DebugMsg;
	public float MoveSpeed = 50.0f;

	public float TurnMultiplier = 1.0f;

	public float OffsetResetLerp = 2.0f;
	public float CableWidth = 2.0f;
	public float InputSensitivity = 2.0f;

	public AnimationCurve TravelCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

	private GameObject mesh;

	private Vector3 pathOffset;
	private Vector3 pathOffsetTarget;
	private Vector3 inputVector;
	
	private GameManager gameManagerInstance;
	private List<TargetNode> nodeList = new List<TargetNode>();

	private TargetNode currentTargetNode;

	private float travelTimer;

	private Vector3 fromTravelPos;

	void Awake()
	{
		fromTravelPos = transform.position;

		gameManagerInstance = FindObjectOfType<GameManager>();

		mesh = GetComponentInChildren<MeshRenderer>().gameObject;

		if (gameManagerInstance != null)
		{
			gameManagerInstance.OnSpawnedNode += Event_SpawnedNode;
			if (DebugMsg) Debug.Log("Player bound to GameManager's spawn event");
		}
	}
	
	// Add a new target node to the end of the list. Don't need all the points before we start traveling.
	private void Event_SpawnedNode(TargetNode inNode)
	{
		if (DebugMsg) Debug.Log("Player notified by spawned node");
		nodeList.Add(inNode);
	}

	void Update()
	{
		if (nodeList.Count > 0)
		{
			// Always travel towards the first element in the list.
			currentTargetNode = nodeList[0];

			if (currentTargetNode != null)
			{
				Vector3 nextPos = currentTargetNode.transform.position;
				Vector3 fromSelfToTarget = nextPos - transform.position;
				fromSelfToTarget.Normalize();

				Vector3 fromPrevToNext = nextPos - fromTravelPos;

				// Move towards target along a curve:
				travelTimer += Mathf.Min(Time.deltaTime * MoveSpeed, 1.0f);
				float lerpValue = TravelCurve.Evaluate(travelTimer);

				Vector3 handlePos1 = fromTravelPos + fromPrevToNext.normalized * TurnMultiplier;
				Vector3 handlePos2 = nextPos;

				if (currentTargetNode.NextNode != null)
				{
					handlePos2 -= (currentTargetNode.NextNode.transform.position - nextPos).normalized * TurnMultiplier;
				}

				if (DebugMsg)
				{
					Debug.DrawLine(fromTravelPos, nextPos, Color.red);
					Debug.DrawLine(fromTravelPos, handlePos1, Color.green);
					Debug.DrawLine(nextPos, handlePos2, Color.green);
				}

				Vector3 pointOnCurve = Spline.CalculateCubicBezierPoint(lerpValue, fromTravelPos, handlePos1, handlePos2, nextPos);

				inputVector = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0.0f);
				inputVector *= Time.deltaTime * InputSensitivity;

				pathOffsetTarget += inputVector;
				pathOffsetTarget = Vector3.Lerp(pathOffsetTarget, Vector3.zero, Time.deltaTime * OffsetResetLerp);

				pathOffset = Vector3.Lerp(pathOffset, pathOffsetTarget, Time.deltaTime * 4.0f);

				pathOffsetTarget = Vector3.ClampMagnitude(pathOffsetTarget, CableWidth);
				pathOffset = Vector3.ClampMagnitude(pathOffset, CableWidth);

				mesh.transform.localPosition = pathOffset;
				//mesh.transform.localPosition = Vector3.Lerp(mesh.transform.localPosition, Vector3.zero, Time.deltaTime * OffsetResetLerp);


				transform.position = pointOnCurve;

				transform.rotation = Quaternion.LookRotation(fromSelfToTarget);

				if ((nextPos.z - transform.position.z) < 0.1f)
				{
					fromTravelPos = transform.position;

					nodeList.RemoveAt(0);
					if (DebugMsg) Debug.Log("Player: Reached target.");
					travelTimer = 0.0f;
				}
			}
		}
	}
}
