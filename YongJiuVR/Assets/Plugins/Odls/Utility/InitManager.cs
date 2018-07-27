using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitManager : ManagerBase<InitManager> {
	[ReorderableList]public List<InitableBehaviour> initObjs = new List<InitableBehaviour>();
	public bool autoInitWhenAwake;
	void Awake(){
		if (autoInitWhenAwake) {
			InitAllObject ();
		}
	}

	void InitAllObject(){
		foreach(InitableBehaviour _obj in initObjs){
			_obj.Init ();
		}
	}
}
