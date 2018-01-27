using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Vector3 axis;

	public float speed = 1;

	void Update ()
	{
		transform.Rotate(axis, Time.deltaTime * speed);
	}

	private void OnValidate()
	{
		axis.Normalize();
	}
}
