#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExtendGUILayout{
	#region "GUI Layout"
	public static Rect GetLayoutRect(int p_width = 30, int p_height = 16){
		Rect _rect = EditorGUILayout.GetControlRect(GUILayout.Width(-4));
		_rect.x -= 2;
		_rect.width = p_width;
		_rect.height = p_height;
		return _rect;
	} 
	static GUIStyle _colorStyle = null; 
	public static void LayoutColorLabel(string p_text, Color p_color, params GUILayoutOption[] p_options){
		if (_colorStyle == null) {
			_colorStyle = new GUIStyle (EditorStyles.largeLabel);
			_colorStyle.normal = new GUIStyleState();
		}
		GUIStyleState _styleState = _colorStyle.normal;
		_styleState.textColor = p_color;
		_colorStyle.normal = _styleState;
		GUILayout.Label (p_text, _colorStyle, p_options);
	}

	static bool lastExpand = false;
	static Color pageButColor = new Color (0.76f, 0.76f, 0.9f, 1);
	static Color pageNowColor = new Color (0.5f, 0.5f, 0.6f, 1);
	public static bool DrawPage(string p_pageName, ref string p_nowPage, ref string p_lastPage){
//		Debug.Log (Event.current.type.ToString() + ", page : " + p_pageName + ", now : " + p_nowPage + ", last : " + p_lastPage);
		if (p_pageName == "Base") {
			return true;
		}else if (p_lastPage == p_pageName) {
			return lastExpand;
		}

		if ((p_lastPage == "") || (p_lastPage == p_nowPage)) {
			GUILayout.Space (8);
		}

		p_lastPage = p_pageName;

		bool _expand;
		if (p_nowPage != p_pageName) {
			GUI.color = pageButColor;
			if (GUILayout.Button (p_pageName)) {
				p_nowPage = p_pageName;
			}
			GUI.color = Color.white;
			_expand = false;
		}else {
			GUILayout.Space (8);
			GUI.color = pageNowColor;
			if (GUILayout.Button (p_pageName)) {
				p_nowPage = "";
			}
			GUI.color = Color.white;
			_expand = true;
		}
		lastExpand = _expand;
		return _expand;
	}

	public static SerializedProperty LayoutPropertyField(string p_name, GUIContent p_label, SerializedObject p_serializedObject, params GUILayoutOption[] p_options){
		SerializedProperty _property = LayoutProperty (p_name, p_label, p_serializedObject, p_options);
		if (_property != null) {
			EditorGUILayout.PropertyField (_property, p_label, p_options);
		}
		return _property;
	}

	public static SerializedProperty LayoutPropertySlider(string p_name, GUIContent p_label, SerializedObject p_serializedObject, params GUILayoutOption[] p_options){
		SerializedProperty _property = LayoutProperty (p_name, p_label, p_serializedObject, p_options);
		if (_property != null) {
			EditorGUILayout.Slider (_property, 0f, 1f, p_label, p_options);
		}
		return _property;
	}

	static SerializedProperty LayoutProperty(string p_name, GUIContent p_label, SerializedObject p_serializedObject, params GUILayoutOption[] p_options){
		SerializedProperty _property = p_serializedObject.FindProperty (p_name);
		if (_property != null) {
			return _property;
		} else {
			LayoutColorLabel (p_name + " No Found", Color.red, p_options);
			return null;
		}
	}
	#endregion

#region "Array"
	public delegate T DrawObjectGate<T>(T p_obj);
	static T[] DrawArray<T>(T[] p_array,DrawObjectGate<T> p_draw,ref bool p_isExpanded){
		GUILayout.BeginVertical ();

		if (p_array == null) {
			p_array = new T[0];
		}
		int f;
		int len = p_array.Length;

		GUILayout.BeginHorizontal ();
		p_isExpanded = GUILayout.Toggle (p_isExpanded,"",GUILayout.Width(30));
		GUILayout.Label ("Size : " + len);
		GUILayout.EndHorizontal ();

		if (p_isExpanded) {
			if (len <= 0) {
				if (GUILayout.Button ("+")) {
					p_array = new T[1];
					len++;
				}
			} else {
				for (f=0; f<len; f++) {
					GUILayout.BeginHorizontal ();
					GUILayout.Label (f.ToString (), GUILayout.Width (30));

					p_array [f] = p_draw (p_array [f]);

					if (GUILayout.Button ("+", GUILayout.Width (20))) {
						List<T> _list = new List<T> (p_array);
						_list.Insert (f + 1, default(T));
						p_array = _list.ToArray ();
						len++;
					}
					if (GUILayout.Button ("-", GUILayout.Width (20))) {
						List<T> _list = new List<T> (p_array);
						_list.RemoveAt (f);
						p_array = _list.ToArray ();
						len--;
					}
					GUILayout.EndHorizontal ();
				}
			}
		}

		GUILayout.EndVertical ();

		return p_array;
	}

	static public T[] ObjectArray<T>(T[] p_array,ref bool p_isExpanded) where T : Object{
		return DrawArray<T> (p_array,DrawObject,ref p_isExpanded);
	}
	static T DrawObject<T>(T p_obj) where T : Object{
		p_obj = (T)EditorGUILayout.ObjectField(p_obj,typeof(T));
		return p_obj;
	}

	//Int
	static public int[] IntArray(int[] p_array,ref bool p_isExpanded){
		return DrawArray<int> (p_array,DrawInt,ref p_isExpanded);
	}
	static int DrawInt(int p_int){
		p_int = EditorGUILayout.IntField(p_int);
		return p_int;
	}

	//String
	static public string[] StringArray(string[] p_array,ref bool p_isExpanded){
		return DrawArray<string> (p_array,DrawString,ref p_isExpanded);
	}
	static string DrawString(string p_string){
		p_string = EditorGUILayout.TextField(p_string);
		return p_string;
	}

	//Bool
	static public bool[] BoolArray(bool[] p_array,ref bool p_isExpanded){
		return DrawArray<bool> (p_array,DrawBool,ref p_isExpanded);
	}
	static bool DrawBool(bool p_bool){
		p_bool = EditorGUILayout.Toggle(p_bool);
		return p_bool;
	}

	//Color
	static public Color[] ColorArray(Color[] p_array,ref bool p_isExpanded){
		return DrawArray<Color> (p_array,DrawColor,ref p_isExpanded);
	}
	static Color DrawColor(Color p_color){
		p_color = EditorGUILayout.ColorField(p_color);
		return p_color;
	}
#endregion
}
#endif
