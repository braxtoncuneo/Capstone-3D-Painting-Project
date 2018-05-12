using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputs : MonoBehaviour
{

    public GameObject cube0_panel;
    public GameObject cube1_panel;
    public GameObject cube2_panel;
    public GameObject cube3_panel;
    public GameObject cube4_panel;

    public GameObject mainCanvas;
    public GameObject selfManager;

    public bool isUp = true;
    public int selectedPos = 1;

    // Use this for initialization
    void Start()
    {
        mainCanvas.SetActive(true);
        selfManager.SetActive(true);
    }
    
    public void nextPanel()
    {
        selectedPos++;
        Debug.Log("Menu manager is firing, " + selectedPos.ToString());
    }

    public void prevPanel()
    {
        selectedPos--;
    }

    public int getPos()
    {
        return selectedPos;
    }

    // Update is called once per frame
    void Update()
    {
        selfManager.SetActive(true);

        if (isUp)
        {
            mainCanvas.SetActive(true);

            if (Input.GetKeyDown("q"))
            {
                selectedPos -= 1;
            }
            if (Input.GetKeyDown("e"))
            {
                selectedPos += 1;
            }
            if (Input.GetKeyDown("c"))
                isUp = false;

            if (selectedPos > 1)
                selectedPos = 1;
            if (selectedPos < 0)
                selectedPos = 0;

           

            // Activate menu
            if (0 == selectedPos)
            {
                cube0_panel.SetActive(true);
            }
            else
            {
                cube0_panel.SetActive(false);
            }
            if (1 == selectedPos)
            {
                cube1_panel.SetActive(true);
            }
            else
            {
                cube1_panel.SetActive(false);
            }
            if (2 == selectedPos)
            {
                cube2_panel.SetActive(true);
            }
            else
            {
                cube2_panel.SetActive(false);
            }
            if (3 == selectedPos)
            {
                cube3_panel.SetActive(true);
            }
            else
            {
                cube3_panel.SetActive(false);
            }
            if (4 == selectedPos)
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
            mainCanvas.SetActive(false);

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
