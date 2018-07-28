using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour {
	[NullAlarm]public Animator centerAnime;
	[NullAlarm]public Animator anime;
	[NullAlarm]public Image image;
	[NullAlarm]public Text Text;
	// Use this for initialization
	void Start () {
		anime.Play ("HideUI", -1, 1);
		centerAnime.Play ("ShowCenter", -1, 1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetInfo (Sprite p_sprite, string p_text) {
		if (p_text == "") {
			
		} else {
			image.sprite = p_sprite;
			Text.text = p_text;
			anime.Play ("ShowUI", -1, 0);
		}
	}
}
