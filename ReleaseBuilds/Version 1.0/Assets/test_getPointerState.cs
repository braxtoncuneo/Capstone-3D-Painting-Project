using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class test_getPointerState : MonoBehaviour {
    public VRTK_Pointer testptr;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        bool fooy = testptr.IsPointerActive();
        Debug.Log(fooy);
	}
}
