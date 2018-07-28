using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrItemCanPick : VrItem {
	[NullAlarm]public TextAnswer textAnswer;
	[EqualAlarm("")]public string answerString;
	public override void OnContentDone() {
		base.OnContentDone ();
		textAnswer.AddString (answerString);
		gameObject.SetActive (false);
	}
}
