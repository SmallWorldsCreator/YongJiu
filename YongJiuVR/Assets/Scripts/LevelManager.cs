using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : ManagerBase<LevelManager> {
	public E_LEVEL nowLevel = E_LEVEL.None;
	public List<LevelObj> levelObjs;

	public override void Init () {
		foreach (LevelObj _levelObj in levelObjs) {
			if (_levelObj != null) {
				_levelObj.ChangeState (E_LEVEL_STATE.before);
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartLevel (E_LEVEL p_level) {
		Debug.Log ("StartLevel : " + p_level);
		nowLevel = p_level;
		if (nowLevel != E_LEVEL.None) {

			for(int f=0; f<(int)E_LEVEL.Len; f++){
				LevelObj _levelObj = levelObjs [f];
				if(_levelObj != null){
					if (f == (int)nowLevel) {
						_levelObj.ChangeState (E_LEVEL_STATE.inLevel);
					}else if(_levelObj.state == E_LEVEL_STATE.before){
						_levelObj.ChangeState (E_LEVEL_STATE.inOtherLevel);
					}
				}
			}
		}
	}
	public void EndLevel () {
		if (nowLevel != E_LEVEL.None) {
			Debug.Log ("EndLevel : " + nowLevel);
			for(int f=0; f<(int)E_LEVEL.Len; f++){
				LevelObj _levelObj = levelObjs [f];
				if(_levelObj != null){
					if (f == (int)nowLevel) {
						_levelObj.ChangeState (E_LEVEL_STATE.after);
					}else if(_levelObj.state == E_LEVEL_STATE.inOtherLevel){
						_levelObj.ChangeState (E_LEVEL_STATE.before);
					}
				}
			}
			nowLevel = E_LEVEL.None;
		}
	}
}
