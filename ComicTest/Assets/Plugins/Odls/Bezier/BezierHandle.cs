using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class BezierHandle {
	static string[] handleTypeStr = {"Free", "liner", "Align", "Mirro", "Auto"};

	[HideInInspector]public BezierLine mLine = null;
	public Vector3 pos = Vector3.zero;
	public Vector3 localPos = Vector3.zero;
	public E_BEZIER_HANDLE_TYPE type = E_BEZIER_HANDLE_TYPE.Mirro;

	public BezierLine line{
		get{	return mLine;	}
		set{	mLine = value;	}
	}

	public void SetType(E_BEZIER_HANDLE_TYPE p_type, BezierHandle p_otherHandle){
		type = p_type;

		switch (type) {
		case E_BEZIER_HANDLE_TYPE.Free:
			switch (p_otherHandle.type) {
			case E_BEZIER_HANDLE_TYPE.Align:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Free;		break;
			case E_BEZIER_HANDLE_TYPE.Mirro:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Free;		break;
			case E_BEZIER_HANDLE_TYPE.Auto:		p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Free;		break;
			}
			break;
		case E_BEZIER_HANDLE_TYPE.liner:
			localPos = Vector3.zero;
			switch (p_otherHandle.type) {
			case E_BEZIER_HANDLE_TYPE.Align:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Free;		break;
			case E_BEZIER_HANDLE_TYPE.Mirro:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Free;		break;
			case E_BEZIER_HANDLE_TYPE.Auto:		p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Free;		break;
			}
			break;
		case E_BEZIER_HANDLE_TYPE.Align:
			switch (p_otherHandle.type) {
			case E_BEZIER_HANDLE_TYPE.Free:		p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Align;	break;
			case E_BEZIER_HANDLE_TYPE.liner:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Align;	break;
			case E_BEZIER_HANDLE_TYPE.Mirro:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Align;	break;
			case E_BEZIER_HANDLE_TYPE.Auto:		p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Align;	break;
			}
			break;
		case E_BEZIER_HANDLE_TYPE.Mirro:
			switch (p_otherHandle.type) {
			case E_BEZIER_HANDLE_TYPE.Free:		p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Mirro;	break;
			case E_BEZIER_HANDLE_TYPE.liner:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Mirro;	break;
			case E_BEZIER_HANDLE_TYPE.Align:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Mirro;	break;
			case E_BEZIER_HANDLE_TYPE.Auto:		p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Mirro;	break;
			}
			p_otherHandle.localPos = -localPos;
			break;
		case E_BEZIER_HANDLE_TYPE.Auto:
			switch (p_otherHandle.type) {
			case E_BEZIER_HANDLE_TYPE.Free:		p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Auto;		break;
			case E_BEZIER_HANDLE_TYPE.liner:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Auto;		break;
			case E_BEZIER_HANDLE_TYPE.Align:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Auto;		break;
			case E_BEZIER_HANDLE_TYPE.Mirro:	p_otherHandle.type = E_BEZIER_HANDLE_TYPE.Auto;		break;
			}
			line.startPoint.CheckAuto(line.spine, true);
			line.endPoint.CheckAuto(line.spine, true);
			break;
		}

		FixOtherHandle (this, p_otherHandle);
#if UNITY_EDITOR
		EditorUtility.SetDirty (line.spine);
#endif
	}

	public static void FixOtherHandle (BezierHandle p_nowHandle, BezierHandle p_targetHandle) {
		switch (p_nowHandle.type) {
		case E_BEZIER_HANDLE_TYPE.Mirro:
			p_targetHandle.localPos = -p_nowHandle.localPos;
			break;
		case E_BEZIER_HANDLE_TYPE.Align:
			p_targetHandle.localPos = -p_nowHandle.localPos.normalized * p_targetHandle.localPos.magnitude;
			break;
		}
	}

#if UNITY_EDITOR
	public E_BEZIER_ACTION Draw (Vector3 p_pointPos, Vector3 p_pointLocalPos, BezierHandle p_otherHandle, bool p_isSelect) {
		E_BEZIER_ACTION _action = E_BEZIER_ACTION.None;

		if ((line != null) && (line.index >= 0)) {
			switch (type) {
			case E_BEZIER_HANDLE_TYPE.Free:		Handles.color = Color.yellow;	break;
			case E_BEZIER_HANDLE_TYPE.liner:	Handles.color = Color.white;	break;
			case E_BEZIER_HANDLE_TYPE.Align:	Handles.color = Color.green;	break;
			case E_BEZIER_HANDLE_TYPE.Mirro:	Handles.color = Color.cyan;		break;
			case E_BEZIER_HANDLE_TYPE.Auto:		Handles.color = Color.gray;		break;
			}

			Vector3 _nowLocalPos = p_pointLocalPos + localPos;
			if (p_isSelect) {
				switch (type) {
				case E_BEZIER_HANDLE_TYPE.liner:
					pos = line.spine.transform.TransformPoint (_nowLocalPos);
					break;
				case E_BEZIER_HANDLE_TYPE.Auto:
					pos = line.spine.transform.TransformPoint (_nowLocalPos);
					Handles.DrawLine (p_pointPos, pos);
					break;
				default:
					_action = BezierPoint.DoFreeMove (out pos, ref _nowLocalPos, line.spine, Quaternion.identity, line.spine.gizmosSize*2, BezierPoint.SphereHandleCap);
					if (_action != E_BEZIER_ACTION.None) {
						localPos = _nowLocalPos - p_pointLocalPos;
					}
					Handles.DrawLine (p_pointPos, pos);
					break;
				}
			} else { 
				pos = line.spine.transform.TransformPoint (_nowLocalPos);
			}
			Handles.color = Color.white;
		}

		return _action;
	}

	public E_BEZIER_HANDLE_TYPE DrawType (BezierHandle p_otherHandle) {
		if ((line != null) && (line.index >= 0)) {
			E_BEZIER_HANDLE_TYPE _nowType = (E_BEZIER_HANDLE_TYPE)GUILayout.Toolbar ((int)type, handleTypeStr);
			if (_nowType != type) {
				SetType (_nowType, p_otherHandle);

				return _nowType;
			} else {
				return E_BEZIER_HANDLE_TYPE.Null;
			}
		} else {
			GUI.enabled = false;
			GUILayout.Toolbar (-1, handleTypeStr);
			GUI.enabled = true;
			return E_BEZIER_HANDLE_TYPE.Null;
		}
	}
#endif
}
