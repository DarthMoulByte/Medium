using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnThingsAlongSpline : MonoBehaviour
{
	public GameObject[] objectsToSpawnRandomly;

	public int spawnResolution = 10;

	public float spawnChance = 0.01f;
	public LevelGenerator LevelGenerator;
	void Start ()
	{
		LevelGenerator = FindObjectOfType<LevelGenerator>();

		var spline = LevelGenerator.GeneratedSpline;

		for (int i = 0; i < spline.nodeList.Count; i++)
		{
			var thisNode = spline.nodeList[i];

			for (int j = 0; j < LevelGenerator.splineResolution; j++)
			{
				var ratio = (1f / LevelGenerator.splineResolution) * j;
				var pos = spline.GetPositionInSpline(i, ratio);

				var rot = Quaternion.LookRotation((spline.GetPositionInSpline(i, ratio+0.01f) - spline.GetPositionInSpline(i, ratio - 0.01f)).normalized);

				if (spawnChance > Random.value)
				{
					Instantiate(objectsToSpawnRandomly[0], pos, rot);
				}


			}
		}

	}
	
	void Update ()
	{
		
	}
}
