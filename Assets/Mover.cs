using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{

	private Vector3 _startPosition;
	public float range = 2;
	public float speed = 1;

	private Vector3 _currentPosition;

	void Start ()
	{
		_startPosition = transform.position;
	}
	
	void Update ()
	{
		float t = Time.time;
		_currentPosition.x = Mathf.Sin(t * speed) * range;
		_currentPosition.y = Mathf.Sin(t * speed + Mathf.PI) * range;
		_currentPosition.z = Mathf.Cos(t * speed + Mathf.PI) * range;

		transform.position = _currentPosition;
	}
}
