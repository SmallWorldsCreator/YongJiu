using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

[DisallowMultipleComponent]
public class ManagerBase<T> : InitableBehaviour where T:ManagerBase<T>{
	public virtual void Awake(){
		if (INSTANCE == null) {
			INSTANCE = (T)this;
			GameObject.DontDestroyOnLoad (gameObject);
		} else if (INSTANCE.gameObject.GetInstanceID () == gameObject.GetInstanceID ()) {
			GameObject.DontDestroyOnLoad (gameObject);
		} else {
			Destroy(gameObject);
		}
	}
	static T INSTANCE;
	public static T instance{
		get{
			if(hasInstanceInScene){
				return INSTANCE;
			}else{
				INSTANCE = new GameObject(typeof(T).ToString()).AddComponent<T>();
				GameObject _managerTop = GameObject.Find("Manager");
				if(!_managerTop){
					_managerTop = new GameObject("Manager");
				}
				INSTANCE.transform.SetParent(_managerTop.transform);
				INSTANCE.transform.position = Vector3.zero;
				INSTANCE.transform.rotation = Quaternion.identity;
				INSTANCE.transform.localScale = Vector3.one;
				return INSTANCE;
			}
		}
	}
	public static bool hasInstance{
		get{
			return (INSTANCE != null);
		}
	}

	public static bool hasInstanceInScene{
		get{
			if (hasInstance) {
				return true;
			}
			INSTANCE = GameObject.FindObjectOfType<T>();
			return (INSTANCE != null);
		}
	}

	[Button]public string SetAsInstanceBut = "SetAsInstance";
	public void SetAsInstance(){
		if(INSTANCE && (INSTANCE.GetInstanceID() != this.GetInstanceID())){
			DestroyImmediate(INSTANCE.gameObject);
		}

		INSTANCE = (T)this;

		string _name = typeof(T).ToString();
		_name = Regex.Replace(_name, @"([a-z])([A-Z])", @"$1 $2");

		INSTANCE.gameObject.name = _name;

		GameObject _managerTop = GameObject.Find("Manager");
		if(!_managerTop){
			_managerTop = new GameObject("Manager");
		}
		INSTANCE.transform.SetParent(_managerTop.transform);
	}
}

