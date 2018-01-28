using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public bool DebugMsg;
	public float MoveSpeed = 50.0f;

	public bool IsPlayer = true;

	public float TurnMultiplier = 1.0f;

	public float OffsetResetLerp = 2.0f;
	public float CableWidth = 2.0f;
	public float InputSensitivity = 2.0f;

	public AnimationCurve TravelCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
	public AnimationCurve EnterRouterCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

	public enum TunnelChoice { Left, Right }
	public System.Action<TunnelChoice, TargetNode> OnReachedTarget;
	public System.Action<TargetNode> OnChoseAlternativeTunnel;

	private GameObject mesh;

	private Vector3 pathOffset;
	private Vector3 pathOffsetTarget;
	private Vector3 inputVector;
	
	private GameManager gameManagerInstance;
	private List<TargetNode> nodeList = new List<TargetNode>();

	private TargetNode currentTargetNode;

	private LevelGenerator levelGenerator;

	private float travelTimer;
	private Vector3 lastFramePos;

	private Vector3 fromTravelPos;

	public Spline spline;

	void Awake()
	{
		fromTravelPos = transform.position;

		gameManagerInstance = FindObjectOfType<GameManager>();

		mesh = GetComponentInChildren<MeshRenderer>().gameObject;

		levelGenerator = FindObjectOfType<LevelGenerator>();

		if (gameManagerInstance != null)
		{
			gameManagerInstance.OnSpawnedNode += Event_SpawnedNode;
			if (DebugMsg) Debug.Log("Player bound to GameManager's spawn event");
		}

		if (!IsPlayer)
		{
			Vector3 randomDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
			pathOffset += randomDir.normalized * Random.Range(0.2f, levelGenerator.CableWidth);

			levelGenerator.OnAllNodesSpawned += Event_OnAllNodesSpawned;
			
		}

		Audio.PlayAudioSource(Audio.Instance.travel);
	}

	private void Start()
	{
		spline = levelGenerator.GeneratedSpline;
	}

	void Event_OnAllNodesSpawned()
	{
		//print("List count: " + nodeList.Count);
		if (!IsPlayer)
		{
			int pointToStartAt = Random.Range(0, nodeList.Count - 1);

			print("Spawning at node " + pointToStartAt);
			for (int i = 0; i < pointToStartAt; i++)
			{
				nodeList.RemoveAt(0);
			}
		}
	}

	public void OnCollided(Collider collider)
	{

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
				

				Vector3 fromPrevToNext = nextPos - fromTravelPos;

				// Figure out bezier stuff:

				Vector3 handlePos1 = fromTravelPos + fromPrevToNext.normalized * TurnMultiplier;
				Vector3 handlePos2 = nextPos;

				if (currentTargetNode.NextNode != null)
				{
					if (currentTargetNode.NextNode.IsTheAlternativeNode)
					{
						handlePos2 -= (currentTargetNode.AlternativeNode.transform.position - nextPos).normalized * TurnMultiplier;
					}
					else
					{
						handlePos2 -= (currentTargetNode.NextNode.transform.position - nextPos).normalized * TurnMultiplier;
					}
				}

				if (DebugMsg)
				{
					Debug.DrawLine(fromTravelPos, nextPos, Color.red);
					Debug.DrawLine(fromTravelPos, handlePos1, Color.green);
					Debug.DrawLine(nextPos, handlePos2, Color.green);
				}

				// Set the position along the curve

				// Increase lerp timer
				float lerpValue = -1.0f;
				AnimationCurve animCurveToUse = TravelCurve;	// Default

				if (currentTargetNode.NextNode != null &&
					currentTargetNode.IsRouterNode)
				{
					travelTimer += Mathf.Min(Time.deltaTime * 0.2f, 1.0f);
					animCurveToUse = EnterRouterCurve;
				}
				else
				{
					travelTimer += Mathf.Min(Time.deltaTime * MoveSpeed, 1.0f);
				}

				//print("Current target: " + currentTargetNode.transform.position);

				lerpValue = animCurveToUse.Evaluate(travelTimer);

				Vector3 pointOnCurve = transform.position;
				if (levelGenerator.spawnedTargetNodeList != null)
				{
					pointOnCurve = spline.GetPositionInSpline(levelGenerator.spawnedTargetNodeList.IndexOf(currentTargetNode), lerpValue);
				}

				transform.position = pointOnCurve;

				if (IsPlayer)
				{
					// Let the player move the mesh with mouse input
					inputVector = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0.0f);
					inputVector *= Time.deltaTime * InputSensitivity;

					pathOffsetTarget += inputVector;
					pathOffsetTarget = Vector3.Lerp(pathOffsetTarget, Vector3.zero, Time.deltaTime * OffsetResetLerp);

					pathOffset = Vector3.Lerp(pathOffset, pathOffsetTarget, Time.deltaTime * 4.0f);

					// Don't let the player move the mesh outside this area:
					pathOffsetTarget = Vector3.ClampMagnitude(pathOffsetTarget, levelGenerator.CableWidth);
					pathOffset = Vector3.ClampMagnitude(pathOffset, levelGenerator.CableWidth);

					mesh.transform.localPosition = pathOffset;
				}


				// Player 'looks' in the direction it's traveling
				Vector3 fromSelfToTarget = nextPos - transform.position;
				fromSelfToTarget.Normalize();
				Vector3 travelVector = transform.position - lastFramePos;
				lastFramePos = transform.position;

				transform.rotation = Quaternion.LookRotation(travelVector);


				// If we're close enough to 'reach' the next node:
				if ((nextPos.z - transform.position.z) < 0.05f)
				{
					fromTravelPos = transform.position;

					// For choosing the right or left tunnel:
					TunnelChoice potentialTunnelChoice = TunnelChoice.Right;
					if (mesh.transform.position.x < currentTargetNode.transform.position.x)
					{
						potentialTunnelChoice = TunnelChoice.Left;
					}

					// Is there a node after the one we're reaching now, and is the current one a branching path?
					if (currentTargetNode.NextNode != null &&
						currentTargetNode.IsBranchingPath &&
						currentTargetNode.AlternativeNode != null)
					{
						// Is the default next node the node we chose?
						if (currentTargetNode.AlternativeNode.transform.position.x < currentTargetNode.transform.position.x)
						{
							if (potentialTunnelChoice == TunnelChoice.Left)
							{
								if (nodeList.Count > 1)
								{
									nodeList[1] = currentTargetNode.AlternativeNode;
									if (OnChoseAlternativeTunnel != null)
										OnChoseAlternativeTunnel(nodeList[1]);
								}
							}
						}
						else if (currentTargetNode.AlternativeNode.transform.position.x >= currentTargetNode.transform.position.x)
						{
							if (potentialTunnelChoice == TunnelChoice.Right)
							{
								if (nodeList.Count > 1)
								{
									nodeList[1] = currentTargetNode.AlternativeNode;
									if (OnChoseAlternativeTunnel != null)
										OnChoseAlternativeTunnel(nodeList[1]);
								}
							}
						}
					}

					// Broadcast that we reached the target
					if (OnReachedTarget != null)
					{
						OnReachedTarget(potentialTunnelChoice, currentTargetNode);
					}

					if (DebugMsg) Debug.Log(potentialTunnelChoice + " tunnel");

					// Remove the node we reached from the list
					nodeList.RemoveAt(0);
					if (DebugMsg) Debug.Log("Player: Reached target.");
					travelTimer = 0.0f;
				}
			}
		}
	}
}
