using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NineSliceObj : MonoBehaviour {
	public Transform[] subObjs;
	float cornerScale = 1;
	Vector3 scale;
	void Start () {
		scale = transform.localScale;
		RefreshSubObj ();
	}

	void Update () {
		Vector3 _scale = transform.localScale;
		if (!_scale.Equals (scale)) {
			RefreshSubObj ();
		}
	}

	void RefreshSubObj () {
		float L = cornerScale*scale.x*0.25f;
		float R = cornerScale*scale.x*0.25f;
		float B = cornerScale*scale.y*0.25f;
		float T = cornerScale*scale.y*0.25f;
		float MX = scale.x-L-R;
		float MY = scale.y-T-B;

		subObjs [0].localScale = new Vector3 (L, T, 1);
		subObjs [1].localScale = new Vector3 (L, MY, 1);
		subObjs [2].localScale = new Vector3 (L, B, 1);
		subObjs [3].localScale = new Vector3 (MX, T, 1);
		subObjs [4].localScale = new Vector3 (MX, MY, 1);
		subObjs [5].localScale = new Vector3 (MX, B, 1);
		subObjs [6].localScale = new Vector3 (R, T, 1);
		subObjs [7].localScale = new Vector3 (R, MY, 1);
		subObjs [8].localScale = new Vector3 (R, B, 1);

		subObjs [0].localPosition = new Vector3 (L+MX, T+MY, 0)/2;
		subObjs [1].localPosition = new Vector3 (0, T+MY, 0)/2;
		subObjs [2].localPosition = new Vector3 (R+MX, T+MY, 0)/2;
		subObjs [3].localPosition = new Vector3 (L+MX, 0, 0)/2;
		subObjs [4].localPosition = new Vector3 (0, 0, 0)/2;
		subObjs [5].localPosition = new Vector3 (R+MX, 0, 0)/2;
		subObjs [6].localPosition = new Vector3 (L+MX, B+MY, 0)/2;
		subObjs [7].localPosition = new Vector3 (0, B+MY, 0)/2;
		subObjs [8].localPosition = new Vector3 (R+MX, B+MY, 0)/2;
	}
}
