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
		print("Collision");	
		playerInstance.OnCollided(collider);
		//Audio.PlayAudioSource(Audio.Instance.error);
	}

	void Update ()
	{
		
	}
}
