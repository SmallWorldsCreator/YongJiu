using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragBall : MonoBehaviour {
	public Camera cmr;
	float fixY;
	// Use this for initialization
	void Start () {
		fixY = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){
			
			Vector3 _mousePos = Input.mousePosition;
			_mousePos.z = cmr.transform.position.y;
			Vector3 _pos = cmr.ScreenToWorldPoint(_mousePos);
			if(transform.position.y < -0.5f){
				_pos.y = -0.5f;
//				Debug.Log("Set " + _pos.y);
			}else if(transform.position.y < fixY){
				_pos.y = Mathf.MoveTowards(transform.position.y, fixY, Time.deltaTime);
//				Debug.Log("MoveTowards " + _pos.y);
			}else{
				_pos.y = fixY;
//				Debug.Log("fixY " + _pos.y);
			}
			transform.position = _pos;
		}else{
			if(transform.position.y > -50){
				transform.Translate(Vector3.down*Time.deltaTime);
			}
		}

	}
}
