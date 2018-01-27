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

	public bool IsBranchingPath;

	public struct PacketData
	{
		public float msElapsed;
		public string label;
	}

	private GameObject spawnedCanvas;




	void Start ()
	{
		
	}

	public void Init(PacketData inPktData, bool inBranchingPathValue)
	{
		packetData = inPktData;
		IsBranchingPath = inBranchingPathValue;

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

	void Update ()
	{
		
	}
}
