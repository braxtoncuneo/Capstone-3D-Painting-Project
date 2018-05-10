namespace VRTK.Examples
{
    using UnityEngine;
    using System.IO;

    public class ControllerEventHandlerRight : MonoBehaviour
    {
        public Block prefab;
        public SteamVR_TrackedObject target;
        public Brush brush;
        public GameObject colorPicker, leftControllerModel, rightControllerModel;

        private bool brushDown = false, gripDown = false, touchpadDown = false, buttonOneDown = false;
        private Vector4 color = new Vector4(0.0f, 0f, 1.0f, 0.0f);
        private string save = "";
        private float x, y, scaleXZ = .1f, scaleY = .1f;
        private Vector3 controllerModelLeftPos, controllerModelRightPos;

        public MenuInputs MenuManager;

        public void saveFile(string f)
        {
            if ("" != f)
            {
                f = "SaveFiles/" + f + ".txt";
                //NOTE: Commenting out save function for current build. Will remove code once menu-controlled saving is implemented
                File.WriteAllText(f, save);
                Debug.Log("Save = " + save);
            }
        }

        public void loadFile(string f)
        {
            if ("" != f)
            {
                float posX, posY, posZ, cX, cY, cZ, cA, sXZ, sY;
                Vector4 tempColor;
                Vector3 tempScale = brush.transform.localScale;
                f = "SaveFiles/" + f + ".txt";
                if (File.Exists(f))
                {
                    save = "";
                    brush.Wipe();

                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(f);
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] subLine = line.Split();
                        if ("d" == subLine[0])
                        {
                            posX = float.Parse(subLine[1]);
                            posY = float.Parse(subLine[2]);
                            posZ = float.Parse(subLine[3]);
                            brush.transform.position = new Vector3(posX, posY, posZ);
                            save += "d " + brush.transform.position.x + " " + brush.transform.position.y + " " + brush.transform.position.z + "\n";
                            brush.Down(null);
                        }
                        else if ("u" == subLine[0])
                        {
                            posX = float.Parse(subLine[1]);
                            posY = float.Parse(subLine[2]);
                            posZ = float.Parse(subLine[3]);

                            brush.transform.position = new Vector3(posX, posY, posZ);
                            brush.Up();
                            save += "u " + brush.transform.position.x + " " + brush.transform.position.y + " " + brush.transform.position.z + "\n";
                        }
                        else
                        {
                            cX = float.Parse(subLine[0]);
                            cY = float.Parse(subLine[1]);
                            cZ = float.Parse(subLine[2]);
                            cA = float.Parse(subLine[3]);
                            posX = float.Parse(subLine[4]);
                            posY = float.Parse(subLine[5]);
                            posZ = float.Parse(subLine[6]);
                            sXZ = float.Parse(subLine[7]);
                            sY = float.Parse(subLine[8]);
                            brush.transform.localScale = new Vector3(sXZ, sY, sXZ);
                            tempColor = new Vector4(cX, cY, cZ, cA);
                            brush.transform.position = new Vector3(posX, posY, posZ);
                            brush.StrokeNoUpdate(tempColor, tempColor, new Vector4(0, 0, 0, 0));
                            save += cX + " " + cY + " " + cZ + " " + cA + " " + posX + " " + posY + " " + posZ + " " + sXZ + " " + sY + "\n";
                        }
                    }
                    brush.PerformUpdate();
                    brush.transform.localScale = tempScale;
                }
            }
        }

        private void Update()
        {
            //set color value from left controller
            color.x = colorPicker.GetComponent<ColorPicker>().GetValue(ColorValues.R);
            color.y = colorPicker.GetComponent<ColorPicker>().GetValue(ColorValues.G);
            color.z = colorPicker.GetComponent<ColorPicker>().GetValue(ColorValues.B);
            color.w = colorPicker.GetComponent<ColorPicker>().GetValue(ColorValues.A);

            brush.transform.position = target.transform.position;
            brush.transform.rotation = target.transform.rotation;
            if (brushDown)
            {
                //draw
                Debug.Log("Brush is down");
                brush.Stroke(color, color, new Vector4(0, 0, 0, 0));
                save += color.x + " " + color.y + " " + color.z + " " + color.w + " " + brush.transform.position.x + " " + brush.transform.position.y + " " + brush.transform.position.z + " " + scaleXZ + " " + scaleY + "\n";
            }
            else if (gripDown)
            {
                //erase
                Debug.Log("Eraser is down");
                brush.Stroke(new Vector4(0,0,0,0), new Vector4(0, 0, 0, 0), new Vector4(0, 0, 0, 0));
                save += color.x + " " + color.y + " " + color.z + " " + color.w + " " + brush.transform.position.x + " " + brush.transform.position.y + " " + brush.transform.position.z + " " + scaleXZ + " " + scaleY + "\n";
            }
            else if (buttonOneDown)
            {
                Block.Scale(controllerModelLeftPos, controllerModelRightPos, leftControllerModel.transform.position, rightControllerModel.transform.position);
                Debug.Log("Scaling; left starting position: " + controllerModelLeftPos.ToString() + ", left ending position: " +
                          leftControllerModel.transform.position.ToString() + 
                          ", right starting position: " + controllerModelRightPos.ToString() +
                          ", right ending position: " + rightControllerModel.transform.position.ToString());
                controllerModelLeftPos = leftControllerModel.transform.position;
                controllerModelRightPos = rightControllerModel.transform.position;
            }

            //change brush size
            if (touchpadDown)
            {
                float min = Block.blockWidth / (float)Block.width ;
                float max = (float)Block.width;
                float step = (max - min) / 36000f;

                scaleXZ += (step * x);
                if (scaleXZ >= max)
                    scaleXZ = max;
                if (scaleXZ <= min)
                    scaleXZ = min;

                scaleY += (step * y);
                if (scaleY >= max)
                    scaleY = max;
                if (scaleY <= min)
                    scaleY = min;

                brush.transform.localScale = new Vector3(scaleXZ, scaleY, scaleXZ);
            }
        }

        private void Start()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

            brush.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
            initGrid(4);


            //Setup controller event listeners
            GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
            GetComponent<VRTK_ControllerEvents>().TriggerReleased += new ControllerInteractionEventHandler(DoTriggerReleased);

            GetComponent<VRTK_ControllerEvents>().TriggerTouchStart += new ControllerInteractionEventHandler(DoTriggerTouchStart);
            GetComponent<VRTK_ControllerEvents>().TriggerTouchEnd += new ControllerInteractionEventHandler(DoTriggerTouchEnd);

            GetComponent<VRTK_ControllerEvents>().TriggerHairlineStart += new ControllerInteractionEventHandler(DoTriggerHairlineStart);
            GetComponent<VRTK_ControllerEvents>().TriggerHairlineEnd += new ControllerInteractionEventHandler(DoTriggerHairlineEnd);

            GetComponent<VRTK_ControllerEvents>().TriggerClicked += new ControllerInteractionEventHandler(DoTriggerClicked);
            GetComponent<VRTK_ControllerEvents>().TriggerUnclicked += new ControllerInteractionEventHandler(DoTriggerUnclicked);

            GetComponent<VRTK_ControllerEvents>().TriggerAxisChanged += new ControllerInteractionEventHandler(DoTriggerAxisChanged);

            GetComponent<VRTK_ControllerEvents>().GripPressed += new ControllerInteractionEventHandler(DoGripPressed);
            GetComponent<VRTK_ControllerEvents>().GripReleased += new ControllerInteractionEventHandler(DoGripReleased);

            GetComponent<VRTK_ControllerEvents>().GripTouchStart += new ControllerInteractionEventHandler(DoGripTouchStart);
            GetComponent<VRTK_ControllerEvents>().GripTouchEnd += new ControllerInteractionEventHandler(DoGripTouchEnd);

            GetComponent<VRTK_ControllerEvents>().GripHairlineStart += new ControllerInteractionEventHandler(DoGripHairlineStart);
            GetComponent<VRTK_ControllerEvents>().GripHairlineEnd += new ControllerInteractionEventHandler(DoGripHairlineEnd);

            GetComponent<VRTK_ControllerEvents>().GripClicked += new ControllerInteractionEventHandler(DoGripClicked);
            GetComponent<VRTK_ControllerEvents>().GripUnclicked += new ControllerInteractionEventHandler(DoGripUnclicked);

            GetComponent<VRTK_ControllerEvents>().GripAxisChanged += new ControllerInteractionEventHandler(DoGripAxisChanged);

            GetComponent<VRTK_ControllerEvents>().TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPressed);
            GetComponent<VRTK_ControllerEvents>().TouchpadReleased += new ControllerInteractionEventHandler(DoTouchpadReleased);

            GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
            GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);

            GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);

            GetComponent<VRTK_ControllerEvents>().ButtonOnePressed += new ControllerInteractionEventHandler(DoButtonOnePressed);
            GetComponent<VRTK_ControllerEvents>().ButtonOneReleased += new ControllerInteractionEventHandler(DoButtonOneReleased);

            GetComponent<VRTK_ControllerEvents>().ButtonOneTouchStart += new ControllerInteractionEventHandler(DoButtonOneTouchStart);
            GetComponent<VRTK_ControllerEvents>().ButtonOneTouchEnd += new ControllerInteractionEventHandler(DoButtonOneTouchEnd);

            GetComponent<VRTK_ControllerEvents>().ButtonTwoPressed += new ControllerInteractionEventHandler(DoButtonTwoPressed);
            GetComponent<VRTK_ControllerEvents>().ButtonTwoReleased += new ControllerInteractionEventHandler(DoButtonTwoReleased);

            GetComponent<VRTK_ControllerEvents>().ButtonTwoTouchStart += new ControllerInteractionEventHandler(DoButtonTwoTouchStart);
            GetComponent<VRTK_ControllerEvents>().ButtonTwoTouchEnd += new ControllerInteractionEventHandler(DoButtonTwoTouchEnd);

            GetComponent<VRTK_ControllerEvents>().StartMenuPressed += new ControllerInteractionEventHandler(DoStartMenuPressed);
            GetComponent<VRTK_ControllerEvents>().StartMenuReleased += new ControllerInteractionEventHandler(DoStartMenuReleased);

            GetComponent<VRTK_ControllerEvents>().ControllerEnabled += new ControllerInteractionEventHandler(DoControllerEnabled);
            GetComponent<VRTK_ControllerEvents>().ControllerDisabled += new ControllerInteractionEventHandler(DoControllerDisabled);

            GetComponent<VRTK_ControllerEvents>().ControllerIndexChanged += new ControllerInteractionEventHandler(DoControllerIndexChanged);
        }

        private void DebugLogger(uint index, string button, string action, ControllerInteractionEventArgs e)
        {
            Debug.Log("Controller on index '" + index + "' " + button + " has been " + action
                    + " with a pressure of " + e.buttonPressure + " / trackpad axis at: " + e.touchpadAxis + " (" + e.touchpadAngle + " degrees)");
        }

        private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "pressed", e);
            brushDown = true;
            brush.Down(null);
            save += "d " + brush.transform.position.x + " " + brush.transform.position.y + " " + brush.transform.position.z + "\n";
        }

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "released", e);
            brush.Up();
            brushDown = false;
            save += "u " + brush.transform.position.x + " " + brush.transform.position.y + " " + brush.transform.position.z + "\n";
        }

        private void DoTriggerTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "touched", e);
        }

        private void DoTriggerTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "untouched", e);
        }

        private void DoTriggerHairlineStart(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "hairline start", e);
        }

        private void DoTriggerHairlineEnd(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "hairline end", e);
        }

        private void DoTriggerClicked(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "clicked", e);
        }

        private void DoTriggerUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "unclicked", e);
        }

        private void DoTriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "axis changed", e);
        }

        private void DoGripPressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "pressed", e);
            gripDown = true;
            brush.Down(null);
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "released", e);
            gripDown = false;
            brush.Up();
        }

        private void DoGripTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "touched", e);
        }

        private void DoGripTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "untouched", e);
        }

        private void DoGripHairlineStart(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "hairline start", e);
        }

        private void DoGripHairlineEnd(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "hairline end", e);
        }

        private void DoGripClicked(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "clicked", e);
        }

        private void DoGripUnclicked(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "unclicked", e);
        }

        private void DoGripAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "axis changed", e);
        }

        private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TOUCHPAD", "pressed down", e);
        }

        private void DoTouchpadReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TOUCHPAD", "released", e);
        }

        private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TOUCHPAD", "touched", e);
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TOUCHPAD", "untouched", e);
            Debug.Log("Touchpad off");
            touchpadDown = false;
        }

        private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TOUCHPAD", "axis changed", e);
            x = e.touchpadAxis.x;
            y = e.touchpadAxis.y;
            touchpadDown = true;
        }

        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON ONE", "pressed down", e);
            buttonOneDown = true;
            controllerModelLeftPos = leftControllerModel.transform.position;
            controllerModelRightPos = rightControllerModel.transform.position;
        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON ONE", "released", e);
            buttonOneDown = false;
        }

        private void DoButtonOneTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON ONE", "touched", e);
        }

        private void DoButtonOneTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON ONE", "untouched", e);
        }

        private void DoButtonTwoPressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON TWO", "pressed down", e);
            //loadFile("save");
        }

        private void DoButtonTwoReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON TWO", "released", e);
        }

        private void DoButtonTwoTouchStart(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON TWO", "touched", e);
        }

        private void DoButtonTwoTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON TWO", "untouched", e);
        }

        private void DoStartMenuPressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "START MENU", "pressed down", e);
        }

        private void DoStartMenuReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "START MENU", "released", e);
        }

        private void DoControllerEnabled(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "CONTROLLER STATE", "ENABLED", e);
        }

        private void DoControllerDisabled(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "CONTROLLER STATE", "DISABLED", e);
        }

        private void DoControllerIndexChanged(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "CONTROLLER STATE", "INDEX CHANGED", e);
        }


        private void initGrid(int width)
        {
            for(int z = 0; z < width; z++)
            {
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Block b = Instantiate(prefab);
                        b.transform.SetPositionAndRotation(
                            new Vector3(x * Block.blockWidth, y * Block.blockWidth, z * Block.blockWidth),
                            new Quaternion()
                        );
                        brush.blocks.Add(b);
                    }
                }
            }
        }

    }
}