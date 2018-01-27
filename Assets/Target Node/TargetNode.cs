using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
	private PacketData packetData;

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




	void Start ()
	{
		
	}

	public void Init(PacketData inPktData, bool inBranchingPathValue)
	{
		packetData = inPktData;
		IsBranchingPath = inBranchingPathValue;
	}

	void Update ()
	{
		
	}
}
