using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemAnswer : VrItem {
	public E_LEVEL endLevel;
	public override void OnShowContent() {
		base.OnShowContent ();
		SoundManager.Play ("SoundTable","Yes");

	}
	public override void OnContentDone() {
		base.OnContentDone ();
		LevelManager.instance.EndLevel (endLevel);
	}
}
