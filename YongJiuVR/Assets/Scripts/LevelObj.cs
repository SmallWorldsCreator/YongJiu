using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_LEVEL{
	None,
	SignBoard,
	SoySauce,
	Machine,
	Chocolate,
	AccounBook,
	Len
}
public enum E_LEVEL_STATE{
	before,
	inLevel,
	after,
	inOtherLevel
}
public class LevelObj : MonoBehaviour {
	[ChangeCall("ChangeState")]public E_LEVEL_STATE state = E_LEVEL_STATE.before;
	public List<VrItem> beforeLevelItems;
	public List<VrItem> inLevelItems;
	public List<VrItem> afterLevelItems;
	public List<VrItem> inOtherLevelItems;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetListActive (List<VrItem> p_list, bool p_active) {
		foreach (VrItem _item in p_list) {
			if (_item != null) {
				_item.gameObject.SetActive (p_active);
			}
		}
	}

	public void ChangeState (E_LEVEL_STATE p_state) {
		Debug.Log ("Level [" + name + "] ChangeState : " + p_state.ToString());
		state = p_state;
		switch (state) {
		case E_LEVEL_STATE.before:
			SetListActive (inLevelItems, false);
			SetListActive (afterLevelItems, false);
			SetListActive (inOtherLevelItems, false);
			SetListActive (beforeLevelItems, true);
			break;
		case E_LEVEL_STATE.inLevel:
			SetListActive (beforeLevelItems, false);
			SetListActive (afterLevelItems, false);
			SetListActive (inOtherLevelItems, false);
			SetListActive (inLevelItems, true);
			break;
		case E_LEVEL_STATE.after:
			SetListActive (beforeLevelItems, false);
			SetListActive (inLevelItems, false);
			SetListActive (inOtherLevelItems, false);
			SetListActive (afterLevelItems, true);
			break;
		case E_LEVEL_STATE.inOtherLevel:
			SetListActive (beforeLevelItems, false);
			SetListActive (inLevelItems, false);
			SetListActive (afterLevelItems, false);
			SetListActive (inOtherLevelItems, true);
			break;
		}
	}
}
