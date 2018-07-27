using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemAnswer : VrItem {
	public override void OnHideContent() {
		base.OnVrRunEvent ();
		LevelManager.instance.EndLevel ();
	}
}
