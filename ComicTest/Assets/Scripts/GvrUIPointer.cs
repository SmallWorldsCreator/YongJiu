using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GvrUIPointer : GvrBasePointer {
	public float maxReticleDistance = 50.0f;
	public float enterRadius;
	public float exitRadius;
	protected override void Start() {
		base.Start();
		
	}

	void Update () {
		
	}

	public override float MaxPointerDistance { get { return maxReticleDistance; } }

	public override void OnPointerEnter(RaycastResult raycastResultResult, bool isInteractive) {
		Debug.Log("OnPointerEnter");
	}

	public override void OnPointerHover(RaycastResult raycastResultResult, bool isInteractive) {
	}

	public override void OnPointerExit(GameObject previousObject) {
		Debug.Log("OnPointerExit");
	}

	public override void OnPointerClickDown() {}

	public override void OnPointerClickUp() {}

	public override void GetPointerRadius(out float p_enterRadius, out float p_exitRadius) {
		p_enterRadius = enterRadius;
		p_exitRadius = exitRadius;
	}
}
