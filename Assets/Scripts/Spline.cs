using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline
{
	public float HandleScale = 2.5f;
	public List<TargetNode> nodeList = new List<TargetNode>();

	public Spline()
	{

	}

	public Spline(List<TargetNode> inNodeList)
	{
		nodeList = inNodeList;
	}

	public void AddNode(TargetNode inNode)
	{
		nodeList.Add(inNode);
	}

	public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float u = 1 - t;
		float tt = t * t;
		float uu = u * u;
		float uuu = uu * u;
		float ttt = tt * t;

		Vector3 p = uuu * p0;
		p += 3 * uu * t * p1;
		p += 3 * u * tt * p2;
		p += ttt * p3;

		return p;
	}

	public Vector3 GetPositionInSpline(int index, float time)
	{
		if (index > nodeList.Count - 1)
		{
			Debug.LogError("Index out of range on Spline");
			return Vector3.one * -99999.0f;
		}

		Vector3 fromTravelPos = Vector3.zero;

		if (index - 1 >= 0)
		{
			fromTravelPos = nodeList[index - 1].transform.position;
		}

		Vector3 nextPos = nodeList[index].transform.position;
		Vector3 fromPrevToNext = nextPos - fromTravelPos;

		Vector3 handlePos1 = fromTravelPos + fromPrevToNext.normalized * HandleScale;
		Vector3 handlePos2 = nextPos;

		return CalculateCubicBezierPoint(time, fromTravelPos, handlePos1, handlePos2, nextPos);
	}
}
