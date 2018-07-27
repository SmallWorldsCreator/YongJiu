using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemQuestion : VrItem {
	public E_LEVEL startLevel;
	public override void OnHideInfo() {
		LevelManager.instance.StartLevel (startLevel);
	}
}
