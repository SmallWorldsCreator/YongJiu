using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_VR_PLAYER_STATE{
	idle,
	move
}
public class VrPlayerManager : ManagerBase<VrPlayerManager> {
	E_VR_PLAYER_STATE state;
	[NullAlarm]public VrPlayer player;
	[NullAlarm]public GvrUIPointer pointer;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		switch(state){
		case E_VR_PLAYER_STATE.idle:	IdleUpdate();	break;
		case E_VR_PLAYER_STATE.move:	MoveUpdate();	break;
		}
	}

	void IdleUpdate () {
	}

	void MoveUpdate () {
		if(!player.isMoving){
			ChangeState(E_VR_PLAYER_STATE.idle);
		}
	}

	void ChangeState (E_VR_PLAYER_STATE p_state) {
		if(state != p_state){
			state = p_state;
			switch(state){
			case E_VR_PLAYER_STATE.idle:
				pointer.enabled = true;
				VrTargetManager.instance.OpenTargetAtFloorPoint (nowFloorPoint);
				break;
			case E_VR_PLAYER_STATE.move:
				pointer.enabled = false;
				VrTargetManager.instance.CloseAllTarget ();
				break;
			}
		}
	}

	FloorPoint nowFloorPoint;
	public void SetFloorPoint(FloorPoint p_floorPoint){
		nowFloorPoint = p_floorPoint;
		player.SetTargetPos(nowFloorPoint.transform.position);
		ChangeState(E_VR_PLAYER_STATE.move);
	}
}
