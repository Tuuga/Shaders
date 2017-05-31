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
	public float worldScale;
	public float u;

	public List<int> debugInds;
	public List<Vector3> debugVerts;
	public List<Vector2> debugUV;

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
		var uv = GetGlobalUV(vertices);

		debugVerts = vertices;
		debugInds = indices;
		debugUV = uv;

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

	List<Vector2> GetGlobalUV (List<Vector3> verts) {
		var uvs = new List<Vector2>();

		for (int i = 0; i < verts.Count; i++) {
			var pos = verts[i];
			var uv = new Vector2();
			uv.x = (pos.x - transform.position.x + worldScale / 2) / worldScale;
			uv.y = (pos.z - transform.position.z + worldScale / 2) / worldScale;
			uvs.Add(uv);
		}
		return uvs;
	}

	List<Vector2> GetGradientUV (List<Vector3> verts) {
		var uvs = new List<Vector2>();

		//uvs.Add(new Vector2(0, 0));
		for (int i = 0; i < verts.Count; i++) {
			var pos = verts[i];
			var uv = new Vector2();
			uv.x = u;
			uv.y = Vector3.Distance(transform.position, pos) / worldScale;
			uvs.Add(uv);
		}
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
