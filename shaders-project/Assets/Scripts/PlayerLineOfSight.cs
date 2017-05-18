using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;

public class PlayerLineOfSight : MonoBehaviour {
	struct HitPoint {
		public Vector3 point;	// Point of the hit
		public float angle;		// Angle of the ray

		public HitPoint (Vector3 point, float angle) {
			this.point = point;
			this.angle = angle;
		}
	}

	public LayerMask mask;
	public Material losMat;
	public float offsetAngle;

	public List<int> inds;

	List<Obstacle> obstacles;
	public Mesh mesh;

	void Start () {
		mesh = new Mesh();
		mesh.name = "LOSMesh";
		obstacles = new List<Obstacle>(FindObjectsOfType<Obstacle>());
	}

	void Update () {
		var allVerts = GetAllVertices();
		var hitPoints = GetHitPoints(allVerts);
		var vertices = GetVertices(hitPoints);
		var indices = GetIndices(vertices);
		inds = indices;
		var uv = GetUV(vertices);
		GenerateMesh(vertices.ToArray(), indices.ToArray(), uv.ToArray(), mesh, losMat);    // Generate the los mesh
	}

	List<Vector3> GetAllVertices () {
		var allVerts = new List<Vector3>();
		foreach (Obstacle o in obstacles) {
			var verts = o.GetWorld2DVerts();
			foreach (Vector3 v in verts) {
				// doesn't add a vertex if meshes have overlapping vertices
				if (!allVerts.Exists(x => x == v)) {
					allVerts.Add(v);
				}
			}
		}

		return allVerts;
	}

	List<HitPoint> GetHitPoints (List<Vector3> verts) {
		var hitPoints = new List<HitPoint>();
		foreach (Vector3 v in verts) {
			var leftHitPoint = GetHitPoint(transform.position, v, -offsetAngle);
			var rightHitPoint = GetHitPoint(transform.position, v, offsetAngle);

			hitPoints.Add(leftHitPoint);
			hitPoints.Add(rightHitPoint);
		}

		hitPoints.Sort((x, y) => x.angle.CompareTo(y.angle));   // Sort hitPoints by angle
		hitPoints.Reverse();
		return hitPoints;
	}

	HitPoint GetHitPoint (Vector3 origin, Vector3 towards, float offsetAngle) {
		var dir = (towards - origin).normalized;
		dir = Quaternion.AngleAxis(offsetAngle, Vector3.up) * dir;	// Offset the direction

		RaycastHit hit;
		if (Physics.Raycast(origin, dir, out hit, Mathf.Infinity, mask)) {
			float angle = GetAngle(origin, hit.point);
			return new HitPoint(hit.point, angle);
		} else {
			var point = dir * 100;  // If no hits, return point at 100 units in direction
			float angle = GetAngle(origin, point);
			return new HitPoint(point, angle);
		}
	}

	float GetAngle (Vector3 p1, Vector3 p2) {
		return Mathf.Atan2(p2.z - p1.z, p2.x - p1.x) * Mathf.Rad2Deg;
	}

	List<int> GetIndices (List<Vector3> vertices) {
		int index = 0;
		List<int> indices = new List<int>();
		while (index + 2 < vertices.Count) {
			indices.Add(0);	// Vertex 0 is players position
			indices.Add(index + 1);
			indices.Add(index + 2);
			index++;
		}
		// Connect last and first
		indices.Add(0);
		indices.Add(vertices.Count - 1);
		indices.Add(1);

		return indices;
	}

	List<Vector3> GetVertices (List<HitPoint> hitPoints) {
		var vertices = new List<Vector3>();
		vertices.Add(transform.position);	// Set vertex 0 to player position
		foreach (HitPoint h in hitPoints) {
			vertices.Add(h.point);
		}

		return vertices;
	}

	List<Vector2> GetUV (List<Vector3> verts) {
		var uvs = new List<Vector2>();
		for (int i = 0; i < verts.Count - 3; i += 3) {

			Debug.DrawLine(verts[0], verts[i + 1], Color.red);
			Debug.DrawLine(verts[0], verts[i + 2], Color.red);
			Debug.DrawLine(verts[i + 1], verts[i + 2], Color.red);

			var p0p1 = Vector3.Distance(verts[0], verts[i + 1]);
			var p0p2 = Vector3.Distance(verts[0], verts[i + 2]);
			var p1p2 = Vector3.Distance(verts[i + 1], verts[i + 2]);

			uvs.Add(new Vector2(0, 0));
			uvs.Add(new Vector2(p0p1, 0));
			uvs.Add(new Vector2(p0p2, p1p2));
		}

		var firstToLast = Vector3.Distance(verts[0], verts[verts.Count - 1]);
		var secondLastToLast = Vector3.Distance(verts[verts.Count - 2], verts[verts.Count - 1]);
		uvs.Add(new Vector2(firstToLast, secondLastToLast));

		//foreach (Vector3 v in verts) {
		//	var uv = Camera.main.WorldToScreenPoint(v);
		//	uv.x = uv.x / Screen.width;
		//	uv.y = uv.y / Screen.height;
		//	uvs.Add(uv);
		//}
		return uvs;
	}

	void GenerateMesh (Vector3[] verts, int[] inds, Vector2[] uv, Mesh mesh, Material mat) {
		mesh.vertices = verts;
		mesh.triangles = inds;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.uv = uv;
		Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, mat, 0);
	}
}
