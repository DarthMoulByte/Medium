using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
	public static Material DefaultSplineMaterial
	{
		get
		{
			if (_defaultSplineMaterial == null)
			{
				_defaultSplineMaterial = Resources.Load<Material>("Default Spline Material");
			}
			return _defaultSplineMaterial;
		}
	}

	private static Material _defaultSplineMaterial;

	public static void GenerateTubeFromSpline(Spline spline)
	{
		int splineResolution = 3;

		var positions = new List<Vector3>();
		for (int i = 0; i < spline.nodeList.Count; i++)
		{
			var thisNode = spline.nodeList[i];

			for (int j = 0; j < splineResolution; j++)
			{
				var ratio = (1f / splineResolution) * j;
				var pos = spline.GetPositionInSpline(i, ratio);

				positions.Add(pos);
			}

			if (thisNode.IsRouterNode || i == spline.nodeList.Count - 1)
			{
				GenerateTubeFromPositions(positions, 3f, 16);
				positions = new List<Vector3>();
			}
		}
	}

	public static void GenerateTubeFromPositions(List<Vector3> positions, float radius, int circleResolution, bool makeClosedCircle = false, int smoothingIterations = 0)
	{
		smoothingIterations = 3;
		if (smoothingIterations > 0)
		{
			positions = SmoothPositions(positions, smoothingIterations);
		}

		var go = new GameObject("SplineMesh");
//		go.transform.position = positions[0];

		var meshRenderer = go.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = DefaultSplineMaterial;
		var meshFilter = go.AddComponent<MeshFilter>();

		var mesh = new Mesh();
		mesh.name = "Generated Tube";

		List<Vector3> sourceNormals = new List<Vector3>();
		List<Vector4> sourceTangents = new List<Vector4>();

		var tubeVerts = GetTubePositions(positions, radius, circleResolution, out sourceNormals, out sourceTangents, makeClosedCircle);

		var vertices = new Vector3[(positions.Count-1) * circleResolution * 4];
		var uvs = new Vector2[vertices.Length];
		var normals = new Vector3[vertices.Length];
		var tangents = new Vector4[vertices.Length];

		int q = 0;

		for (int v = 0, i = 0; v < vertices.Length; v += 4, i++)
		{
			if (i != 0 && i % circleResolution == 0) q++; // quad index

			var qr = (1f / 4f * circleResolution) * i % circleResolution;

			var v1 = i;
			var v2 = (i + 1);

			if ((i+1) % circleResolution == 0)
			{
				v2 = q * circleResolution;
			}

			var v3 = v1 + circleResolution;
			var v4 = v2 + circleResolution;

			vertices[v] = 	  tubeVerts[v1];
			vertices[v + 1] = tubeVerts[v2];
			vertices[v + 2] = tubeVerts[v3];
			vertices[v + 3] = tubeVerts[v4];

			normals[v] = 	 sourceNormals[v1];
			normals[v + 1] = sourceNormals[v2];
			normals[v + 2] = sourceNormals[v3];
			normals[v + 3] = sourceNormals[v4];

			tangents[v] = 	 sourceTangents[v1];
			tangents[v + 1] = sourceTangents[v2];
			tangents[v + 2] = sourceTangents[v3];
			tangents[v + 3] = sourceTangents[v4];

			uvs[v]     = new Vector2(0, 0);
			uvs[v + 1] = new Vector2(qr, 0);
			uvs[v + 2] = new Vector2(0, 1);
			uvs[v + 3] = new Vector2(qr, 1);
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
		mesh.uv = uvs;
		mesh.normals = normals;

		// tangents are broken
//		mesh.tangents = tangents;
		mesh.RecalculateTangents();

		mesh.RecalculateBounds();
		meshFilter.sharedMesh = mesh;

	}

	private static List<Vector3> SmoothPositions(List<Vector3> positions, int iterations)
	{
		for (int it = 0; it < iterations; it++)
		{
			for (int i = positions.Count - 1; i >= 0; i-=2)
			{
				var thisPoint = positions[i];
				var nextPoint = positions[Mathf.Min(positions.Count-1, i + 1)];
				var previousPoint = positions[Mathf.Max(0, i - 1)];

				var midPoint = (thisPoint + previousPoint) * 0.5f;

				positions.Insert(i, midPoint);
			}

			for (int i = positions.Count - 1; i >= 0; i-= 2)
			{
				var thisPoint = positions[i];
				var nextPoint = positions[Mathf.Min(positions.Count-1, i + 1)];
				var previousPoint = positions[Mathf.Max(0, i - 1)];

				var midPoint = (nextPoint + previousPoint) * 0.5f;

				thisPoint = Vector3.Lerp(thisPoint, midPoint, 0.5f);

				positions[i] = thisPoint;
			}
			for (int i = positions.Count - 2; i >= 0; i-= 2)
			{
				var thisPoint = positions[i];
				var nextPoint = positions[Mathf.Min(positions.Count-1, i + 1)];
				var previousPoint = positions[Mathf.Max(0, i - 1)];

				var midPoint = (nextPoint + previousPoint) * 0.5f;

				thisPoint = Vector3.Lerp(thisPoint, midPoint, 0.5f);

				positions[i] = thisPoint;
			}
			for (int i = positions.Count - 1; i >= 0; i-= 2)
			{
				var thisPoint = positions[i];
				var nextPoint = positions[Mathf.Min(positions.Count-1, i + 1)];
				var previousPoint = positions[Mathf.Max(0, i - 1)];

				var midPoint = (nextPoint + previousPoint) * 0.5f;

				thisPoint = Vector3.Lerp(thisPoint, midPoint, 0.5f);

				positions[i] = thisPoint;
			}
		}

		return positions;
	}

	static List<Vector3> GetTubePositions(List<Vector3> centerPositions, float radius, int circleResolution, out List<Vector3> normals, out List<Vector4> tangents, bool makeClosedCircle = false)
	{
		if (makeClosedCircle)
		{
			var firstPos = centerPositions[0];
			centerPositions.Add(new Vector3(firstPos.x, firstPos.y, firstPos.z));
		}

		normals = new List<Vector3>();
		tangents = new List<Vector4>();

		List<Vector3> tubeVerts = new List<Vector3>();

		Vector3 directionToNext = Vector3.up;
		Vector3 directionToPrevious = Vector3.up;

		for (int iLength = 0; iLength < centerPositions.Count; iLength++)
		{
			Vector3 position = centerPositions[iLength];

			var previousPosition = centerPositions[Mathf.Clamp(iLength-1, 0, centerPositions.Count)];
			var nextPosition = centerPositions[Mathf.Clamp(iLength + 1, 0, centerPositions.Count-1)];

			directionToNext = (nextPosition - position).normalized;
			directionToPrevious = (position - previousPosition).normalized;

			var usedDirection = -directionToNext;

			if (iLength == centerPositions.Count - 1) // last node
			{
				usedDirection = directionToPrevious;
			}
			else if (iLength == 0) // first node
			{
				usedDirection = directionToNext;
			}
			else // all the nodes in between
			{
//				usedDirection = (directionToNext + directionToPrevious).normalized;
//				usedDirection = Vector3.Cross(directionToNext, directionToPrevious);
				usedDirection = (nextPosition - previousPosition).normalized;
			}

			List<Vector3> ringVerts = new List<Vector3>();

			for (int iCircle = 0; iCircle < circleResolution; iCircle++)
			{
				var ringVertPos = GetPositionOnCircle(position, usedDirection, Vector2.one * radius, (1f/circleResolution)*iCircle);
				var normal = (position - ringVertPos).normalized;
				ringVerts.Add(ringVertPos);
				normals.Add(normal);
				var tangent = Vector3.Cross(normal, usedDirection);
				tangents.Add(new Vector4(tangent.x, tangent.y, tangent.z, 1));
//				Debug.DrawRay(ringVertPos, normal, Color.red, 20f);
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

		if (forward == Vector3.zero)
		{
			forward = Vector3.forward;
		}
		var mat = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(forward), Vector3.one);
		var pos = new Vector3(x, y, z);

		pos -= center;
		pos = mat * pos;
		pos += center;

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
