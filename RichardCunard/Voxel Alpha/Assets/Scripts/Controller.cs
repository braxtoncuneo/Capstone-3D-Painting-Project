using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public int x, y, z;
    public float scale;

	void Start ()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y - 1;
        z = (int)transform.position.z;
        transform.localScale = new Vector3(scale, scale, scale);
	}
	
	//check for user input
	void Update ()
    {
        //moves position of controller
		if (Input.GetButton("Up"))
        {
            transform.Translate(0, scale, 0);
            y++;
        }
        if (Input.GetButton("Down"))
        {
            transform.Translate(0, -scale, 0);
            y--;
        }
        if (Input.GetButton("Left"))
        {
            transform.Translate(-scale, -scale, 0);
            x--;
            y--;
        }
        if (Input.GetButton("Right"))
        {
            transform.Translate(scale, -scale, 0);
            x++;
            y--;
        }
        if (Input.GetButton("Back"))
        {
            transform.Translate(0, scale, -scale);
            z--;
            y++;
        }
        if (Input.GetButton("Forward"))
        {
            transform.Translate(0, scale, scale);
            z++;
            y++;
        }
    }
}
