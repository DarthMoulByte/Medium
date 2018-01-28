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

        var current = nodeList[index];
        var next = nodeList[index];
        var nextNext = nodeList[index];
        var previous = nodeList[index];

        if (index == 0) // first
        {
            next = nodeList[index + 1];
            nextNext = nodeList[index + 1 + 1];
        }
        else if (index == nodeList.Count - 1 || index == nodeList.Count - 2) // last
        {
            previous = nodeList[index - 1];
        }
        else
        {
            next = nodeList[index + 1];
            previous = nodeList[index - 1];
            nextNext = nodeList[index + 1 + 1];
        }

        var nextPos = next.transform.position;
        var nextNextPos = nextNext.transform.position;
        var currentPos = current.transform.position;
        var prevPos = previous.transform.position;

        Vector3 fromTravelPos = Vector3.zero;

        if (index - 1 >= 0)
        {
            fromTravelPos = nodeList[index - 1].transform.position;
        }

        Vector3 fromPrevToNext = nextPos - fromTravelPos;

        Vector3 handlePos1 = fromTravelPos + fromPrevToNext.normalized * HandleScale;
        Vector3 handlePos2 = nextPos;

        handlePos1 = prevPos;
        handlePos2 = currentPos;

        HandleScale = 10;

        var h1Dir = (nextPos - prevPos).normalized;
        var h2Dir = (nextPos - nextNextPos).normalized;

        h1Dir = Vector3.forward;
        h2Dir = Vector3.back;

        h1Dir *= HandleScale;
        h2Dir *= HandleScale;
        handlePos1 = prevPos + h1Dir;
        handlePos2 = currentPos + h2Dir;

        Debug.DrawLine(prevPos, handlePos1, Color.red, 20);
        Debug.DrawLine(currentPos, handlePos2, Color.green, 20);

        return CalculateCubicBezierPoint(time, prevPos, handlePos1, handlePos2, currentPos);
    }
}