using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class TextAnswer : MonoBehaviour {
	[NullAlarm]public VrItemAnswer answerItem;
	[NullAlarm]public TextMesh text;
	[NullAlarm]public VrItemKey[] clearTargets;

	public bool anyOrder;
	[LessAlarm(0)]public int stringCount;
	int nowStringCount = 0;
	[EqualAlarm("")]public string answer = "";
	[EqualAlarm("")]public string failInfo = "";
	[LockInInspector]public string nowString = "";
	// Use this for initialization
	void Start () {
		Clear ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void Clear () {
		nowString = "";
		text.text = "";
		nowStringCount = 0;

		foreach (VrItemKey _clearTarget in clearTargets) {
			_clearTarget.render.material = _clearTarget.normalMate;
		}
	}
	public void AddString (string p_string) {
		if (p_string == "Clear") {
			Clear ();
			return;
		}

		if (nowStringCount == 0) {
			nowString = p_string;
		} else {
			nowString += " " + p_string;
		}
		text.text = nowString;
		nowStringCount++;
		if (nowStringCount >= stringCount) {
			CheckAnswer ();
		}
	}
	void CheckAnswer () {
		if (anyOrder) {
			string _noOrderAnswer = SortString (answer);
			string _noOrderNowAnswer = SortString (nowString);
			if (_noOrderAnswer == _noOrderNowAnswer) {
				answerItem.ShowContent ();
				return;
			}
		} else {
			if (answer == nowString) {
				answerItem.ShowContent ();
				return;
			}
		}

		Clear ();
		text.text = failInfo;
	}

	static string SortString(string p_input){
		char[] _chars = p_input.ToArray();
		System.Array.Sort(_chars);
		return new string(_chars);
	}
}
