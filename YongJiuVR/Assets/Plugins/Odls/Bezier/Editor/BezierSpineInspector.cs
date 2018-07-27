#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpine))]
public class BezierSpineInspector : Editor {
	BezierSpine spine;
	void OnEnable () {
		spine = (BezierSpine)target;
		spine.FixInfo ();
		Undo.undoRedoPerformed += UndoRedoPerformed;
	}
	void OnDisable () {
		Undo.undoRedoPerformed -= UndoRedoPerformed;
		EditorUtility.SetDirty (spine);
	}
	void UndoRedoPerformed(){
		spine.FixInfo ();
		spine.AllLineValueChanged ();
	}
	public override void OnInspectorGUI(){
		GUI.enabled = false;
		EditorGUILayout.ObjectField ("Script", MonoScript.FromMonoBehaviour(spine), typeof(BezierSpine));
		GUI.enabled = true;

//		base.OnInspectorGUI ();
		EditorGUI.BeginChangeCheck ();
		spine.gizmosSize = EditorGUILayout.FloatField ("gizmosSize", spine.gizmosSize);
		if (EditorGUI.EndChangeCheck ()) {
			SceneView.RepaintAll ();
		}
		float _autoStrength = EditorGUILayout.FloatField ("autoStrength", spine.autoStrength);
		if (spine.autoStrength != _autoStrength) {
			Undo.RegisterCompleteObjectUndo (spine, "Bezier autoStrength Change");
			spine.autoStrength = _autoStrength;
			foreach (BezierPoint _point in spine.points) {
				_point.CheckAuto(spine, false);
			}
			spine.AllLineValueChanged ();
			SceneView.RepaintAll ();
		}

		EditorGUI.BeginChangeCheck ();
		if (spine.pointCount == 0) {
			if (GUILayout.Button ("Add Point")) {
				Undo.RegisterCompleteObjectUndo (spine, "Bezier Add Point");
				spine.AddPoint (Vector3.zero, 0);
				spine.selectIndex = 0;
			}
		}
		if (EditorGUI.EndChangeCheck()) {
			spine.AllLineValueChanged ();
			SceneView.RepaintAll ();
		}

		if (NumberExtend.isInRange (spine.selectIndex, 0, spine.pointCount - 1)) {
			GUILayout.Label ("selectIndex = " + spine.selectIndex);
			BezierPoint _point = spine.points[spine.selectIndex];

			E_BEZIER_HANDLE_TYPE _nowlastType = ((_point.lastHandle!=null) ? _point.lastHandle.DrawType(_point.nextHandle) : E_BEZIER_HANDLE_TYPE.Null);
			E_BEZIER_HANDLE_TYPE _nowNextType = ((_point.nextHandle!=null) ? _point.nextHandle.DrawType(_point.lastHandle) : E_BEZIER_HANDLE_TYPE.Null);

			Vector3 _localPos = EditorGUILayout.Vector3Field("localPos", _point.localPos);
			if (_localPos != _point.localPos) {
				Undo.RegisterCompleteObjectUndo (spine, "Bezier Move Point");
				_point.localPos = _localPos;
				_point.pos = spine.transform.TransformPoint(_localPos);
				_point.CheckAuto (spine, true);
			}

			if (_point.lastHandle != null) {
				_localPos = EditorGUILayout.Vector3Field ("last Handle", _point.lastHandle.localPos);
				if (_localPos != _point.lastHandle.localPos) {
					Undo.RegisterCompleteObjectUndo (spine, "Bezier Move Point");
					_point.lastHandle.localPos = _localPos;
					_point.lastHandle.pos = spine.transform.TransformPoint (_localPos);
					_point.CheckAuto (spine, true);
				}
			}

			if (_point.nextHandle != null) {
				_localPos = EditorGUILayout.Vector3Field ("next Handle", _point.nextHandle.localPos);
				if (_localPos != _point.nextHandle.localPos) {
					Undo.RegisterCompleteObjectUndo (spine, "Bezier Move Point");
					_point.nextHandle.localPos = _localPos;
					_point.nextHandle.pos = spine.transform.TransformPoint (_localPos);
					_point.CheckAuto (spine, true);
				}
			}

			EditorGUI.BeginChangeCheck ();

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Add Befor")) {
				Undo.RegisterCompleteObjectUndo (spine, "Bezier Add Point");
				Vector3 _newPos = Vector3.zero;
				if (_point.lastPoint != null) {
					_newPos = _point.lastHandle.line.GetLocalPos(0.5f);
				} else if (_point.nextPoint != null) {
					_newPos = _point.localPos -_point.nextHandle.localPos;
				} else {
					_newPos = _point.localPos + Vector3.one;
				}
				spine.AddPoint (_newPos, spine.selectIndex);
			}
			if (GUILayout.Button ("Del")) {
				Undo.RegisterCompleteObjectUndo (spine, "Bezier Del Point");
				spine.RemovePoint (spine.selectIndex);
				spine.selectIndex = Mathf.Min(spine.selectIndex, spine.pointCount-1);
			}
			if (GUILayout.Button ("Add After")) {
				Undo.RegisterCompleteObjectUndo (spine, "Bezier Add Point");
				Vector3 _newPos = Vector3.zero;
				if (_point.nextPoint != null) {
					_newPos = _point.nextHandle.line.GetLocalPos(0.5f);
				} else if (_point.lastPoint != null) {
					_newPos = _point.localPos -_point.lastHandle.localPos;
				} else {
					_newPos = _point.localPos + Vector3.one;
				}
				spine.AddPoint (_newPos, spine.selectIndex + 1);
				spine.selectIndex++;
			}
			GUILayout.EndHorizontal ();

			if (EditorGUI.EndChangeCheck()) {
				spine.AllLineValueChanged ();
				SceneView.RepaintAll ();
			}
		}



//		GUILayout.Label ("hotControl = " + GUIUtility.hotControl);
//		foreach (BezierPoint _point in spine.points) {
//			GUILayout.Label (
//				"point = " + _point.index +
//				", id = " + _point.id +
//				", handle = " + _point.handle +
//				", last = " + ((_point.lastHandle == null) ? "No Handle" : ((_point.lastHandle.line == null) ? "Handle No line" : "line " + _point.lastHandle.line.index))+
//				", next = " + ((_point.nextHandle == null) ? "No Handle" : ((_point.nextHandle.line == null) ? "Handle No line" : "line " + _point.nextHandle.line.index))
//			);
//		}
	}
	void OnSceneGUI () {
		if (!spine.enabled) {
			return;
		}

		E_BEZIER_ACTION _action = E_BEZIER_ACTION.None;

		foreach (BezierPoint _point in spine.points) {
			E_BEZIER_ACTION _pointAction = _point.Draw (spine);
			_action = ((_pointAction == E_BEZIER_ACTION.None) ? _action : _pointAction);
		}

		foreach (BezierPoint _point in spine.points) {
			E_BEZIER_ACTION _handleAction = _point.DrawHandle (_point.index == spine.selectIndex);
			_action = ((_handleAction == E_BEZIER_ACTION.None) ? _action : _handleAction);
		}

		if (_action != E_BEZIER_ACTION.None) {
			BezierPoint _point = spine.points[spine.selectIndex];
			BezierLine _lastLine = _point.lastLine;
			if (_lastLine != null) {	_lastLine.ValueChanged();	}
			BezierLine _nextLine = _point.nextLine;
			if (_nextLine != null) {	_nextLine.ValueChanged();	}

			spine.ValueChanged ();
		}
	}
}
#endif