using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GvrUIPointer : GvrBasePointer {
	[NullAlarm]public Image centerPoint;
	[NullAlarm]public Image lookRing;

	public float maxReticleDistance = 50.0f;
	public float enterRadius;
	public float exitRadius;
	[LockInInspector]public VrTarget lookObj;

	public float clickDelayTime;
	float lookTime = 0;

	protected override void Start() {
		base.Start();
		lookRing.fillAmount = 0;
		centerPoint.color = Color.white;
	}

	void Update () {
		if(lookObj){
			if(lookTime < clickDelayTime){
				lookRing.fillAmount = lookTime/clickDelayTime;
				lookTime += Time.deltaTime;
				if(lookTime >= clickDelayTime){
					lookObj.OnVrRunEvent();
				}
			}else{
				lookRing.fillAmount = 0;
			}
		}
	}

	void OnDisable () {
		centerPoint.color = Color.white;
		lookRing.fillAmount = 0;
	}

	public override float MaxPointerDistance { get { return maxReticleDistance; } }

	public override void OnPointerEnter(RaycastResult raycastResultResult, bool isInteractive) {
//		Debug.Log("OnPointerEnter");
		lookObj = raycastResultResult.gameObject.GetComponent<VrTarget>();
		if(lookObj != null){
			centerPoint.color = Color.red;
			lookObj.OnVrPointEnter();
		}
		lookTime = 0;
		lookRing.fillAmount = 0;
	}

	public override void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive) {
		
	}

	public override void OnPointerExit(GameObject previousObject) {
//		Debug.Log("OnPointerExit");
		if(lookObj != null){
			lookObj.OnVrPointExit();
		}
		lookObj = null;
		centerPoint.color = Color.white;
		lookRing.fillAmount = 0;
	}

	public override void OnPointerClickDown() {}

	public override void OnPointerClickUp() {}

	public override void GetPointerRadius(out float p_enterRadius, out float p_exitRadius) {
		p_enterRadius = enterRadius;
		p_exitRadius = exitRadius;
	}
}
