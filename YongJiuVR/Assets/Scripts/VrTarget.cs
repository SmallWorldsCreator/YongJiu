using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrTarget : MonoBehaviour {
	public Collider collider;
	public VrTarget[] subTargets;

	public virtual void OnVrPointEnter() {}
	public virtual void OnVrPointExit() {}
	public virtual void OnVrRunEvent() {}
	public virtual void OnOpenTarget() {}
	public virtual void OnCloseTarget() {}

	public void OpenTarget() {
		if (collider != null) {
			collider.enabled = true;
		}
		OnOpenTarget ();
		foreach (VrTarget _target in subTargets) {
			_target.OpenTarget ();
		}
	}
	public void CloseTarget() {
		if (collider != null) {
			collider.enabled = false;
		}
		OnCloseTarget ();
		foreach (VrTarget _target in subTargets) {
			_target.CloseTarget ();
		}
	}
}
