using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : ManagerBase<LevelManager> {
	public int nowLevelMask = 0;
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

	bool LevelIsRun(E_LEVEL p_level){
		return (((1 << (int)p_level) & nowLevelMask) != 0);
	}

	public void StartLevel (E_LEVEL p_level) {
		Debug.Log ("StartLevel : " + p_level.ToString());
		nowLevelMask |= (1<<(int)p_level);
		if (p_level != E_LEVEL.None) {

			LevelObj _levelObj = levelObjs [(int)p_level];
			if(_levelObj != null){
				_levelObj.ChangeState (E_LEVEL_STATE.inLevel);
			}
		}
	}
	public void EndLevel (E_LEVEL p_level) {
		if (LevelIsRun(p_level)) {
			Debug.Log ("EndLevel : " + p_level.ToString());
			LevelObj _levelObj = levelObjs [(int)p_level];
			if(_levelObj != null){
				_levelObj.ChangeState (E_LEVEL_STATE.after);
			}
			nowLevelMask &= ~(1<<(int)p_level);
		}
	}
}
