using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLineOfSight : MonoBehaviour {

	enum HitInfo { None, Left, Right, Both }

	struct VertexInfo {
		public HitInfo hitInfo;
		public Vector3 dirFromPlayer;
		public Vector3 point;

		public VertexInfo (HitInfo hitInfo, Vector3 dirFromPlayer, Vector3 point) {
			this.hitInfo = hitInfo;
			this.dirFromPlayer = dirFromPlayer;
			this.point = point;
		}
	}

	public LayerMask mask;
	public Material shadowMat;
	public float forkAngle;
	public float distPad;
	public float shadowRadius;

	List<Obstacle> obstacles;
	List<GameObject> shadows = new List<GameObject>();

	void Start () {
		obstacles = new List<Obstacle>(FindObjectsOfType<Obstacle>());
	}
	
	void Update () {
		
		//foreach (Obstacle o in obstacles) {
		//	var verts = o.GetWorld2DVerts();
		//	var quadVerts = new Vector3[4];
		//	foreach (Vector3 v in verts) {
		//		var hit = DoubleCast(transform.position, v, o);
		//		if (hit.hitInfo == HitInfo.Left) {
		//			quadVerts[0] = v;
		//			quadVerts[3] = transform.position + hit.dirFromPlayer * shadowRadius;
		//		}
		//		if (hit.hitInfo == HitInfo.Right) {
		//			quadVerts[1] = v;
		//			quadVerts[2] = transform.position + hit.dirFromPlayer * shadowRadius;
		//		}
		//	}
		//	GenerateQuad(quadVerts);
		//}
		

		//var triVerts = new List<Vector3>();
		//foreach (Obstacle o in obstacles) {
		//	var verts = o.GetWorld2DVerts();
		//	foreach (Vector3 v in verts) {
		//		var dir = (v - transform.position).normalized;
		//		RaycastHit hit;
		//		Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, mask);

		//		if (hit.point == v) {
		//			Debug.DrawLine(transform.position, hit.point, Color.green);
		//			triVerts.Add(v);
		//			if (triVerts.Count == 2) {
		//				GenerateTriabgle(new Vector3[] { transform.position, triVerts[0], triVerts[1] });
		//				triVerts = new List<Vector3>();
		//			}
		//		} else {
		//			Debug.DrawLine(transform.position, hit.point, Color.red);
		//		}
		//	}
		//}
	}

	VertexInfo DoubleCast (Vector3 origin, Vector3 end, Obstacle obst) {
		HitInfo info = HitInfo.None;
		var dir = (end - origin).normalized;
		var dist = (end - origin).magnitude;
		var hitPoint = new Vector3();

		var left = Quaternion.AngleAxis(forkAngle / 2, Vector3.up) * dir;
		var right = Quaternion.AngleAxis(-forkAngle / 2, Vector3.up) * dir;

		var hits = new List<RaycastHit>(Physics.RaycastAll(origin, left, dist + distPad, mask));
		foreach (RaycastHit hit in hits) {
			if (hit.collider.GetComponent<Obstacle>() == obst) {
				hitPoint = hit.point;
				info = HitInfo.Left;
			}
		}

		hits = new List<RaycastHit>(Physics.RaycastAll(origin, right, dist + distPad, mask));
		foreach (RaycastHit hit in hits) {
			if (hit.collider.GetComponent<Obstacle>() == obst) {
				hitPoint = hit.point;
				if (info == HitInfo.Left) {
					info = HitInfo.Both;
				} else {
					info = HitInfo.Right;
				}
			}
		}

		return new VertexInfo(info, dir, hitPoint);
	}

	void GenerateQuad (Vector3[] vertices) {
		var mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = new int[] { 2, 1, 0, 2, 0, 3 };
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, shadowMat, 0);
	}

	void GenerateTriangle (Vector3[] vertices) {
		var mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = new int[] { 0, 1, 2 };
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, shadowMat, 0);
	}
}
