using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CrossFade : MonoBehaviour {

	public Shader shader;
	public Camera overlayCam;
	[Range(0, 1)]
	public float t;

	Material mat;
	Camera cam;
	RenderTexture overlay;

	void Awake () {
		cam = GetComponent<Camera>();
		mat = new Material(shader);

		overlay = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);
		overlay.name = "Crossfade";
		overlayCam.targetTexture = overlay;
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		if (t == 0) {
			Graphics.Blit(source, destination);
			return;
		} else if (t == 1) {
			Graphics.Blit(overlay, destination);
			return;
		}

		mat.SetFloat("_t", t);
		mat.SetTexture("_SecondTex", overlay);

		Graphics.Blit(source, destination, mat);
	}
}
