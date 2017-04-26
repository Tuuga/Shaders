using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Distortion : MonoBehaviour {

	public Shader shader;
	public float strength;

	Material mat;

	void Awake () {
		mat = new Material(shader);
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		if (strength == 0) {
			Graphics.Blit(source, destination);
			return;
		}

		mat.SetFloat("_Strength", strength);

		Graphics.Blit(source, destination, mat);
	}

}
