using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemAnswer : VrItem {
	public override void OnContentDone() {
		base.OnContentDone ();
		LevelManager.instance.EndLevel ();
	}
}
