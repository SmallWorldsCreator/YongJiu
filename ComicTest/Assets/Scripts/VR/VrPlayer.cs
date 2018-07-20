using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VrPlayer : MonoBehaviour {
	Vector3 targetPos;
	[LockInInspector]public bool isMoving;
	public float moveSpeed;
	// Use this for initialization
	void Start () {
		targetPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		isMoving = ((transform.position - targetPos).sqrMagnitude > 0.0001f);
		if(isMoving){
			transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed*Time.deltaTime);
		}else{
			transform.position = targetPos;
		}
	}

	public void SetTargetPos (Vector3 p_pos) {
		targetPos = p_pos;
	}
}
