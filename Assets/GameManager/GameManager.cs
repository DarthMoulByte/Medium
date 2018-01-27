using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private LevelGenerator levelGeneratorInstance;
	public bool DebugMsg;
	public static GameManager Instance;

	public System.Action<TargetNode> OnSpawnedNode;

	void Awake()
	{
		levelGeneratorInstance = GetComponentInChildren<LevelGenerator>();

		if (levelGeneratorInstance != null)
		{
			levelGeneratorInstance.OnSpawnedNode += Event_SpawnedNode;
			if (DebugMsg) Debug.Log("Game manager bound to generator's spawn event");
		}
	}

	private void Event_SpawnedNode(TargetNode inNode)
	{
		if (DebugMsg) Debug.Log("Game Manager notified by spawn.");

		if (OnSpawnedNode != null)
		{
			OnSpawnedNode(inNode);
		}
	}
	
	void Update ()
	{
		
	}
}
