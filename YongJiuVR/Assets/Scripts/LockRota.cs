using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRota : MonoBehaviour {
	public Vector3 euler;
	Quaternion rota;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 _euler = transform.rotation.eulerAngles;
		_euler.x = euler.x;
		_euler.z = euler.z;
		transform.rotation = Quaternion.Euler (_euler);
	}
}
