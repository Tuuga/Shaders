using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {

	public float movementSpeed;
	
	void Update () {
		var dir = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
		if (dir.magnitude > 1) {
			dir = dir.normalized;
		}
		transform.position += dir * movementSpeed * Time.deltaTime;
	}
}
