using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Timeline;

public class MeshGenerator : MonoBehaviour
{
//	[SerializeField]
//	private Transform[] _points;

//	[SerializeField] private int _circleResolution = 8;
//	[SerializeField] private int _lengthResolution = 10;

//	private List<Vector3> _vertices = new List<Vector3>();
//	private Vector3[] _vertices;

//	private MeshFilter meshFilter;
//	private MeshRenderer meshRenderer;
//	private Mesh mesh;

	public static void GenerateTubeFromSpline(Spline spline)
	{
		int splineResolution = 20;
		for (int i = 0; i < spline.nodeList.Count; i++)
		{
			var positions = new List<Vector3>();
			for (int j = 0; j < splineResolution; j++)
			{
				var ratio = (1 / splineResolution) * i;
				positions.Add(spline.GetPositionInSpline(i, ratio));
			}
			GenerateTubeFromPositions(positions, 5f, 12);
		}
	}

	public static void GenerateTubeFromPositions(List<Vector3> positions, float radius, int circleResolution)
	{
		var go = new GameObject("SPLINEMESH");
		go.transform.position = positions[0];

		var meshRenderer = go.AddComponent<MeshRenderer>();
//		meshRenderer.sharedMaterial =
		var meshFilter = go.AddComponent<MeshFilter>();

		var mesh = new Mesh();
		mesh.name = "Generated Tube";

		var tubeVerts = GetTubePositions(positions, radius, circleResolution);

		var vertices = new Vector3[(positions.Count-1) * circleResolution * 4];

		int q = 0;

		for (int v = 0, i = 0; v < vertices.Length; v += 4, i++)
		{
			if (i != 0 && i % circleResolution == 0) q++; // quad index

			var v1 = i;
			var v2 = (i + 1);

			if ((i+1) % circleResolution == 0)
			{
				v2 = q * circleResolution;
			}

			var v3 = v1 + circleResolution;
			var v4 = v2 + circleResolution;

//			Debug.Log("q" + q + "/v" + v + "/i" + i + ": " + v1 + " " + v2 + " " + v3 + " " + v4);

			vertices[v] = 	   tubeVerts[v1];
			vertices[v + 1] = tubeVerts[v2];
			vertices[v + 2] = tubeVerts[v3];
			vertices[v + 3] = tubeVerts[v4];
		}

		mesh.vertices = vertices;

		var triangles = new int[(positions.Count-1) * circleResolution * 6];

		for (int t = 0, v = 0; t < triangles.Length; t += 6, v += 4) {

			var v0 = v;
			var v1 = v + 1;
			var v2 = v + 2;
			var v3 = v + 2;
			var v4 = v + 1;
			var v5 = v + 3;

			triangles[t    ] = v0;
			triangles[t + 1] = v1;
			triangles[t + 2] = v2;

			triangles[t + 3] = v3;
			triangles[t + 4] = v4;
			triangles[t + 5] = v5;
		}

//		Debug.Log("Setting tris");
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		meshFilter.sharedMesh = mesh;

	}

	static List<Vector3> GetTubePositions(List<Vector3> centerPositions, float radius, int circleResolution)
	{
		List<Vector3> tubeVerts = new List<Vector3>();

		Vector3 direction = Vector3.up;

		for (int iLength = 0; iLength < centerPositions.Count; iLength++)
		{
			Vector3 position = centerPositions[iLength];

			Debug.Log(position);

			var nextPosition = centerPositions[Mathf.Clamp(iLength + 1, 0, centerPositions.Count-1)];
			direction = (position - nextPosition).normalized;

			List<Vector3> ringVerts = new List<Vector3>();

			for (int iCircle = 0; iCircle < circleResolution; iCircle++)
			{
				var ringVertPos = GetPositionOnCircle(position, direction, Vector2.one * radius, (1f/circleResolution)*iCircle);
				ringVerts.Add(ringVertPos);
			}

			tubeVerts.AddRange(ringVerts);
		}

		return tubeVerts;
	}

	public static Vector3 GetPositionOnCircle(Vector3 center, Vector3 forward, Vector2 radius, float ratioAroundCircle)
	{
		forward.Normalize();

		float x;
		float y;
		float z = center.z;

		x = center.x + Mathf.Sin(Mathf.PI * 2 * ratioAroundCircle) * radius.x;
		y = center.y + Mathf.Cos(Mathf.PI * 2 * ratioAroundCircle) * radius.y;

		var mat = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(forward), Vector3.one);
		var pos = new Vector3(x, y, z);

		pos -= center;
		pos = mat * pos;
		pos += center;

		Debug.Log("POS: " + pos);

		return pos;
	}

//	void Start ()
//	{
//		GenerateTubeFromPositions(new List<Vector3>(){new Vector3(0,0,0), new Vector3(0,0,500)});
//		_vertices = GetTubePositions(_points);
//	}
	
//	void Update ()
//	{
//		foreach (var vertex in mesh.vertices)
//		{
//			DrawCross(vertex, 0.5f);
//		}
//	}
//
//	void DrawCross(Vector3 pos, float size)
//	{
//		Debug.DrawLine(pos + Vector3.down * size * 0.5f, pos + Vector3.up * size * 0.5f);
//		Debug.DrawLine(pos + Vector3.left * size * 0.5f, pos + Vector3.right * size * 0.5f);
//		Debug.DrawLine(pos + Vector3.back * size * 0.5f, pos + Vector3.forward * size * 0.5f);
//	}
}
