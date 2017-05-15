using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

	public Shader shader;
	public Color color;

	public Transform player, obstacle;

	public List<Vector3> verts = new List<Vector3>();
	public List<Vector3> verts2D = new List<Vector3>();
	List<Transform> visuals = new List<Transform>();

	MeshFilter obsFilter;
	
	Material mat;

	void Awake () {
		mat = new Material(shader);
		obsFilter = obstacle.GetComponent<MeshFilter>();
	}

	void Start () {
		var tempVerts = obsFilter.mesh.vertices;
		foreach (Vector3 v in tempVerts) {
			var tempVert = v;
			if (!verts.Contains(tempVert)) {

				if (!ContainsYExcluded(verts2D, tempVert)) {
					var visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					visual.transform.localScale = Vector3.one * 0.1f;
					visuals.Add(visual.transform);
					verts2D.Add(new Vector3(tempVert.x, 0, tempVert.z));
				}
				verts.Add(tempVert);
			}
		}
	}

	void Update () {
		for (int i = 0; i < verts2D.Count; i++) {
			var vertWorld = GetWorldPos(obstacle, verts2D[i]);
			vertWorld.y = 0;
			visuals[i].position = vertWorld;
		}
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {

		//var pScreenPos = Camera.main.WorldToScreenPoint(player.position);
		//playerUVPos = new Vector2(pScreenPos.x / Camera.main.pixelWidth, pScreenPos.y / Camera.main.pixelHeight);

		mat.SetTexture("_MainTex", source);
		mat.SetColor("_Color", color);
	
		Graphics.Blit(source, destination, mat);
	}

	Vector3 GetWorldPos (Transform t, Vector3 v) {
		var result = v;

		// Scale
		result.x *= t.lossyScale.x;
		result.y *= t.lossyScale.y;
		result.z *= t.lossyScale.z;
		// Rotation
		result = t.rotation * result;
		// World pos
		result += t.position;

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
}