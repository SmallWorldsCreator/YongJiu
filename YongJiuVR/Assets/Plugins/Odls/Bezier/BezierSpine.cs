using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum E_BEZIER_ACTION{None, Click, Move};
public enum E_BEZIER_HANDLE_TYPE{Free, liner, Align, Mirro, Auto, Null};

public class BezierSpine : MonoBehaviour {
//	[HideInInspector]
	public List<BezierPoint> points = new List<BezierPoint>();
//	[HideInInspector]
	public List<BezierLine> lines = new List<BezierLine>();
	[HideInInspector]public int pointCount = 0;
	[HideInInspector]public int selectIndex = -1;
	public float autoStrength = 0.5f;
	public float gizmosSize = 1;

	void Start () {
		
	}

	void Update () {
		
	}

	public void FixInfo () {
//		selectIndex = -1;

		pointCount = points.Count;
		for (int f = 0; f < pointCount; f++) {
			points [f].index = f;
		}

		int _lineCount = lines.Count;
		if (_lineCount < (pointCount - 1)) {
			for (int f = _lineCount; f < (pointCount - 1); f++) {
				lines.Add (new BezierLine());
			}
		}else if (_lineCount > (pointCount - 1)) {
			for (int f = _lineCount-1; f >= (pointCount - 1); f--) {
				if (f >= 0) {
					lines.RemoveAt (f);
				}
			}
		}

		for (int f = 0; f < (pointCount-1); f++) {
			lines [f].index = f;
			lines [f].SetStartPoint (points [f]);
			lines [f].SetEndPoint (points [f+1]);
			lines [f].spine = this;
		}

		if (pointCount > 0) {
			points [0].lastHandle = null;
			points [pointCount - 1].nextHandle = null;
		}
	}

	[Button]public string clearBut = "Clear";
	public void Clear () {
		pointCount = 0;
		points.Clear ();
		lines.Clear ();
	}

	[Button("AddPointAtEnd")]public Vector3 addPointBut = Vector3.zero;
	public void AddPointAtEnd (Vector3 p_pos) {
		AddPoint (p_pos, pointCount);
	}

	public void AddPoint (Vector3 p_pos, int p_index) {
		if (!NumberExtend.isInRange(p_index, 0, pointCount)) {
			Debug.LogErrorFormat (
				"BezierSpine [{0}] length = {1}, can't Add Point at {2}",
				GameObjectExtend.GetFullPath(gameObject),
				pointCount,
				p_index
			);
			return;
		}

		BezierPoint _newPoint = new BezierPoint (p_pos);

		if (pointCount == 0) {
			points.Add (_newPoint);
		}else if (p_index == 0) {
			BezierLine _newLine = new BezierLine (this, _newPoint, points[0]);
			points.Insert (0, _newPoint);
			lines.Insert (0, _newLine);
		}else if (p_index == pointCount) {
			BezierLine _newLine = new BezierLine (this, points[p_index-1], _newPoint);
			points.Add (_newPoint);
			lines.Add (_newLine);
		}else{
			BezierLine _newLine = new BezierLine (this, points[p_index-1], _newPoint);
			lines [p_index - 1].SetStartPoint (_newPoint);
			points.Insert (p_index, _newPoint);
			lines.Insert (p_index-1, _newLine);
		}
		FixInfo ();
#if UNITY_EDITOR
		_newPoint.Draw (this);
#endif
		_newPoint.CheckAuto (this, true);

	}

	public void RemovePoint (int p_index) {
		if (NumberExtend.isInRange (p_index, 0, pointCount - 1)) {
			points.RemoveAt(p_index);
			FixInfo ();
		}
	}

	Dictionary<int, Vector3[]> LocalPossDict = new Dictionary<int, Vector3[]> ();
	bool hasChange = false;
	public Vector3[] GetLocalPoss(int p_count){
		Vector3[] _poss;
		if(!LocalPossDict.TryGetValue(p_count, out _poss)){
			_poss = new Vector3[p_count];
			LocalPossDict.Add (p_count, _poss);
			hasChange = true;
		}

		if (hasChange) {
			float _step = (pointCount-1) / (float)(p_count-1);
			if (_step > 0) {
				for (int f = 0; f < p_count; f++) {
					float _time = _step * (float)f;
					int _lineIndex = Mathf.FloorToInt (_time);
					float _lineTime = _time % 1;

					if (_lineIndex >= (pointCount-1)) {
						_lineIndex = pointCount - 2;
						_lineTime = 1;
					}

					_poss [f] = lines[_lineIndex].GetLocalPos (_lineTime);
				}
			} else {
				for (int f = 0; f < p_count; f++) {
					_poss [f] = Vector3.zero;
				}
			}
			hasChange = false;
		}
		return _poss;
	}

