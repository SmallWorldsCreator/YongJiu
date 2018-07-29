using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoCunTop : MonoBehaviour {
	public static ChoCunTop instance;
	public GameObject[] ChoCuns;

	void Start () {
		instance = this;
		ChangeChoCun (0);
	}

	void Update () {
		
	}
	public void RandomChangeChoCun () {
		ChangeChoCun (Random.Range(0, ChoCuns.Length));
	}
	public void ChangeChoCun (int p_index) {
		foreach (GameObject _obj in ChoCuns) {
			_obj.gameObject.SetActive (false);
		}
		ChoCuns[p_index].SetActive (true);
	}

}
