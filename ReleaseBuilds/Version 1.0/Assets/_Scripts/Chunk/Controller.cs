using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;


public class Controller : MonoBehaviour
{
    public int x, y, z;
    public float scale;
    public SteamVR_TrackedObject tracked;
    public SteamVR_Controller.Device device;

    void Start ()
    {
        tracked = GetComponent<SteamVR_TrackedObject>();
        x = (int)transform.position.x;
        y = (int)transform.position.y - 1;
        z = (int)transform.position.z;
        transform.localScale = new Vector3(scale, scale, scale);
    }
	
	//check for user input
	void Update ()
    {

        /*device = SteamVR_Controller.Input((int)tracked.index);
        Debug.Log("First");

        if (device.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger))
        {
            Debug.Log("Test");
            device.TriggerHapticPulse(700);
        }


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
        }*/
    }
    private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
    {
        Debug.Log("Controller on index '" + index + "' " + button + " has been " + action
                + " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        DebugLogger(e.controllerIndex, "TRIGGER", "pressed", e);
    }



}
