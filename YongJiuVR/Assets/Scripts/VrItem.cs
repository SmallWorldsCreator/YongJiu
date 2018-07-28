using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItem : VrTarget {
	[NullAlarm]public Animator Anime;
	[NullAlarm]public VrCloseContent closeBut;
	[NullAlarm]public Transform infoObj;
	// Use this for initialization
	void Start () {
		if (closeBut != null) {
			closeBut.item = this;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override void OnVrPointEnter() {
		if (VrTargetManager.instance.nowLookItem != null) {
			if (VrTargetManager.instance.nowLookItem.closeBut == null) {
				base.OnVrPointEnter ();
				VrTargetManager.instance.nowLookItem.HideContent ();
			}
		}
	}

	public override void OnVrRunEvent() {
		base.OnVrRunEvent ();
		ShowContent ();
	}

	public void OnBecameInvisible(){
		if (closeBut == null) {
			HideContent ();
		}
	}

	public void ShowContent(){		
		Anime.SetBool ("Show", true);
		if (closeBut != null) {
			closeBut.OpenTarget ();
		}
		if (infoObj != null) {
			infoObj.position = VrPlayerManager.instance.infoAnchor.position;
			infoObj.rotation = VrPlayerManager.instance.infoAnchor.rotation;
		}
		VrTargetManager.instance.nowLookItem = this;
		VrTargetManager.instance.nowItemCloseBut = closeBut;
	}

	public void HideContent(){
		Anime.SetBool ("Show", false);
		VrTargetManager.instance.nowLookItem = null;
		VrTargetManager.instance.nowItemCloseBut = null;
	}

	public virtual void OnContentDone() {}

}
