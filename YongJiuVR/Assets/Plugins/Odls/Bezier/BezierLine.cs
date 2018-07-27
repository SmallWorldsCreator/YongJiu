using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class BezierLine {
	public const int segmentSize = 10;

	public int index = -1;
	[HideInInspector]public BezierSpine mSpine = null;
	[HideInInspector]public BezierPoint mStartPoint = null;
	[HideInInspector]public BezierPoint mEndPoint = null;
	public BezierSegment[] segments = new BezierSegment[segmentSize];

	public BezierSpine spine{
		get{	return mSpine;	}
		set{	mSpine = value;	}
	}

	public BezierPoint startPoint{
		get{	return mStartPoint;	}
		set{	mStartPoint = value;	}
	}

	public BezierPoint endPoint{
		get{	return mEndPoint;	}
		set{	mEndPoint = value;	}
	}

	public BezierLine(){
		for (int f = 0; f <segmentSize; f++) {
			segments [f] = new BezierSegment(0, Vector3.zero, Vector3.zero);
		}
	}

	public BezierLine(BezierSpine p_spine, BezierPoint p_startPoint, BezierPoint p_endPoint){
		spine = p_spine;
		SetStartPoint (p_startPoint);
		SetEndPoint (p_endPoint);
		for (int f = 0; f <segmentSize; f++) {
			segments [f] = new BezierSegment(0, Vector3.zero, Vector3.zero);
		}
	}
	public void SetStartPoint(BezierPoint p_startPoint){
		startPoint = p_startPoint;
		if (startPoint != null) {
			if (startPoint.nextHandle == null) {
				startPoint.nextHandle = new BezierHandle ();
			}
			startPoint.nextHandle.line = this;
		} else {
			startPoint.nextHandle = null;
		}
	}
	public void SetEndPoint(BezierPoint p_endPoint){
		endPoint = p_endPoint;
		if (startPoint != null) {
			if (endPoint.lastHandle == null) {
				endPoint.lastHandle = new BezierHandle ();
			}
			endPoint.lastHandle.line = this;
		} else {
			startPoint.lastHandle = null;
		}
	}

	public Vector3 GetLocalPos(float p_time){
		float _u = 1f - p_time;
		float _t2 = p_time * p_time;
		float _u2 = _u * _u;
		float _u3 = _u2 * _u;
		float _t3 = _t2 * p_time;

		Vector3 _pos =
			(_u3) * startPoint.localPos +
			(3f * _u2 * p_time) * (startPoint.localPos + startPoint.nextHandle.localPos) +
			(3f * _u * _t2) *  (endPoint.localPos + endPoint.lastHandle.localPos)+
			(_t3) * endPoint.localPos;
		return _pos;
	}

	Dictionary<int, Vector3[]> localPossDict = new Dictionary<int, Vector3[]> ();
	bool hasChangeLocal = false;
	public Vector3[] GetLocalPoss(int p_count){
		Vector3[] _poss;
		if(!localPossDict.TryGetValue(p_count, out _poss)){
			_poss = new Vector3[p_count];
			localPossDict.Add (p_count, _poss);
			hasChangeLocal = true;
		}

		if (hasChangeLocal) {
			float _step = 1f / (float)(p_count-1);
			for (int f = 0; f <p_count; f++) {
				_poss [f] = GetLocalPos (_step*(float)f);
			}
			hasChangeLocal = false;
		}
		return _poss;
	}

	Dictionary<int, Vector3[]> possDict = new Dictionary<int, Vector3[]> ();
	bool hasChange = false;
	public Vector3[] GetPoss(int p_count){
		Vector3[] _poss;
		if(!possDict.TryGetValue(p_count, out _poss)){
			_poss = new Vector3[p_count];
			possDict.Add (p_count, _poss);
			hasChange = true;
		}

		if (hasChange) {
			Vector3[] _localPoss = GetLocalPoss(p_count);
			for (int f = 0; f <p_count; f++) {
				_poss [f] = spine.transform.TransformPoint(_localPoss[f]);
			}
			hasChange = false;
		}
		return _poss;
	}
	public void ValueChanged(){
		hasChangeLocal = true;
		hasChange = true;
		FixSegment ();
	}

	public void FixSegment(){
		float _distanceSum = 0;
		Vector3[] _poss = GetPoss(segmentSize+1);

		for (int f = 0; f <segmentSize; f++) {
			segments [f].SetPoint (_distanceSum, _poss[f], _poss[f+1]);
			_distanceSum += segments [f].distance; 
		}
	}

#if UNITY_EDITOR
	public void DrawLine () {
		if ((startPoint != null) && (endPoint != null)) {
			Handles.DrawBezier (
				startPoint.pos,
				endPoint.pos,
				startPoint.nextHandle.pos,
				endPoint.lastHandle.pos,
				Color.white,
				null,
				2
			);
		}

//		Gizmos.color = Color.blue;
//		for (int f = 0; f <segmentSize; f++) {
//			Gizmos.DrawLine (segments [f].startPoint, segments [f].endPoint);
//		}
//		Gizmos.color = Color.white;
	}
#endif

}
