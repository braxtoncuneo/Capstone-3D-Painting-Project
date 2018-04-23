namespace VRTK.Examples
{
    using UnityEngine;
    using UnityEditor.UI;
    using UnityEngine.UI;
    using UnityEngine.Events;
    using System.IO;
    using Valve.VR;

    public class ControllerEventHandlerLeft : MonoBehaviour
    {
        public SteamVR_TrackedObject target;
        public MenuInputs MenuManager;
        public GameObject colorPicker, rightController, fileText;
        public Slider alphaSlider;
        public Button saveUI, loadUI;
        

        private bool triggerDown = false, touchpadDown = false, gripDown = false, save = false, load = false;
        private float x, y, hue, sat, val, alpha;
        string f = "";

        private void Update()
        {
            //checks if control stick is being used
            if (touchpadDown)
            {
                //check if grip button is pressed in tandom
                if (gripDown)
                {
                    //if grip button is pressed, touchpad will control hue and alpha
                    //conditionals to keep from going beyond value limits
                    hue += (y / 45f);
                    if (hue > 1f)
                        hue = 1f;
                    if (hue < 0f)
                        hue = 0f;
                    colorPicker.GetComponent<ColorPicker>().AssignColor(ColorValues.Hue, hue);

                    alpha += (x / 45f);
                    if (sat > 1f)
                        sat = 1f;
                    if (sat < 0f)
                        sat = 0f;
                    alphaSlider.value = alpha;
                    colorPicker.GetComponent<ColorPicker>().AssignColor(ColorValues.A, alpha);

                }
                else
                {
                    //if grip is not pressed, touchpad will control saturation and value;
                    //conditionals to keep from going beyond value limits
                    val += (y / 45f);
                    if (val > 1f)
                        val = 1f;
                    if (val < 0f)
                        val = 0f;
                    colorPicker.GetComponent<ColorPicker>().AssignColor(ColorValues.Value, val);

                    sat += (x / 45f);
                    if (sat > 1f)
                        sat = 1f;
                    if (sat < 0f)
                        sat = 0f;
                    colorPicker.GetComponent<ColorPicker>().AssignColor(ColorValues.Saturation, sat);
                }
            }
        }

        private void Start()
        {
            if (GetComponent<VRTK_ControllerEvents>() == null)
            {
                Debug.LogError("VRTK_ControllerEvents_ListenerExample is required to be attached to a Controller that has the VRTK_ControllerEvents script attached to it");
                return;
            }

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

            //keyboard event listeners
            SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Listen(OnKeyboardCharInput);
            SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(OnKeyboardClosed);

            //button listeners
            Button saveBtn = saveUI.GetComponent<Button>();
            saveBtn.onClick.AddListener(saveButton);
            Button loadBtn = loadUI.GetComponent<Button>();
            loadBtn.onClick.AddListener(loadButton);
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

        private void DoTriggerReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "TRIGGER", "released", e);
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
        }

        private void DoGripReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "GRIP", "released", e);
            gripDown = false;
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
            if (1 == MenuManager.getPos())
            {
                touchpadDown = true;
                Debug.Log("Touchpad on");
                x = e.touchpadAxis.x;
                y = e.touchpadAxis.y;
            }
        }

        private void DoButtonOnePressed(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON ONE", "pressed down", e);

        }

        private void DoButtonOneReleased(object sender, ControllerInteractionEventArgs e)
        {
            DebugLogger(e.controllerIndex, "BUTTON ONE", "released", e);
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

            //SteamVR.instance.overlay.ShowKeyboard(0, 0, "Test", 256, "", true, 0);
            //load = true;
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

        private void saveButton()
        {
            SteamVR.instance.overlay.ShowKeyboard(0, 0, "Test", 256, "", true, 0);
            save = true;
        }

        private void loadButton()
        {
            SteamVR.instance.overlay.ShowKeyboard(0, 0, "Test", 256, "", true, 0);
            load = true;
        }

        private void OnKeyboardCharInput(VREvent_t ev)
        {
            Debug.Log("Keyboard receiving input");
            if ('\b' == (char)ev.data.keyboard.cNewInput0)
            {
                if (0 != f.Length)
                {
                    f = f.Remove(f.Length - 1);
                }
            }
            else
            {
                f += (char)ev.data.keyboard.cNewInput0;
            }
            fileText.GetComponent<Text>().text = f;
        }

        private void OnKeyboardClosed(VREvent_t ev)
        {
            Debug.Log("Keyboard closing");
            if(save)
            {
                rightController.GetComponent<ControllerEventHandlerRight>().saveFile(f);
                save = false;
            }
            else if(load)
            {
                rightController.GetComponent<ControllerEventHandlerRight>().loadFile(f);
                load = false;
            }
            f = "";
        }
    }
}