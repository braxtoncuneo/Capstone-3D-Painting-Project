using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushStyleSelection : MonoBehaviour {

    public float stylex;
    public float styley;
    public float stylez;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale.Set(60 + stylex, 60 + styley, 60 + stylez);
    }

    public void StyleXchanged( float value)
    {
        this.stylex = value;
    }

    public void StyleYchanged(float value)
    {
        this.styley = value;
    }

    public void StyleZchanged(float value)
    {
        this.stylez = value;
    }
}
