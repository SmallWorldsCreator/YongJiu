using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoCunIdleObj : MonoBehaviour {
	public Animator anime;
	public string name;
	// Use this for initialization
	void Start () {
		anime.Play (name, -1, 0);
	}
	
	// Update is called once per frame
	void OnBecameInvisible () {
		if (Random.value < 0.2f) {
			ChoCunTop.instance.RandomChangeChoCun ();
		}
	}
}
