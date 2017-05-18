using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

	List<Vector3> verts = new List<Vector3>();
	List<Vector3> verts2D = new List<Vector3>();

	//List<Transform> visuals = new List<Transform>();

	MeshFilter filter;

	void Awake () {
		filter = GetComponent<MeshFilter>();
	}

	void Start () {
		var meshVerts = filter.mesh.vertices;
		foreach (Vector3 v in meshVerts) {
			if (!verts.Contains(v)) {
				if (!ContainsYExcluded(verts2D, v)) {
					//var visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					//visual.transform.localScale = Vector3.one * 0.1f;
					//visuals.Add(visual.transform);
					verts2D.Add(new Vector3(v.x, 0, v.z));
				}
				verts.Add(v);
			}
		}
	}

	//void Update () {
	//	for (int i = 0; i < verts2D.Count; i++) {
	//		var vertWorld = GetWorldPos(verts2D[i]);
	//		vertWorld.y = 0;
	//		visuals[i].position = vertWorld;
	//	}
	//}

	Vector3 GetWorldPos (Vector3 v) {
		var result = v;
		// Scale
		result.x *= transform.lossyScale.x;
		result.y *= transform.lossyScale.y;
		result.z *= transform.lossyScale.z;
		// Rotation
		result = transform.rotation * result;
		// World pos
		result += transform.position;

		return result;
	}

	bool ContainsYExcluded (List<Vector3> list, Vector3 comp) {
		foreach (Vector3 v in list) {
			if (v.x == comp.x && v.z == comp.z) {
				return true;
			}
		}
		return false;
	}

	public List<Vector3> Get2DVerts () {
		return verts2D;
	}

	public List<Vector3> GetWorld2DVerts () {
		var worldVerts = new List<Vector3>();
		foreach (Vector3 v in verts2D) {
			var worldV = GetWorldPos(v);
			worldV.y = 0;
			worldVerts.Add(worldV);
		}
		return worldVerts;
	}
}
