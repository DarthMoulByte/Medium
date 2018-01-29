using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
	Player playerInstance;

	void Awake()
	{
		playerInstance = transform.parent.GetComponent<Player>();
	}

	void Start ()
	{
		
	}

	private void OnTriggerEnter(Collider collider)
	{
//		if (!collider.gameObject.CompareTag("Player"))
//		{
//			return;
//		}
		print("Collision");
		playerInstance.OnCollided(collider);
	}

	void Update ()
	{
		
	}
}
