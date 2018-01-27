using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeTestGenerator : MonoBehaviour {

	void Start ()
	{
		int points = 100;

		var positions = new List<Vector3>();

		for (int i = 0; i < points; i++)
		{
			var ratio = (1f / points) * i;

			Debug.Log("RATIO OKAY: " + ratio);
			var pos = MeshGenerator.GetPositionOnCircle(Vector3.zero, Vector3.down , Vector2.one * 10f, ratio);
			positions.Add(pos);
		}

		foreach (var position in positions)
		{
			Debug.Log("POS AFTER: " + position);
		}

		MeshGenerator.GenerateTubeFromPositions(positions, 2f, 10);
	}
}
