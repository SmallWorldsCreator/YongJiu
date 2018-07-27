using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItem : VrTarget {
	static VrItem nowLookItem;
	[NullAlarm]public Animator Anime;
	// Use this for initialization
	void Start () {
		Anime.Play ("HideInfo", -1, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnVrRunEvent() {
		Show ();
	}

	public void OnBecameInvisible(){
		Hide ();
	}

	public void Show(){
		if (nowLookItem != null) {
			nowLookItem.Hide ();
		}
		nowLookItem = this;
		Anime.Play ("ShowInfo", -1, 0);
	}

	public void Hide(){
		nowLookItem = null;
		Anime.Play ("HideInfo", -1, 0);
	}
}
