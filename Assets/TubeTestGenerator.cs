using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeTestGenerator : MonoBehaviour
{
	[SerializeField] private TestType _testType;

	[Header("Circle Settings")]
	public int circleResolution = 16;

	[Header("Transform List Settings")]
	public List<Transform> transformList;

	[Header("General Settings")]
	public int tubeResolution = 8;
	public bool makeClosedCircle;
	public float radius = 2f;

	private enum TestType
	{
		Circle,
		Transforms
	}

	void Start ()
	{
		List<Vector3> positions = new List<Vector3>();
		switch (_testType)
		{
			case TestType.Circle:
				positions = GenerateTestCircle(circleResolution);
				break;
			case TestType.Transforms:
				positions = GenerateFromTransforms(transformList);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		MeshGenerator.GenerateTubeFromPositions(positions, radius, tubeResolution, makeClosedCircle, 0, MeshGenerator.DefaultSplineMaterial);

	}

	private List<Vector3> GenerateFromTransforms(List<Transform> transforms)
	{
		var positions = new List<Vector3>();

		foreach (var t in transforms)
		{
			positions.Add(t.position);
		}

		return positions;
	}

	List<Vector3> GenerateTestCircle(int circleResolution)
	{
		var positions = new List<Vector3>();

		for (int i = 0; i < circleResolution; i++)
		{
			var ratio = (1f / circleResolution) * i;

			Debug.Log("RATIO OKAY: " + ratio);
			var pos = MeshGenerator.GetPositionOnCircle(Vector3.zero, Vector3.down	 , Vector2.one * 10f, ratio);
			positions.Add(pos);
		}

		foreach (var position in positions)
		{
			Debug.Log("POS AFTER: " + position);
		}

		return positions;

	}
}
