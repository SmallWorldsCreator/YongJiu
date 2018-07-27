using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItem : VrTarget {
	static VrItem nowLookItem;
	[NullAlarm]public Animator Anime;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override void OnVrPointEnter() {
		if (nowLookItem != null) {
			nowLookItem.HideContent ();
		}
	}

	public override void OnVrRunEvent() {
		ShowContent ();
	}

	public void OnBecameInvisible(){
		HideContent ();
	}

	public void ShowContent(){		
		nowLookItem = this;
		Anime.SetBool ("Show", true);
	}

	public void HideContent(){
		nowLookItem = null;
		Anime.SetBool ("Show", false);
	}

	public virtual void OnHideContent() {}

}
