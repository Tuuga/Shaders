using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLineOfSight : MonoBehaviour {

	enum HitInfo { None, Left, Right, Both }

	struct VertexInfo {
		public HitInfo hitInfo;
		public Vector3 dirFromPlayer;

		public VertexInfo (HitInfo info, Vector3 dir) {
			hitInfo = info;
			dirFromPlayer = dir;
		}
	}

	public LayerMask mask;
	public Material shadowMat;
	public float forkAngle;
	public float distPad;
	public float shadowRadius;

	List<Obstacle> obstacles;
	List<GameObject> shadows = new List<GameObject>();
	int shadowIndex;

	void Start () {
		obstacles = new List<Obstacle>(FindObjectsOfType<Obstacle>());
		foreach (Obstacle o in obstacles) {
			var shadow = new GameObject("Shadow");
			shadow.AddComponent<MeshFilter>();
			shadow.AddComponent<MeshRenderer>();
			shadows.Add(shadow);
		}
	}
	
	void Update () {
		foreach (Obstacle o in obstacles) {
			var verts = o.GetWorld2DVerts();
			var quadVerts = new Vector3[4];
			foreach (Vector3 v in verts) {
				var hit = DoubleCast(transform.position, v, o);
				if (hit.hitInfo == HitInfo.Left) {
					quadVerts[0] = v;
					quadVerts[3] = transform.position + hit.dirFromPlayer * shadowRadius;
				}
				if (hit.hitInfo == HitInfo.Right) {
					quadVerts[1] = v;
					quadVerts[2] = transform.position + hit.dirFromPlayer * shadowRadius;
				}
			}
			GenerateQuad(quadVerts);
		}
		shadowIndex = 0;
	}

	VertexInfo DoubleCast (Vector3 origin, Vector3 end, Obstacle obst) {
		HitInfo info = HitInfo.None;
		var dir = (end - origin).normalized;
		var dist = (end - origin).magnitude;

		var left = Quaternion.AngleAxis(forkAngle / 2, Vector3.up) * dir;
		var right = Quaternion.AngleAxis(-forkAngle / 2, Vector3.up) * dir;

		var hits = new List<RaycastHit>(Physics.RaycastAll(origin, left, dist + distPad, mask));
		foreach (RaycastHit hit in hits) {
			if (hit.collider.GetComponent<Obstacle>() == obst) {
				info = HitInfo.Left;
			}
		}

		hits = new List<RaycastHit>(Physics.RaycastAll(origin, right, dist + distPad, mask));
		foreach (RaycastHit hit in hits) {
			if (hit.collider.GetComponent<Obstacle>() == obst) {
				if (info == HitInfo.Left) {
					info = HitInfo.Both;
				} else {
					info = HitInfo.Right;
				}
			}
		}

		return new VertexInfo(info, dir);
	}

	void GenerateQuad (Vector3[] vertices) {
		var quad = shadows[shadowIndex];

		var mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = new int[] { 2, 1, 0, 2, 0, 3 };
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		var mf = quad.GetComponent<MeshFilter>();
		var mr = quad.GetComponent<MeshRenderer>();
		mf.mesh = mesh;
		mr.material = shadowMat;

		shadowIndex++;
	}
}
