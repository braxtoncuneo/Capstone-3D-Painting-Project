using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputs : MonoBehaviour {
    public FollowCameraObj cube0;
    public FollowCameraObj cube1;
    public FollowCameraObj cube2;
    public FollowCameraObj cube3;
    public FollowCameraObj cube4;

    public GameObject cube0_panel;
    public GameObject cube1_panel;
    public GameObject cube2_panel;
    public GameObject cube3_panel;
    public GameObject cube4_panel;

    public bool isUp = false;
    public int selectedPos = 2;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(isUp)
        {
            cube0.SetHeight(0.07f);
            cube1.SetHeight(0.12f);
            cube2.SetHeight(0.15f);
            cube3.SetHeight(0.12f);
            cube4.SetHeight(0.07f);

            if(Input.GetKeyDown("q"))
            {
                selectedPos -= 1;
            }
            if(Input.GetKeyDown("e"))
            {
                selectedPos += 1;
            }
            if (Input.GetKeyDown("c"))
                isUp = false;

            if (selectedPos > 4)
                selectedPos = 4;
            if (selectedPos < 0)
                selectedPos = 0;

            if (selectedPos == 0)
                cube0.SetSelected(true);
            else
                cube0.SetSelected(false);
            if (selectedPos == 1)
                cube1.SetSelected(true);
            else
                cube1.SetSelected(false);
            if (selectedPos == 2)
                cube2.SetSelected(true);
            else
                cube2.SetSelected(false);
            if (selectedPos == 3)
                cube3.SetSelected(true);
            else
                cube3.SetSelected(false);
            if (selectedPos == 4)
                cube4.SetSelected(true);
            else
                cube4.SetSelected(false);

            // Activate menu
            if(cube0.selected)
            {
                cube0_panel.SetActive(true);
            }
            else
            {
                cube0_panel.SetActive(false);
            }
            if (cube1.selected)
            {
                cube1_panel.SetActive(true);
            }
            else
            {
                cube1_panel.SetActive(false);
            }
            if (cube2.selected)
            {
                cube2_panel.SetActive(true);
            }
            else
            {
                cube2_panel.SetActive(false);
            }
            if (cube3.selected)
            {
                cube3_panel.SetActive(true);
            }
            else
            {
                cube3_panel.SetActive(false);
            }
            if (cube4.selected)
            {
                cube4_panel.SetActive(true);
            }
            else
            {
                cube4_panel.SetActive(false);
            }

        }
        else
        {
            cube0.SetHeight(-0.05f);
            cube1.SetHeight(0.02f);
            cube2.SetHeight(0.05f);
            cube3.SetHeight(0.02f);
            cube4.SetHeight(-0.05f);

            cube0.SetSelected(false);
            cube1.SetSelected(false);
            cube2.SetSelected(false);
            cube3.SetSelected(false);
            cube4.SetSelected(false);
            cube4_panel.SetActive(false);
            cube3_panel.SetActive(false);
            cube2_panel.SetActive(false);
            cube1_panel.SetActive(false);
            cube0_panel.SetActive(false);

            if (Input.GetKeyDown("c"))
                isUp = true;
        }
	}
}
