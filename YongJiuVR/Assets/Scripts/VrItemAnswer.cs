using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemAnswer : VrItem {
	public override void OnHideInfo() {
		base.OnVrRunEvent ();
		LevelManager.instance.EndLevel ();
	}
}
