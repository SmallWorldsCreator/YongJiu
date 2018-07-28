using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrCloseContent : VrTarget {
	[LockInInspector]public VrItem item;
	public override void OnVrRunEvent() {
		item.HideContent ();
	}
}
