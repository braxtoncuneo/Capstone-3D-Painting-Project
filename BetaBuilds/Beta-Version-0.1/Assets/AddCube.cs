using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;


public class AddCube : MonoBehaviour {

	public 

	// Use this for initialization
	void Start () {
		if (GetComponent<VRTK_ControllerEvents>() == null) {
			Debug.LogError("ColorWheel is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
			return;
		}

		//blackWheel = transform.Find ("CanvasHolder/Canvas/BlackWheel").gameObject;

		//GetComponent<VRTK_ControllerEvents>().OnTriggerClicked += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
