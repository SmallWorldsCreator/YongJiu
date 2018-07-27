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
			nowLookItem.HideInfo ();
		}
	}

	public override void OnVrRunEvent() {
		ShowInfo ();
	}

	public void OnBecameInvisible(){
		HideInfo ();
	}

	public void ShowInfo(){		
		nowLookItem = this;
		Anime.SetBool ("Show", true);
	}

	public void HideInfo(){
		if (nowLookItem == this) {
			VrTargetManager.instance.StartCoroutine (IeHideInfo ());
		}
	}

	IEnumerator IeHideInfo(){
		nowLookItem = null;
		Anime.SetBool ("Show", false);
		yield return new WaitForSeconds (0.25f);
		OnHideInfo ();
	}
	public virtual void OnHideInfo() {}

}
