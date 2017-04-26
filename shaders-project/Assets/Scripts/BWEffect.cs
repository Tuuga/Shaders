using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BWEffect : MonoBehaviour {

	[Range(0, 1)]
	public float intensity;
	Material mat;

	void Awake () {
		mat = new Material(Shader.Find("Hidden/BWDiffuse"));
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		if (intensity == 0) {
			Graphics.Blit(source, destination);
			return;
		}

		mat.SetFloat("_bwBlend", intensity);
		Graphics.Blit(source, destination, mat);
	}
}
