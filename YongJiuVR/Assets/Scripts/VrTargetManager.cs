using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrTargetManager : ManagerBase<VrTargetManager> {
	[NullAlarm]public FloorPoint firstFloor;
	[NullAlarm]public VrItem nowLookItem;
	[LockInInspector]public VrCloseContent nowItemCloseBut;
	List<VrTarget> targets = new List<VrTarget>();
	// Use this for initialization
	public override void Init () {
		targets = GameObjectExtend.GetComponentsInScene<VrTarget> ();
		VrPlayerManager.instance.SetFloorPoint (firstFloor);
	}
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void CloseAllTarget(){
		foreach (VrTarget _target in targets) {
			_target.CloseTarget();
		}
	}

	public void OpenTargetAtFloorPoint(FloorPoint p_floorPoint){
		foreach (VrTarget _target in p_floorPoint.canSeeTargets) {
			_target.OpenTarget();
		}
	}
}
