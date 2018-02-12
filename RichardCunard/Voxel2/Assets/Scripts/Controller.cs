using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public World world;

    public int x, y, z;

	void Start ()
    {
        x = world.chunkSize;
        y = 0;
        z = world.chunkSize;
	}
	
	// Update is called once per frame
	void Update ()
    {
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
