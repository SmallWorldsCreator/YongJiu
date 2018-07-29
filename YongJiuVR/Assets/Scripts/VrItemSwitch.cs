using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemSwitch : VrItem {
	[NullAlarm]public VrItem switchTarget;

	public virtual void OnContentDone() {
		base.OnContentDone ();
		gameObject.SetActive (false);
		switchTarget.gameObject.SetActive (true);
	}
}
