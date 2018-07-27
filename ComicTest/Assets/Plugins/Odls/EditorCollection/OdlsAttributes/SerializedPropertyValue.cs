#if UNITY_EDITOR
using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public static class SerializedPropertyValue {
	public static object GetValue(SerializedProperty p_property){
//		Debug.Log("Get : " + p_Property.propertyType.ToString() + " isArray:" + p_Property.isArray.ToString());
		if (p_property.isArray) {
			if(p_property.propertyType == SerializedPropertyType.String){
				return p_property.stringValue;
			}else{
				return GetArray<object>(p_property);
			}
		} else {
			switch (p_property.propertyType) {
			case SerializedPropertyType.Integer:
				return p_property.intValue;
			case SerializedPropertyType.Boolean:
				return p_property.boolValue;
			case SerializedPropertyType.Float:
				return p_property.floatValue;
			case SerializedPropertyType.String:
				return p_property.stringValue;
			case SerializedPropertyType.Color:
				return p_property.colorValue;
			case SerializedPropertyType.Gradient:
				return GetValue<Gradient>(p_property, "gradientValue");
			case SerializedPropertyType.ObjectReference:
				return p_property.objectReferenceValue;
			case SerializedPropertyType.LayerMask:
				return p_property.intValue;
			case SerializedPropertyType.Enum:
				return p_property.enumValueIndex;
			case SerializedPropertyType.Vector2:
				return p_property.vector2Value;
			case SerializedPropertyType.Vector3:
				return p_property.vector3Value;
			case SerializedPropertyType.Rect:
				return p_property.rectValue;
			case SerializedPropertyType.ArraySize:
				SerializedProperty _parent = SerializedPropertyValue.GetParent(p_property);
				return _parent.arraySize;
			case SerializedPropertyType.Character:
				return (char)p_property.intValue;
			case SerializedPropertyType.AnimationCurve:
				return p_property.animationCurveValue;
			case SerializedPropertyType.Bounds:
				return p_property.boundsValue;
			case SerializedPropertyType.Quaternion:
				return p_property.quaternionValue;
			case SerializedPropertyType.Generic:
				return p_property.Copy();
			default:
				Debug.LogError ("Not support this type : " + p_property.propertyType.ToString());
				return null;
			}
		}
	}

	public static void SetValue(SerializedProperty p_targetProperty,object p_sourceObj){
		if (p_targetProperty.isArray) {
			if (p_targetProperty.propertyType == SerializedPropertyType.String){
				p_targetProperty.stringValue = (string)p_sourceObj;
			}else{
				object[] _targetArr = (object[])p_sourceObj;
				if(_targetArr != null){
					int f;
					int len = _targetArr.Length;
					p_targetProperty.ClearArray();
					for(f=0; f<len; f++){
						p_targetProperty.InsertArrayElementAtIndex(f);
						SerializedProperty _targetProperty = p_targetProperty.GetArrayElementAtIndex(f);
						SetValue(_targetProperty,_targetArr[f]);
					}
				}else{
					Debug.LogError ("SetValue Type Wrong");
				}
			}
		} else {
			try{
				switch (p_targetProperty.propertyType) {
				case SerializedPropertyType.Integer:
					p_targetProperty.intValue = (int)p_sourceObj;
					break;
				case SerializedPropertyType.String:
					p_targetProperty.stringValue = (string)p_sourceObj;
					break;
				case SerializedPropertyType.Boolean:
					p_targetProperty.boolValue = (bool)p_sourceObj;
					break;
				case SerializedPropertyType.Float:
					p_targetProperty.floatValue = (float)p_sourceObj;
					break;
				case SerializedPropertyType.Enum:
					p_targetProperty.enumValueIndex = (int)p_sourceObj;
					break;
				case SerializedPropertyType.LayerMask:
					p_targetProperty.intValue = (int)p_sourceObj;
					break;
				case SerializedPropertyType.ObjectReference:
					p_targetProperty.objectReferenceValue = (Object)p_sourceObj;
					break;
				case SerializedPropertyType.Color:
					p_targetProperty.colorValue = (Color)p_sourceObj;
					break;
				case SerializedPropertyType.Gradient:
					SetValue<Gradient>(p_targetProperty, "gradientValue", (Gradient)p_sourceObj);
					break;
				case SerializedPropertyType.AnimationCurve:
					p_targetProperty.animationCurveValue = (AnimationCurve)p_sourceObj;
					break;
				case SerializedPropertyType.Rect:
					p_targetProperty.rectValue = (Rect)p_sourceObj;
					break;
				case SerializedPropertyType.Bounds:
					p_targetProperty.boundsValue = (Bounds)p_sourceObj;
					break;		
				case SerializedPropertyType.Vector2:
					p_targetProperty.vector2Value = (Vector2)p_sourceObj;
					break;
				case SerializedPropertyType.Vector3:
					p_targetProperty.vector3Value = (Vector3)p_sourceObj;
					break;
				case SerializedPropertyType.Vector4:
					p_targetProperty.vector4Value = (Vector4)p_sourceObj;
					break;
				case SerializedPropertyType.ArraySize:
					SerializedProperty _parent = SerializedPropertyValue.GetParent(p_targetProperty);
					_parent.arraySize = (int)p_sourceObj;
					break;
				case SerializedPropertyType.Generic:
					SerializedProperty _sourceProperty = (SerializedProperty)p_sourceObj;
					SetValueByProperty(p_targetProperty, _sourceProperty.Copy());
					break;
				default:
					Debug.LogError ("SetValue Not support this type : " + p_targetProperty.propertyType.ToString());
					break;
				}
			}catch(System.Exception e){
				Debug.LogErrorFormat ("[CopyPaste] Type Wrong, sourceObj is {1}, targetProperty is {0}\n{2}",
				                      p_targetProperty.propertyType,
				                      p_sourceObj.GetType().Name,
				                      e.Message);
			}
		}
	}
	static T GetValue<T>(SerializedProperty p_targetProperty, string p_name)where T : class{
		BindingFlags _instanceAnyPrivacyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		PropertyInfo _propertyInfo = typeof(SerializedProperty).GetProperty(
			p_name,
			_instanceAnyPrivacyBindingFlags,
			null,
			typeof(T),
			new System.Type[0],
			null
			);
		if (_propertyInfo == null){
			Debug.LogError("propertyInfo Is Null : " + p_name);
			return null;
		}
		
		T _value = _propertyInfo.GetValue(p_targetProperty, null) as T;
		return _value;
	}

	static void SetValue<T>(SerializedProperty p_targetProperty, string p_name, T p_value){
		BindingFlags _instanceAnyPrivacyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		PropertyInfo _propertyInfo = typeof(SerializedProperty).GetProperty(
			p_name,
			_instanceAnyPrivacyBindingFlags,
			null,
			typeof(T),
			new System.Type[0],
			null
			);
		if (_propertyInfo == null)
			return;
		_propertyInfo.SetValue(p_targetProperty, p_value, null);
	}

	static void SetValueByProperty(SerializedProperty p_targetProperty, SerializedProperty p_sourceProperty){
//		Debug.Log("SetProperty : " +  p_targetProperty.name + " = " + p_sourceProperty.name);
		try{
			if(p_targetProperty.name != p_sourceProperty.name){
				Debug.LogError("Not Match : " +  p_targetProperty.name + " = " + p_sourceProperty.name);
				return;
			}

			if(p_targetProperty.propertyType == SerializedPropertyType.Generic){
				IEnumerator _targetEnumerator = p_targetProperty.GetEnumerator();
				IEnumerator _sourceEnumerator = p_sourceProperty.GetEnumerator();

				while(_targetEnumerator.MoveNext() && _sourceEnumerator.MoveNext()){
					p_targetProperty = (SerializedProperty)_targetEnumerator.Current;
					p_sourceProperty = (SerializedProperty)_sourceEnumerator.Current;
					if(p_targetProperty.propertyType == SerializedPropertyType.ArraySize){
//						Debug.Log("SetProperty : " +  p_targetProperty.name + " = " + p_sourceProperty.name + "(Skip ArraySize)");
					}else{
						SetValueByProperty(p_targetProperty, p_sourceProperty);
//						Debug.Log("SetProperty : " +  p_targetProperty.name + " = " + p_sourceProperty.name);
					}
				}
			}else{
				object _value = GetValue(p_sourceProperty);
				SetValue(p_targetProperty,_value);
//				Debug.Log("SetValue : " + p_targetProperty.name + " = " + p_sourceProperty.name + " : " + _value.ToString());
			}

//			Object _parentObject = p_sourceProperty.serializedObject.targetObject;
//			System.Type _parentType = _parentObject.GetType();
//			FieldInfo _propertyInfo = _parentType.GetField(p_sourceProperty.propertyPath);  
//			object _obj = _propertyInfo.GetValue(_parentObject);
//			_propertyInfo.SetValue(p_targetProperty, _obj);
		}catch(System.Exception e){
			Debug.LogException(e);
//			Debug.LogErrorFormat ("SetValueByProperty Failed, sourceObj is {1}, targetProperty is {0}\n{2}",
//			                      p_targetProperty.propertyType,
//			                      p_sourceProperty.propertyType,
//			                      e.Message);
		}
	}
	//
	//	public static SerializedPropertyType GetPropertyType<T>(){
//		switch (typeof(T).Name) {
//		case typeof(int).Name:
//
//			break;
//		}
//		return (SerializedPropertyType)-1;
//	}
	public static SerializedProperty GetParent(SerializedProperty p_property){
		string _path = p_property.propertyPath;
		int _last = _path.LastIndexOf (".Array.data[");
		_path = _path.Substring (0,_last);

		SerializedProperty _parent = p_property.serializedObject.FindProperty(_path);

		return _parent;
	}
	public static T[] GetArray<T>(SerializedProperty p_property){
		if (p_property.isArray) {
			int f;
			int len = p_property.arraySize;
			List<T> _list = new List<T>();
			for(f=0; f<len; f++){
				SerializedProperty _itemProperty = p_property.GetArrayElementAtIndex(f);
				try{
					T _item;
					_item = (T)GetValue(_itemProperty);
					_list.Add(_item);
				}catch{
					return null;
				}
			}
			return _list.ToArray();
		}
		return null;
	}
	public static bool isNumber(SerializedProperty p_Property){
		switch (p_Property.propertyType) {
		case SerializedPropertyType.Integer:	return true;
		case SerializedPropertyType.Float:		return true;
		case SerializedPropertyType.LayerMask:	return true;
		case SerializedPropertyType.Enum:		return true;
		default:								return false;
		}
	}

	public static float GetPropertyHeight (SerializedProperty p_property, GUIContent p_label){
		float _height = EditorGUIUtility.singleLineHeight;
		if(p_property.isExpanded){
			IEnumerator _enumerator = p_property.GetEnumerator();
			SerializedProperty _nowProperty;
			string _hideProperty = "";

			while (_enumerator.MoveNext()){
				_nowProperty = (SerializedProperty)_enumerator.Current;

//				if((_nowProperty.propertyType == SerializedPropertyType.String) && (_nowProperty.stringValue == "@@@")){
//					Debug.Log("");
//				}

				string _nowPath = _nowProperty.propertyPath;

				if((_hideProperty != "") && _nowPath.Contains(_hideProperty)){
					continue;
				}else{
					_hideProperty = "";
				}


				if(_nowProperty.isExpanded){
					_hideProperty = "";
					_height += EditorGUI.GetPropertyHeight(_nowProperty, GUIContent.none, false) + 2;
				}else{
					_hideProperty = _nowPath + ".";
					_height += EditorGUI.GetPropertyHeight(_nowProperty, GUIContent.none, true) + 2;
				}
			}
		}
		return _height;
	}
	public static void DrawCopyPasteBehind (Rect p_position, SerializedProperty p_property) {
		p_position.x += p_position.width;
		p_position.height = 18;
		p_position.width = 120;
		DrawCopyPaste(p_position, p_property);
	}
	public static void DrawCopyPaste (Rect p_position, SerializedProperty p_property) {
		p_position.width = p_position.width / 2;
		if (GUI.Button (p_position, "Copy")) {
			CopyPasteAttribute.sourceObj = SerializedPropertyValue.GetValue(p_property);
			CopyPasteAttribute.isArray = false;
		}
		p_position.x += p_position.width;
		if((CopyPasteAttribute.sourceObj != null) && (!CopyPasteAttribute.isArray)){
			if (GUI.Button (p_position, "Paste")) {
				SerializedPropertyValue.SetValue(p_property,CopyPasteAttribute.sourceObj);
			}
		}else{
			GUI.enabled = false;
			GUI.Button (p_position, "Paste");
			GUI.enabled = true;
		}
	}
	public static void DrawCopyPasteArray (Rect p_position, SerializedProperty p_property) {
		p_position.width = p_position.width / 2;
		if (GUI.Button (p_position, "Copy All")) {
			CopyPasteAttribute.sourceObj = SerializedPropertyValue.GetValue(p_property);
			CopyPasteAttribute.isArray = true;
		}
		p_position.x += p_position.width;
		if((CopyPasteAttribute.sourceObj != null) && (CopyPasteAttribute.isArray)){
			if (GUI.Button (p_position, "Paste All")) {
				SerializedPropertyValue.SetValue(p_property,CopyPasteAttribute.sourceObj);
			}
		}else{
			GUI.enabled = false;
			GUI.Button (p_position, "Paste All");
			GUI.enabled = true;
		}
	}
}
#endif