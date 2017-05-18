using System.Collections;
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

		//var pScreenPos = Camera.main.WorldToScreenPoint(player.position);
		//playerUVPos = new Vector2(pScreenPos.x / Camera.main.pixelWidth, pScreenPos.y / Camera.main.pixelHeight);

		mat.SetTexture("_MainTex", source);
		mat.SetColor("_SecondTex", color);
		mat.SetFloat("_t", color.a);
	
		Graphics.Blit(source, destination, mat);
	}
}