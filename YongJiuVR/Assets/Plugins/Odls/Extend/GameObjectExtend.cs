//#define log
#define IALED
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class GameObjectExtend{
#if log
	static string log = "";
#endif

#region "Hierarchy"
	public static string GetFullPath(GameObject p_obj) {
		List<string> _pathList = new List<string>();
		
		Transform _nowTrans = p_obj.transform;
		_pathList.Add(_nowTrans.name);
		
		while (_nowTrans.parent != null) {
			_pathList.Insert(0, _nowTrans.parent.name);
			_nowTrans = _nowTrans.parent;
		}
		
		return string.Join("/", _pathList.ToArray());
	}
#endregion

#region "Transform"
	public static void SetLossyScale (Transform p_tran, Vector3 p_lossyScale) {
		Transform _parentTrans = p_tran.parent;
		if(_parentTrans == null){
			p_tran.localScale = p_lossyScale;
		}else{
			Vector3 _parentScale = _parentTrans.lossyScale;
			p_tran.localScale = new Vector3((_parentScale.x == 0 ? 0 : p_lossyScale.x / _parentScale.x),
			                                (_parentScale.y == 0 ? 0 : p_lossyScale.y / _parentScale.y),
			                                (_parentScale.z == 0 ? 0 : p_lossyScale.z / _parentScale.z));
		}
	}
#endregion

#region "Child"
	public static void DelAllChild(GameObject p_obj) {
		Transform[] _childs = p_obj.transform.GetComponentsInChildren<Transform>();
		int f;
		int len = _childs.Length;
		for (f=1; f<len; f++) {
			GameObject.Destroy(_childs[f].gameObject);
		}
	}
	public static void AddChild(GameObject p_obj, GameObject p_child){
		p_child.transform.SetParent (p_obj.transform);
	}
	public static void AddChildAndResetTransform(GameObject p_obj, GameObject p_child){
		AddChild(p_obj,p_child,Vector3.zero,Quaternion.identity,Vector3.one);
	}
	public static void AddChild(GameObject p_obj, GameObject p_child, Vector3 p_pos, Vector3 p_rota, Vector3 p_scale) {
		AddChild (p_obj, p_child, p_pos, Quaternion.Euler(p_rota), p_scale);
	}
	public static void AddChild(GameObject p_obj, GameObject p_child, Vector3 p_pos, Quaternion p_rota, Vector3 p_scale) {
		p_child.transform.SetParent (p_obj.transform);
		if (p_pos != null) {
			p_child.transform.localPosition = p_pos;
		}
		if (p_rota != null) {
			p_child.transform.localRotation = p_rota;
		}
		if (p_scale != null) {
			p_child.transform.localScale = p_scale;
		}
	}
#endregion

#region "Component"
	public static T GetComponentInParent<T>(Transform p_tran) where T : Component{
		Transform _parent = p_tran;
		while(true){
			_parent = _parent.transform.parent;
			if (_parent == null) {
				break;
			}
			T _parentComponent = _parent.GetComponent<T> ();
			if (_parentComponent) {
				return _parentComponent;
			}
		}
		return null;
	}
	public static T  GetComponentInScene<T>() where T : Component{
		GameObject[] _obj = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().GetRootGameObjects ();
		T[] _components;
		
		int f,len2;
		int len = _obj.Length;
		for(f=0; f<len; f++){
			_components = _obj[f].transform.GetComponentsInChildren<T>(true);
			len2 = _components.Length;

			if(len2 <= 0){
				continue;
			}else{
				return _components[0];
			}
		}
		return null;
	}
	public static List<T> GetComponentsInScene<T>() where T : Component{
		GameObject[] _obj = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().GetRootGameObjects ();
		T[] _components;
		List<T> _list = new List<T> ();
		
		int f,f2,len2;
		int len = _obj.Length;
		for(f=0; f<len; f++){
			_components = _obj[f].transform.GetComponentsInChildren<T>(true);
			len2 = _components.Length;
			for(f2=0; f2<len2; f2++){
				_list.Add(_components[f2]);
			}
		}
		
		return _list;
	}
	public static List<Transform> GetChildList(GameObject p_obj){
		return GetChildList (p_obj.transform);
	}
	public static List<Transform> GetChildList(Transform p_trans){
		int _childLen = p_trans.childCount;
		List<Transform> _list = new List<Transform> ();
		for (int f = 0; f < _childLen; f++) {
			_list.Add (p_trans.GetChild(f));
		}
		return _list;
	}

//	static BindingFlags fieldFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
	static BindingFlags fieldFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default;
#if IALED
	public delegate GameObject GetGameObjectWithPrefab(GameObject p_obj);
	public static void CopyGameObjectValue(GameObject p_targetGameObject, GameObject p_sourceGameObject, GetGameObjectWithPrefab p_getGameObjectWithPrefab , bool p_includeChild = false, bool p_delOther = false){
#else
	public static void CopyGameObjectValue(GameObject p_targetGameObject, GameObject p_sourceGameObject, bool p_includeChild = false){
#endif
		p_targetGameObject.name = p_sourceGameObject.name + "_Copy";
#if log
		log = "";
#endif
#if IALED
		DoCopyGameObjectValue(p_targetGameObject, p_sourceGameObject, p_getGameObjectWithPrefab, p_includeChild, p_delOther);
#else
		DoCopyGameObjectValue(p_targetGameObject, p_sourceGameObject, p_includeChild, p_delOther);
#endif
#if log
		Debug.Log (log);
#endif


		string _targetPath = GetFullPath (p_targetGameObject); 
		string _sourcePath = GetFullPath (p_sourceGameObject);
#if log
		log = "";
		try{
#endif
			DoFixGameObjectSelfReference (p_targetGameObject, _targetPath, _sourcePath, p_includeChild);
#if log
		}catch(System.Exception e){
			Debug.LogError (e.Message + "\n" + e.StackTrace + "\n---------\n");

		}
		Debug.Log (log);
		GUIUtility.systemCopyBuffer = log;
#endif
		p_targetGameObject.name = p_sourceGameObject.name;
	}

#if IALED
	static void DoCopyGameObjectValue(GameObject p_targetGameObject, GameObject p_sourceGameObject, GetGameObjectWithPrefab p_getGameObjectWithPrefab, bool p_includeChild = false, bool p_delOther = false){
#else
	static void DoCopyGameObjectValue(GameObject p_targetGameObject, GameObject p_sourceGameObject, bool p_includeChild = false, bool p_delOther = false){
#endif
#if log
		log += "CopyGameObject " + p_targetGameObject.name + "\n";
#endif
		Component[] _sourceComponents = p_sourceGameObject.GetComponents<Component>();
		List<Component> _targetComponents = new List<Component>( p_targetGameObject.GetComponents<Component>() );
		int _targetLen = _targetComponents.Count;
		foreach (Component _sourceComponent in _sourceComponents) {
			for(int f=0; f<_targetLen; f++) {
				Component _targetComponent = _targetComponents[f];
				if (_sourceComponent.GetType().Name == _targetComponent.GetType().Name) {
					CopyComponentValue(_targetComponent, _sourceComponent);
					_targetComponents.RemoveAt (f);
					_targetLen--;
					break;
				}
			}
		}

		if (p_includeChild) {
			List<Transform> _sourceChilds = GetChildList(p_sourceGameObject);
			List<Transform> _targetChilds = GetChildList(p_targetGameObject);
			_targetLen = _targetChilds.Count;
			foreach (Transform _sourceChild in _sourceChilds) {
				Transform _targetChild = null;
				for(int f=0; f<_targetLen; f++) {
					if (_sourceChild.name == _targetChilds[f].name) {
						_targetChild = _targetChilds[f];
						_targetChilds.RemoveAt (f);
						_targetLen--;
						break;
					}
				}
				if (_targetChild == null) {
#if IALED
					if(p_getGameObjectWithPrefab == null){
						_targetChild = GameObject.Instantiate (_sourceChild.gameObject).transform;
					}else{
						_targetChild = p_getGameObjectWithPrefab(_sourceChild.gameObject).transform;
					}
#else
					_targetChild = GameObject.Instantiate (_sourceChild.gameObject).transform;
#endif
					_targetChild.name = _sourceChild.name;
					_targetChild.SetParent (p_targetGameObject.transform);
				}
#if IALED
				DoCopyGameObjectValue(_targetChild.gameObject, _sourceChild.gameObject, p_getGameObjectWithPrefab, true, p_delOther);
#else
				DoCopyGameObjectValue(_targetChild.gameObject, _sourceChild.gameObject, true, p_delOther);
#endif
			}

			if(p_delOther){
				foreach (Transform _targetChild in _targetChilds) {
					if(_targetChild != null){
						GameObject.DestroyImmediate(_targetChild.gameObject);
					}
				}
			}
		}
	}

	static void DoFixGameObjectSelfReference(GameObject p_targetGameObject, string p_targetPath, string p_sourcePath, bool p_includeChild = false){
#if log
		log += "FixGameObject " + p_targetGameObject.name + "\n";
#endif
		List<Component> _targetComponents = new List<Component>( p_targetGameObject.GetComponents<Component>() );
		int _targetLen = _targetComponents.Count;
		for(int f=0; f<_targetLen; f++) {
			Component _targetComponent = _targetComponents[f];
			FixComponentSelfReference(_targetComponent, p_targetPath, p_sourcePath);
		}

		if (p_includeChild) {
			List<Transform> _targetChilds = GetChildList(p_targetGameObject);
			_targetLen = _targetChilds.Count;

			for(int f=0; f<_targetLen; f++) {
				DoFixGameObjectSelfReference(_targetChilds[f].gameObject, p_targetPath, p_sourcePath, true);
			}
		}
	}

	public static void CopyComponentValue(Component p_targetComponent, Component p_sourceComponent){
#if UNITY_EDITOR
		UnityEditorInternal.ComponentUtility.CopyComponent(p_sourceComponent);
		UnityEditorInternal.ComponentUtility.PasteComponentValues (p_targetComponent);
#endif
	}
	public static void FixComponentSelfReference(Component p_targetComponent, string p_targetPath, string p_sourcePath){
#if log
		log += "  FixComponent " + p_targetComponent.GetType().Name + "\n";
#endif

		System.Type _type = p_targetComponent.GetType();

		FieldInfo[] _fields = _type.GetFields(fieldFlags);
		foreach(FieldInfo _field in _fields){
			FixFieldSelfReference (_field, p_targetComponent, p_targetPath, p_sourcePath);
		}

	}
	static void FixObjMemberSelfReference(object p_value, string p_targetPath, string p_sourcePath){
		System.Type _type = p_value.GetType();
		FieldInfo[] _fields = _type.GetFields(fieldFlags);
		foreach(FieldInfo _field in _fields){
			FixFieldSelfReference (_field, p_value, p_targetPath, p_sourcePath);
		}
	}
	static void FixFieldSelfReference(FieldInfo p_field, object p_targetObject, string p_targetPath, string p_sourcePath){
		object _value = p_field.GetValue (p_targetObject);
		_value = FixObjSelfReference (p_field.Name, _value, p_targetPath, p_sourcePath);
		if (_value != null) {
			p_field.SetValue (p_targetObject, _value);
		}
	}
	static object FixObjSelfReference(string p_objName, object _value, string p_targetPath, string p_sourcePath){
		string _path;

		if ((_value==null) || (_value.ToString() == "null")) {
#if log
			log += "    FixField " + p_objName + " (Not Need [Null]) \n";
#endif
			return null;
		}

		string _typeName = _value.GetType().Name;
			
		Component _component = _value as Component;
		if (_component != null) {
			_path = GetFullPath (_component.gameObject);
			if (_path.Contains (p_sourcePath)) {
				_path = _path.Replace (p_sourcePath, p_targetPath);
				GameObject _newObj = GameObject.Find (_path);
				if (_newObj) {
					Component _newComponent = _newObj.GetComponent (_component.GetType ());
					if (_newComponent) {
#if log
						log += "    FixField " + p_objName + " (Component SetValue [" + _path + "]) \n";
#endif
//						FixObjMemberSelfReference (_value, p_targetPath, p_sourcePath);
						return _newComponent;
					}
				}
			}
#if log
			log += "    FixField " + p_objName + " (Component Not Self [" + _path + "]) \n";
#endif
			return null;
		}

		GameObject _gameObject = _value as GameObject;
		if (_gameObject != null) {
			_path = GetFullPath (_gameObject);
			if (_path.Contains (p_sourcePath)) {
				_path = _path.Replace (p_sourcePath, p_targetPath);
				GameObject _newObj = GameObject.Find (_path);
				if (_newObj) {
#if log
					log += "    FixField " + p_objName + " (GameObject SetValue [" + _path + "]) \n";
#endif
//					FixObjMemberSelfReference (_value, p_targetPath, p_sourcePath);
					return _newObj;
				}
			}
#if log
			log += "    FixField " + p_objName + " (GameObject Not Self [" + _path + "]) \n";
#endif
			return null;
		}

		Object _object = _value as Object;
		if (_object != null) {
#if log
			log += "    FixField " + p_objName + " (Not Need [" + _typeName + "]) \n";
#endif
			return null;
		}

		IList _list = _value as IList;
		if (_list != null) {
#if log
			log += "    FixField " + p_objName + " (Check List [" + _typeName + "]) \n";
#endif
			int len = _list.Count;
			for (int f=0; f<len; f++) {
				object _subValue = _list [f];
				_subValue = FixObjSelfReference (p_objName + "[" + f + "]", _subValue, p_targetPath, p_sourcePath);
				if (_subValue != null) {
					_list [f] = _subValue;
				}
			}
			return _list;
		}

#if log
		log += "    FixField " + p_objName + " (Not Need [" + _typeName + "]) \n";
#endif
		FixObjMemberSelfReference (_value, p_targetPath, p_sourcePath);
		return null;
	}
#endregion

#region "Shader"
	static Dictionary<string,Shader> shaderDict = new Dictionary<string, Shader> ();
	public static void ReloadShader(GameObject p_obj){
		string _log = "";

		if(p_obj == null){
			Debug.LogError("ReloadShader Failed : No GameObject");
			return;
		}

		try{
			Renderer[] _renders = p_obj.GetComponentsInChildren<Renderer> (true);
			Material[] _materials;
			int f,f2;
			int len = _renders.Length;

			if (len <= 0) {
				Debug.LogError("No Material In GameObject : " + p_obj.name);
				return;
			}

			int len2;
			Shader _shader;
			_log += "Reload Shader : " + p_obj.name;
			for (f=0; f<len; f++) {
				_materials = _renders[f].sharedMaterials;

				len2 = _materials.Length;
				for (f2=0; f2<len2; f2++) {
					if(_materials[f2].shader == null){
						_log += "\n  Set Shader " + _renders[f].gameObject.name + " material " + f2.ToString() +
							StringExtend.RichColor(" Failed",Color.green) + "\n     Shader is Null";
					}else{
						string _name = _materials[f2].shader.name;

						_log += "\n  Set Shader [" + _name + "] to " + _renders[f].gameObject.name + " material " + f2.ToString();

						if(!shaderDict.TryGetValue(_name, out _shader)){
							_shader = Shader.Find(_name);
							if(_shader == null){
								_log += StringExtend.RichColor(" Failed",Color.green) + "\n     Can't Find Shader : [" + _name + "]";
								continue;
							}
						}

						_materials[f2].shader = _shader;
						_log += StringExtend.RichColor(" OK",Color.green);
					}
				}
			}
			Debug.Log (_log);
		}catch(System.Exception e){
			_log += StringExtend.RichColor("\nException : " + e.Message,Color.red);
			Debug.LogError (_log);
		}
	}
#endregion

#region "Editor"

	public static Vector2 GetViewSize(){
#if UNITY_EDITOR
		System.Type _typeofGameView = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo _GetSizeMethod = _typeofGameView.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object _result = _GetSizeMethod.Invoke(null,null);
		return (Vector2)_result;
#else
		return new Vector2(Screen.width,Screen.height);
#endif
	}
#endregion
}