	Dictionary<int, Vector3[]> evenlyPossDict = new Dictionary<int, Vector3[]> ();
	bool hasChangeEvenly = false;
	public Vector3[] GetEvenlyPoss(int p_count){
		Vector3[] _poss;
		if(!evenlyPossDict.TryGetValue(p_count, out _poss)){
			_poss = new Vector3[p_count];
			evenlyPossDict.Add (p_count, _poss);
			hasChangeEvenly = true;
		}

		if (hasChangeEvenly) {
			float _totalDistance = 0;
			foreach (BezierLine _line in lines) {
				_totalDistance += _line.segments [BezierLine.segmentSize - 1].distanceSum;
			}
			float _step = _totalDistance / (float)(p_count - 1);

			if (_step > 0) {
				int _lineIndex = 0;
				int _segmentIndex = 0;
				int _lineCount = pointCount - 1;

				BezierSegment _segment = lines [0].segments [0];
				float _nowDistance = _segment.distance;
				string _log = "";
				for (int f = 0; f < p_count; f++) {
					float _distance = _step * f;
					_log += "pos " + f + " distance " + _distance + "\n  ";
					while (_distance > _nowDistance) {
						_segmentIndex++;

						if (_segmentIndex >= BezierLine.segmentSize) {
							_segmentIndex = 0;
							_lineIndex++;
						}
						if (_lineIndex >= _lineCount) {
							break;
						}
						try{
							_log += "[" + _lineIndex + " - " + _segmentIndex + "] ";
							_segment = lines [_lineIndex].segments [_segmentIndex];
							_nowDistance += _segment.distance;
							_log += " : " + _segment.distance + "/" + _nowDistance + ", ";

						}catch(System.Exception ex){
							Debug.LogError (_log);
							return _poss;
						}

					}

					if (_distance > _nowDistance) {
						_poss [f] = _segment.endPoint;
					} else {
						float _laveDistance = _nowDistance - _distance;
						_poss [f] = _segment.GetPoint (_segment.distance - _laveDistance);
					}
					_log += "\n";
				}
			} else {
				for (int f = 0; f < p_count; f++) {
					_poss [f] = Vector3.zero;
				}
			}

			hasChangeEvenly = false;
		}
		return _poss;
	}

	Dictionary<int, Vector3[]> evenlyLocalPossDict = new Dictionary<int, Vector3[]> ();
	bool hasChangeLocalEvenly = false;
	public Vector3[] GetLocalEvenlyPoss(int p_count){
		Vector3[] _poss;
		if(!evenlyLocalPossDict.TryGetValue(p_count, out _poss)){
			_poss = new Vector3[p_count];
			evenlyLocalPossDict.Add (p_count, _poss);
			hasChangeLocalEvenly = true;
		}

		if (hasChangeLocalEvenly) {
			Vector3[] _evenlyPoss = GetEvenlyPoss(p_count);
			for (int f = 0; f <p_count; f++) {
				_poss [f] = transform.InverseTransformPoint(_evenlyPoss[f]);
			}
			hasChangeLocalEvenly = false;
		}
		return _poss;
	}

	public void ValueChanged(){
//		EditorUtility.SetDirty (this);
		hasChange = true;
		hasChangeEvenly = true;
		hasChangeLocalEvenly = true;
	}
	public void AllLineValueChanged(){
		foreach (BezierLine _line in lines) {
			_line.ValueChanged();
		}
		ValueChanged ();
	}

#if UNITY_EDITOR
	void OnDrawGizmos () {
		if (!enabled) {
			return;
		}

////		Vector3[] _poss = GetEvenlyPoss (19);
//		Vector3[] _poss = GetLocalPoss (19);
//		foreach (Vector3 _pos in _poss) {
////			Gizmos.DrawSphere (_pos, gizmosSize*0.4f);
//			Gizmos.DrawSphere (transform.TransformPoint(_pos), gizmosSize*0.4f);
//		}

		foreach (BezierLine _line in lines) {
			_line.DrawLine();
		}
	}
#endif
}
