using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	public GameObject NodePrefab;
	public Vector3 SpawnDeltaRange = new Vector3(10.0f, 0.0f, 10.0f);
	public int Iterations = 5;
	public bool UseFixedDistance = true;

	[Header("If not fixed distance between nodes:")]
	public float FixedDistance = 15.0f;
	public float MinZDistance = 10.0f;
	public float MsToUnitScale = 1.0f;

	public Spline GeneratedSpline { get; private set; }

	public System.Action<TargetNode> OnSpawnedNode;

	private Player playerInstance;
	private List<TargetNode> spawnedTargetNodeList = new List<TargetNode>();

	private void Awake()
	{
		GeneratedSpline = new Spline();
		playerInstance = FindObjectOfType<Player>();

		if (playerInstance != null)
		{
			playerInstance.OnReachedTarget += Event_OnPlayerReachedTarget;
		}
	}

	void Start()
	{
		GenerateNodes();

		MeshGenerator.GenerateTubeFromSpline(GeneratedSpline);
	}

	private void OnEnable()
	{
	}

	private void Event_OnPlayerReachedTarget(Player.TunnelChoice potentialTunnelChoice, TargetNode nodeThatWasReached)
	{
		// If they chose an alternative path, we must fix the bezier handles
	}

	private void GenerateNode(bool isBranchingPath, bool isRouterNode = false, TargetNode.PacketData inPktData = null)
	{
		Vector3 currentPos = transform.position;
		Vector3 nextPos = currentPos;
		TargetNode prevNode = null;

		// Is there a previous node?
		if (spawnedTargetNodeList.Count > 0)
		{
			prevNode = spawnedTargetNodeList[spawnedTargetNodeList.Count - 1];

			// Generate a position in a random direction, distance set by packet ms.
			if (prevNode != null)
			{
				currentPos = prevNode.transform.position;

				float randRangeX = Random.Range(-SpawnDeltaRange.x, SpawnDeltaRange.x);
				float randRangeY = Random.Range(-SpawnDeltaRange.y, SpawnDeltaRange.y);
				float randRangeZ = Random.Range(-SpawnDeltaRange.z, SpawnDeltaRange.z);

				randRangeZ = Mathf.Clamp(randRangeZ, MinZDistance, SpawnDeltaRange.z);

				Vector3 randomDir = new Vector3(randRangeX, randRangeY, Mathf.Abs(randRangeZ));
				randomDir.Normalize();

				if (UseFixedDistance)
				{
					nextPos = currentPos + randomDir * FixedDistance;
				}

				// Was the last node a branching point? If so, make another node at the inverse X position (relative to the branching point).
				if (prevNode.IsBranchingPath && !isRouterNode)
				{
					prevNode.GetComponent<Renderer>().material.color = Color.red; // TEMPORARY

					Vector3 relativePos = prevNode.transform.InverseTransformPoint(nextPos);

					nextPos.x = prevNode.transform.TransformPoint(relativePos).x;

					relativePos.x *= -1;
					Vector3 altNodePos = prevNode.transform.TransformPoint(relativePos);

					TargetNode spawnedAltNode = Instantiate(NodePrefab, altNodePos, Quaternion.identity).GetComponent<TargetNode>();
					prevNode.AlternativeNode = spawnedAltNode;
					spawnedAltNode.IsTheAlternativeNode = true;
				}

				if (prevNode.IsBranchingPath || isRouterNode)
				{
					isBranchingPath = false;
				}
			}
		}


		// Spawn a new node, and set it to be the 'next node' for the previous one, then add it to the list.
		TargetNode spawnedNode = Instantiate(NodePrefab, nextPos, Quaternion.identity).GetComponent<TargetNode>();

		if (spawnedNode)
		{
			spawnedNode.Init(isBranchingPath, isRouterNode, inPktData);

			if (prevNode != null)
			{
				//print("LevelGenerator: Set the previous node's next node to the spawned one");
				prevNode.NextNode = spawnedNode;

				// If there are two previous nodes, make sure there's a NextNode on the potential alternative path.
				TargetNode prevPrevNode = null;
				if (spawnedTargetNodeList.Count > 1)
				{
					prevPrevNode = spawnedTargetNodeList[spawnedTargetNodeList.Count - 2];

					if (prevPrevNode && prevPrevNode.IsBranchingPath && prevPrevNode.AlternativeNode != null)
					{
						prevPrevNode.AlternativeNode.NextNode = spawnedNode;

						float dist1 = Vector3.Distance(prevNode.transform.position, nextPos);
						float dist2 = Vector3.Distance(prevPrevNode.AlternativeNode.transform.position, nextPos);

						if (dist1 < dist2)
						{
							prevNode.IsShortestPath = true;
							prevNode.GetComponent<Renderer>().material.color = Color.green;
						}
						else
						{
							prevPrevNode.AlternativeNode.IsShortestPath = true;
							prevPrevNode.GetComponent<Renderer>().material.color = Color.green;
						}
					}
				}
			}

			spawnedTargetNodeList.Add(spawnedNode);

			// Broadcast the spawn.
			if (OnSpawnedNode != null)
			{
				OnSpawnedNode(spawnedNode);

				//if (GeneratedSpline == null)
				//{
				//	print("Making new spline");
				//	GeneratedSpline = new Spline();
				//}

				GeneratedSpline.AddNode(spawnedNode);


			}
		}
	}

	private void GenerateNodes()
	{
		for (int i = 0; i < Iterations; i++)
		{
			TargetNode.PacketData pktData = new TargetNode.PacketData();
			pktData.msElapsed = Random.Range(25.0f, 100.0f);
			pktData.label = "192.168.128." + (i + 1).ToString();

			GenerateNode(false, true, pktData);

			for (int j = 0; j < 5; j++)
			{
				bool branchingPath = (Random.Range(0, 10) < 4) ? true : false;
				GenerateNode(branchingPath);

			}

		}
	}
	
}
