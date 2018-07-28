using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_stickerState{
	init,
	wait,
	up
}
public class Sticker : MonoBehaviour {

	E_stickerState state = E_stickerState.init;
	public Cloth cloth;
	public SkinnedMeshRenderer render;

	void Start () {
		Vector3 _pos = transform.position;
		_pos.y = 0.1444574f;
		transform.position = _pos;
//
//		Vector3 _scale = transform.lossyScale;
//		_scale.y = 0.4589705f / transform.parent.lossyScale.y;
//		transform.localScale = _scale;

		ChangeState(E_stickerState.init);
	}
	public System.Action OnStickerUp;
	public void ChangeState (E_stickerState p_state) {
		Debug.Log("Sticker [" + transform.parent.name + "] ChangeState : " + p_state.ToString() + " pos " + transform.position.y + " scale " + transform.lossyScale.y);
		state = p_state;
		switch(state){
		case E_stickerState.init:
			render.material.SetColor ("_Color", new Color(1,1,1,0));
			cloth.externalAcceleration = new Vector3(0, -1000, 0);
			break;
		case E_stickerState.wait:
			render.material.SetColor ("_Color", new Color(1,1,1,1));
			cloth.externalAcceleration = new Vector3(0, -50, 0);
			break;
		case E_stickerState.up:
			render.material.SetColor ("_Color", new Color (1, 1, 1, 1));
			cloth.externalAcceleration = new Vector3 (0, 50, 0);
			if (OnStickerUp != null) {
				OnStickerUp.Invoke ();
			}
			break;
		}
	}

	void Update () {
		switch(state){
		case E_stickerState.init:	InitUpdate();		break;
		case E_stickerState.wait:	WaitUpdate();		break;
		case E_stickerState.up:		UpUpdate();			break;
		}
	}

	void InitUpdate () {
		Bounds _bounds = render.bounds;
		if((_bounds.center.y + (_bounds.size.y/2f)) <= 0.01f){
			ChangeState(E_stickerState.wait);
		}
	}

	void WaitUpdate () {
		Bounds _bounds = render.bounds;
		if((_bounds.center.y - (_bounds.size.y/2f)) >  0.01f){
			ChangeState(E_stickerState.up);
		}
	}

	void UpUpdate () {

	}
}
