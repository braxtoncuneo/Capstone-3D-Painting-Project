using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public int x, y, z;
	public GameObject controller;

	void Start ()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y - 1;
        z = (int)transform.position.z;
	}
	
	//check for user input
	void Update ()
    {
		x = (int)controller.transform.position.x;
		y = (int)controller.transform.position.y;
		z = (int)controller.transform.position.z;
        //moves position of controller
		if (Input.GetButton("Up"))
        {
            transform.Translate(0, 1, 0);
            y++;
        }
        if (Input.GetButton("Down"))
        {
            transform.Translate(0, -1, 0);
            y--;
        }
        if (Input.GetButton("Left"))
        {
            transform.Translate(-1, -1, 0);
            x--;
            y--;
        }
        if (Input.GetButton("Right"))
        {
            transform.Translate(1, -1, 0);
            x++;
            y--;
        }
        if (Input.GetButton("Back"))
        {
            transform.Translate(0, 1, -1);
            z--;
            y++;
        }
        if (Input.GetButton("Forward"))
        {
            transform.Translate(0, 1, 1);
            z++;
            y++;
        }
    }
}
