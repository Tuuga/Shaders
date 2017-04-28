using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseTest;

public class PerlinMover : MonoBehaviour {

	struct Node {
		public Transform transform;
		public Vector3 gridPosition;

		public Node (Transform t, Vector3 gridPos) {
			transform = t;
			gridPosition = gridPos;
		}
	}

	public Vector3 gridSize;
	public Vector3 gridScale;
	public bool runTime;
	public float timeScale;
	public float time;
	List<Node> grid = new List<Node>();

	OpenSimplexNoise noise = new OpenSimplexNoise();

	void Start () {
		var parent = new GameObject("Grid");
		for (int z = 0; z < gridSize.z; z++) {
			for (int y = 0; y < gridSize.y; y++) {
				for (int x = 0; x < gridSize.x; x++) {
					var node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					node.name = "Node " + x + ", " + y + ", " + z;
					node.transform.parent = parent.transform;
					node.transform.position = new Vector3(x * gridScale.x, y * gridScale.y, z * gridScale.z);
					Node n = new Node(node.transform, new Vector3(x, y, z));
					grid.Add(n);
				}
			}
		}
	}
	
	void Update () {
		if (runTime) {
			time += Time.deltaTime * timeScale;
		}

		foreach (Node n in grid) {
			float x = (float)noise.Evaluate(n.gridPosition.x, n.gridPosition.y, n.gridPosition.z, time);
			float y = (float)noise.Evaluate(n.gridPosition.z, n.gridPosition.y, n.gridPosition.y, time);
			float z = (float)noise.Evaluate(n.gridPosition.x, n.gridPosition.z, n.gridPosition.y, time);

			var noisePos = new Vector3(n.gridPosition.x * gridScale.x + x, n.gridPosition.y * gridScale.y + y, n.gridPosition.z * gridScale.z + z);
			n.transform.position = noisePos;
		}
	}

	//Node GetNode (int x, int y) {
	//	return grid[x + (int)gridSize.x * y];
	//}
	//Node GetNode (float x, float y) {
	//	return GetNode((int)x, (int)y);
	//}
}
