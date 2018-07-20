using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPoint : VrTarget {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnVrRunEvent() {
		VrPlayerManager.instance.SetFloorPoint(this);
	}
}
