using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemKey : VrItem {
	[NullAlarm]public TextAnswer textAnswer;
	public MeshRenderer render;
	[EqualAlarm("")]public string answerString;

	public Material normalMate, selectMate;
	public override void OnVrRunEvent() {
		base.OnVrRunEvent ();
		if (render != null) {
			render.material = selectMate;
		}
		textAnswer.AddString (answerString);

	}
}
