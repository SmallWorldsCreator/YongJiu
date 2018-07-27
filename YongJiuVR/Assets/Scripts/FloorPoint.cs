using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPoint : VrTarget {
	public VrTarget[] canSeeTargets;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnVrRunEvent() {
		VrPlayerManager.instance.SetFloorPoint(this);
	}
	public override void OnOpenTarget() {
		gameObject.SetActive (true);
	}
	public override void OnCloseTarget() {
		gameObject.SetActive (false);
	}
}
