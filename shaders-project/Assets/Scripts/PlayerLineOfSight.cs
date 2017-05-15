using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLineOfSight : MonoBehaviour {
	struct HitPoint {
		public Vector3 point;
		public float angle;

		public HitPoint (Vector3 point, float angle) {
			this.point = point;
			this.angle = angle;
		}
	}

	public LayerMask mask;
	public Material shadowMat;
	public float offsetAngle;
	public bool debugDraw;

	public List<int> indices;

	List<Obstacle> obstacles;

	void Start () {
		obstacles = new List<Obstacle>(FindObjectsOfType<Obstacle>());
	}

	void Update () {
		var hitPoints = new List<HitPoint>();
		foreach (Obstacle o in obstacles) {
			var verts = o.GetWorld2DVerts();
			foreach (Vector3 v in verts) {
				hitPoints.Add((GetHitPoint(transform.position, v, 0)));
				hitPoints.Add((GetHitPoint(transform.position, v, offsetAngle)));
				hitPoints.Add((GetHitPoint(transform.position, v, -offsetAngle)));
			}
		}

		hitPoints.Sort((x, y) => x.angle.CompareTo(y.angle));
		hitPoints.Reverse();

		if (Input.GetKeyDown(KeyCode.Space)) {
			foreach (HitPoint h in hitPoints) {
				print(h.angle);
			}
		}

		var vertices = new List<Vector3>();
		vertices.Add(transform.position);
		foreach (HitPoint h in hitPoints) {
			vertices.Add(h.point);
			if (debugDraw) {
				Debug.DrawLine(transform.position + Vector3.up, h.point + Vector3.up, Color.red);
			}
		}

		indices = new List<int>();
		int index = 0;
		while (index + 2 < vertices.Count) {
			indices.Add(0);
			indices.Add(index + 1);
			indices.Add(index + 2);
			index++;
		}
		// Connect last and first
		indices.Add(0);
		indices.Add(vertices.Count - 1);
		indices.Add(1);

		var mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = indices.ToArray();
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, shadowMat, 0);
	}

	HitPoint GetHitPoint (Vector3 origin, Vector3 towards, float offsetAngle) {
		var dir = (towards - origin).normalized;
		dir = Quaternion.AngleAxis(offsetAngle, Vector3.up) * dir;

		RaycastHit hit;
		if (Physics.Raycast(origin, dir, out hit, Mathf.Infinity, mask)) {
			float angle = Mathf.Atan2(hit.point.z - origin.z, hit.point.x - origin.x) * Mathf.Rad2Deg;
			return new HitPoint(hit.point, angle);
		} else {
			var point = dir * 100;
			float angle = Mathf.Atan2(point.z - origin.z, point.x - origin.x) * Mathf.Rad2Deg;
			return new HitPoint(point, angle);
		}
	}
}
