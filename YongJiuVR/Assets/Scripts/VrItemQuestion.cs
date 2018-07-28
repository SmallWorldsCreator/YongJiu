using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemQuestion : VrItem {
	public E_LEVEL startLevel;
	public override void OnContentDone() {
		LevelManager.instance.StartLevel (startLevel);
	}
}
