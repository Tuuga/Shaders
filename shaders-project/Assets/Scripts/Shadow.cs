﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

	public Shader shader;
	public Color color;

	Material mat;

	void Awake () {
		mat = new Material(shader);
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination) {

		mat.SetTexture("_MainTex", source);
		mat.SetColor("_Color", color);
	
		Graphics.Blit(source, destination, mat);
	}
}
