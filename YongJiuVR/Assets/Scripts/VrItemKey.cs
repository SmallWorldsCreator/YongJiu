using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemKey : VrItem {
	[NullAlarm]public TextAnswer textAnswer;
	[EqualAlarm("")]public string answerString;
	public override void OnVrRunEvent() {
		base.OnVrRunEvent ();
		textAnswer.AddString (answerString);
	}
}
