using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sticker : MonoBehaviour {
	enum E_stickerState{
		init,
		wait,
		up
	}
	E_stickerState state = E_stickerState.init;
	public Cloth cloth;
	public SkinnedMeshRenderer render;

	void Start () {
		ChangeState(E_stickerState.init);
	}

	void ChangeState (E_stickerState p_state) {
		Debug.Log("Sticker [" + name + "] ChangeState : " + p_state.ToString());
		state = p_state;
		switch(state){
		case E_stickerState.init:
			render.enabled = false;
			cloth.externalAcceleration = new Vector3(0, -1000, 0);
			break;
		case E_stickerState.wait:
			render.enabled = true;
			cloth.externalAcceleration = new Vector3(0, -50, 0);
			break;
		case E_stickerState.up:
			render.enabled = true;
			cloth.externalAcceleration = new Vector3(0, 50, 0);
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
