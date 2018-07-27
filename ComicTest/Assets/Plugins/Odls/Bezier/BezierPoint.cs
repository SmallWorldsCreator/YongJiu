using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class BezierPoint{
	public int index = -1;
	public int id = -1;
	public Vector3 localPos = Vector3.zero;
	public Vector3 pos = Vector3.zero;

	public BezierHandle lastHandle = null;
	public BezierHandle nextHandle = null;
//	public BezierLine lastLine = null;
//	public Vector3 lastHandlePos = Vector3.zero;
//	public Vector3 localLastHandlePos = Vector3.zero;
//	public E_BEZIER_HANDLE_TYPE lastHandleType = E_BEZIER_HANDLE_TYPE.Mirro;
//
//	public BezierLine nextLine = null;
//	public Vector3 nextHandlePos = Vector3.zero;
//	public Vector3 localNextHandlePos = Vector3.zero;
//	public E_BEZIER_HANDLE_TYPE nextHandleType = E_BEZIER_HANDLE_TYPE.Mirro;

	public int handle = 0;

	public BezierPoint (Vector3 p_pos) {
		localPos = p_pos;
		id = Random.Range (0, 9999);
	}

	public void CheckAuto (BezierSpine p_spine, bool p_isMove) {
		BezierPoint _lastPoint = lastPoint;
		BezierPoint _nextPoint = nextPoint;

		if ((_lastPoint != null) && (_nextPoint != null)) {
			if (lastHandle.type == E_BEZIER_HANDLE_TYPE.Auto) {
				Vector3 _lastRay = _lastPoint.pos - pos;
				Vector3 _nextRay = _nextPoint.pos - pos;
				float _lastDistance = _lastRay.magnitude;
				float _nextDistance = _nextRay.magnitude;
				float _lerp = _lastDistance / (_lastDistance + _nextDistance);

				Vector3 _localLastRay = _lastPoint.localPos - localPos;
				Vector3 _localNextRay = _nextPoint.localPos - localPos;
				Vector3 _normal = Vector3.Lerp (_localLastRay, -_localNextRay, _lerp).normalized * p_spine.autoStrength;
				lastHandle.localPos =  _normal * _localLastRay.magnitude;
				nextHandle.localPos = -_normal * _localNextRay.magnitude;
			}
			if (p_isMove) {
				_lastPoint.CheckAuto (p_spine, false);
				_nextPoint.CheckAuto (p_spine, false);
			}
		} else if (_lastPoint != null) {
			if (lastHandle.type == E_BEZIER_HANDLE_TYPE.Auto) {
				Vector3 _lastRay = _lastPoint.localPos - localPos;
				lastHandle.localPos = _lastRay * 0.5f;
			}
			if (p_isMove) {
				_lastPoint.CheckAuto (p_spine, false);
			}
		} else if (_nextPoint != null) {
			if (nextHandle.type == E_BEZIER_HANDLE_TYPE.Auto) {
				Vector3 _nextRay = _nextPoint.localPos - localPos;
				nextHandle.localPos = _nextRay * 0.5f;
			}
			if (p_isMove) {
				_nextPoint.CheckAuto (p_spine, false);
			}
		} else {
			if (lastHandle != null) {
				lastHandle.localPos = Vector3.zero;
			}
			if (nextHandle != null) {
				nextHandle.localPos = Vector3.zero;
			}
		}
		BezierLine _lastLine = lastLine;
		if (_lastLine != null) {
			_lastLine.ValueChanged ();
		}
		BezierLine _nextLine = nextLine;
		if (_nextLine != null) {
			_nextLine.ValueChanged ();
		}
	}
	public BezierLine lastLine{
		get{
			if ((lastHandle != null) && (lastHandle.line != null) && (lastHandle.line.index >= 0)) {
				return lastHandle.line;
			} else {
				return null;
			}
		}
	}
	public BezierLine nextLine{
		get{
			if ((nextHandle != null) && (nextHandle.line != null) && (nextHandle.line.index >= 0)) {
				return nextHandle.line;
			} else {
				return null;
			}
		}
	}

	public BezierPoint lastPoint{
		get{
			BezierLine _line = lastLine;
			if (_line != null) {
				return _line.startPoint;
			} else {
				return null;
			}
		}
	}
	public BezierPoint nextPoint{
		get{
			BezierLine _line = nextLine;
			if (_line != null) {
				return _line.endPoint;
			} else {
				return null;
			}
		}
	}
#if UNITY_EDITOR
	public E_BEZIER_ACTION Draw (BezierSpine p_spine) {

		bool _isSelect = (p_spine.selectIndex == index);
		if (_isSelect) {
			Handles.color = Color.yellow;
		}

		E_BEZIER_ACTION _action = BezierPoint.DoFreeMove (out pos, ref localPos, p_spine, Quaternion.identity, p_spine.gizmosSize, BezierPoint.SelectDotHandleCap);
			
		if (_isSelect) {
			Handles.color = Color.white;
		}
		if (_action != E_BEZIER_ACTION.None) {
//			Debug.Log ("selectIndex = " + index + " by " + _action.ToString() + " When " + Event.current.ToString());
			if (p_spine.selectIndex != index) {
				p_spine.selectIndex = index;
				Undo.RegisterCompleteObjectUndo (p_spine, "Bezier Select Point");
//				EditorUtility.SetDirty (p_spine);
			}
		}
		if (_action == E_BEZIER_ACTION.Move) {
			CheckAuto (p_spine, true);
		}
		handle = BezierPoint.selectHandleId;
		BezierPoint.EndSelectHandleCheck();
		return _action;
	}
	public E_BEZIER_ACTION DrawHandle (bool p_isSelect) {
		E_BEZIER_ACTION _lastAction = ((lastHandle==null) ? E_BEZIER_ACTION.None : lastHandle.Draw(pos, localPos, nextHandle, p_isSelect));
		if (_lastAction != E_BEZIER_ACTION.None) {
			BezierHandle.FixOtherHandle (lastHandle, nextHandle);
		}

		E_BEZIER_ACTION _nextAction = ((nextHandle==null) ? E_BEZIER_ACTION.None : nextHandle.Draw(pos, localPos, lastHandle, p_isSelect));
		if (_nextAction != E_BEZIER_ACTION.None) {
			BezierHandle.FixOtherHandle (nextHandle, lastHandle);
		}

		return ((_lastAction==E_BEZIER_ACTION.None) ? _nextAction : _lastAction);
	}

	public static E_BEZIER_ACTION DoFreeMove(out Vector3 p_pos, ref Vector3 p_localPos, BezierSpine p_spine, Quaternion p_rotation, float p_size, Handles.CapFunction p_capFunction){
		E_BEZIER_ACTION _action = E_BEZIER_ACTION.None;

		p_pos = p_spine.transform.TransformPoint (p_localPos);

		EditorGUI.BeginChangeCheck();
		Vector3 _nowPos = Handles.FreeMoveHandle (p_pos, p_rotation, p_size, Vector3.one, p_capFunction);

		if (EditorGUI.EndChangeCheck ()) {
			Undo.RegisterCompleteObjectUndo (p_spine, "Bezier Move Point");

			p_pos = _nowPos;
			p_localPos = p_spine.transform.InverseTransformPoint(_nowPos);
			_action = E_BEZIER_ACTION.Move;
		}else if (GUIUtility.hotControl == selectHandleId) {			
			if (Event.current.type != EventType.Repaint) {
				_action = E_BEZIER_ACTION.Click;
			}
		}
		return _action;
	}

	static int selectHandleId = -1;
	static int nowHandleId = -1;
	public static void SelectDotHandleCap(int p_handleId, Vector3 p_position, Quaternion p_rotation, float p_size, EventType p_eventType){
		selectHandleId = p_handleId;
		nowHandleId = p_handleId;
		Handles.DotHandleCap(p_handleId, p_position, p_rotation, p_size, p_eventType);
	}
	public static void EndSelectHandleCheck(){
		selectHandleId = -1;
	}
	public static void SphereHandleCap(int p_handleId, Vector3 p_position, Quaternion p_rotation, float p_size, EventType p_eventType){
		nowHandleId = p_handleId;
		Handles.SphereHandleCap(p_handleId, p_position, p_rotation, p_size, p_eventType);
	}
#endif
}
