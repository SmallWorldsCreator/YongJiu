using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class StickerObj : DefaultTrackableEventHandler {
	[NullAlarm]public Sticker sticker;
	public Sprite p_sprite;
	public string p_text;
	// Use this for initialization
	protected override void Start () {
		base.Start ();
		sticker.OnStickerUp += OnStickerUp;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected override void OnTrackingFound(){
		ArManager.instance.infoUi.centerAnime.Play ("HideCenter", -1, 0);
	}

	protected override void OnTrackingLost()	{
		ArManager.instance.infoUi.centerAnime.Play ("ShowCenter", -1, 0);
		sticker.ChangeState (E_stickerState.init);
	}

	public void OnStickerUp(){
		ArManager.instance.StartCoroutine (IeStickerUp());
	}

	IEnumerator IeStickerUp(){
		yield return new WaitForSeconds (1f);
		ArManager.instance.infoUi.SetInfo (p_sprite, p_text);
	}
}
