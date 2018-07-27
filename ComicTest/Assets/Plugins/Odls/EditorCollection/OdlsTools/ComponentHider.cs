using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentHider{
	public static void UnhideAllComponent(GameObject p_obj){
		Component[] _components = p_obj.GetComponents<Component>();
        foreach(Component _comp in _components) {
			UnhideComponent (_comp);
        }
    }
	public static void HideAllComponent(GameObject p_obj){
		Component[] _components = p_obj.GetComponents<Component>();
		foreach(Component _comp in _components) {
			HideComponent (_comp);
		}
	}

	public static void UnhideComponent(Component p_comp){
		p_comp.hideFlags = HideFlags.None;
	}

	public static void HideComponent(Component p_comp){
		if (p_comp) {
			p_comp.hideFlags = HideFlags.HideInInspector;
		}
	}
}
