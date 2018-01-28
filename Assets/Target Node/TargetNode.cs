using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetNode : MonoBehaviour
{
	private PacketData packetData;

	public GameObject NodeCanvasPrefab;

	public TargetNode NextNode;
	public TargetNode AlternativeNode = null;

	// Only relevant if IsBranchingPath is true
	public bool IsShortestPath;
	public bool IsTheAlternativeNode;
	public bool IsBranchingPath;
	public bool IsRouterNode;

	public float msToTravel = 20.0f;

	public class PacketData
	{
		public PacketData()
		{
		}

		public float msElapsed;
		public string label;
	}

	private GameObject spawnedCanvas;




	void Start ()
	{
		
	}

	public void Init(bool inBranchingPathValue, bool inRouterNodeValue = false, PacketData inPktData = null)
	{
		IsBranchingPath = inBranchingPathValue;
		IsRouterNode = inRouterNodeValue;

		if (IsRouterNode)
		{
			if (spawnedCanvas == null)
			{
				spawnedCanvas = Instantiate(NodeCanvasPrefab);
				spawnedCanvas.transform.SetParent(transform);
				spawnedCanvas.transform.position = transform.position + Vector3.up * 1.0f;
				spawnedCanvas.transform.rotation = transform.rotation;
				spawnedCanvas.transform.localScale = Vector3.one * 0.005f;
			}

			Text textComp = spawnedCanvas.GetComponentInChildren<Text>();
			if (textComp != null)
			{
				textComp.text = inPktData.label;
			}
		}

		msToTravel = Random.Range(5.0f, 40.0f);
	}

	void Update ()
	{
		
	}
}
