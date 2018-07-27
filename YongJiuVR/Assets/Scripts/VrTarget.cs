using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrTarget : MonoBehaviour {
	public Collider collider;

	public virtual void OnVrPointEnter() {}
	public virtual void OnVrPointExit() {}
	public virtual void OnVrRunEvent() {}
	public virtual void OnOpenTarget() {}
	public virtual void OnCloseTarget() {}

	public void OpenTarget() {
		collider.enabled = true;
		OnOpenTarget ();
	}
	public void CloseTarget() {
		collider.enabled = false;
		OnCloseTarget ();
	}
}
