using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    public Vector3 center;
    public float radius;
    Camera cam;
	// Use this for initialization
	void Start () {
        center = new Vector3(1f, 1f, 1f);
        radius = 3.0f;
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3((float)Math.Cos(Time.time),0.0f, (float)Math.Sin(Time.time)) * radius + center;
        transform.LookAt(center);
	}
}
