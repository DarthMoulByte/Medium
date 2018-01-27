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

	private List<TargetNode> spawnedTargetNodeList = new List<TargetNode>();

	public System.Action<TargetNode> OnSpawnedNode;

	void Start()
	{
		GenerateNodes();
	}

	private void OnEnable()
	{
	}

	private void GenerateNode(TargetNode.PacketData inPacketData, bool isBranchingPath)
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
				else
				{
					nextPos = currentPos + randomDir * inPacketData.msElapsed * MsToUnitScale;
				}

				// Was the last node a branching point? If so, make another node at the inverse X position (relative to the branching point).
				if (prevNode.IsBranchingPath)
				{
					prevNode.GetComponent<Renderer>().material.color = Color.red; // TEMPORARY

					Vector3 relativePos = prevNode.transform.InverseTransformPoint(nextPos);

					//relativePos.x = Mathf.Clamp(relativePos.x, -5.0f, 5.0f);

					nextPos.x = prevNode.transform.TransformPoint(relativePos).x;

					relativePos.x *= -1;
					Vector3 altNodePos = prevNode.transform.TransformPoint(relativePos);

					TargetNode spawnedAltNode = Instantiate(NodePrefab, altNodePos, Quaternion.identity).GetComponent<TargetNode>();
					prevNode.AlternativeNode = spawnedAltNode;

					// If the last node was a branching pah, don't allow setting this as branching path..
					isBranchingPath = false;
				}
			}
		}


		// Spawn a new node, and set it to be the 'next node' for the previous one, then add it to the list.
		TargetNode spawnedNode = Instantiate(NodePrefab, nextPos, Quaternion.identity).GetComponent<TargetNode>();

		if (spawnedNode)
		{
			spawnedNode.Init(inPacketData, isBranchingPath);			

			if (prevNode != null)
			{
				//print("LevelGenerator: Set next node to the spawned one");
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
			}
		}

	}

	private void GenerateNodes()
	{
		for (int i = 0; i < Iterations; i++)
		{
			TargetNode.PacketData pktData;
			bool branchingPath = (Random.Range(0, 10) < 3) ? true : false;
			pktData.msElapsed = Random.Range(25.0f, 100.0f);
			pktData.label = "192.168.128.1";

			GenerateNode(pktData, branchingPath);
		}
	}

	//private void GenerateNodes_Old()
	//{
	//	Vector3 currentPos = transform.position;

	//	for (int i = 0; i < Iterations; i++)
	//	{
	//		// Spawn a node at 'currentPosition'. Then figure out the next 'currentPosition' for the next node.
	//		TargetNode spawnedNode = Instantiate(NodePrefab, currentPos, Quaternion.identity).GetComponent<TargetNode>();

	//		Vector3 randomOffset = Vector3.zero;

	//		if (spawnedNode)
	//		{
	//			spawnedTargetNodeList.Add(spawnedNode);

	//			float randRangeX = Random.Range(-SpawnDeltaRange.x, SpawnDeltaRange.x);
	//			float randRangeY = Random.Range(-SpawnDeltaRange.y, SpawnDeltaRange.y);
	//			float randRangeZ = Random.Range(-SpawnDeltaRange.z, SpawnDeltaRange.z);
	//			randomOffset = new Vector3(randRangeX, randRangeY, Mathf.Abs(randRangeZ));

	//			Vector3 nextPos = currentPos + randomOffset;

	//			// Is the distance between the current position and next position absurdly small?
	//			if (Vector3.Distance(currentPos, nextPos) < MinDistance)
	//			{
	//				// Can't be any closer to current point than MinDistance
	//				nextPos = nextPos + (nextPos - currentPos).normalized * MinDistance;
	//			}

	//			currentPos = nextPos;

	//		}
	//	}
	//}
	
}
