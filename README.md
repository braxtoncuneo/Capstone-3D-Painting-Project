# Capstone-3D-Painting-Project
The team repository for a 3D VR painting environment

Works by individual team members are contained in the folder with their respective name.

Files associated with a specific step/aspect of the project are contained by folders with the name that step/aspect.

Notes

This program was developed on Oculus hardware. While the program should, in theory, be platform agnostic and 
compatible with other VR devices, (Vive etc.) errors and controller incompatibilities may arise.

Running the Program:
Run the executable labled 'Version-0.1-Executable' located in the 'ReleaseBuilds' folder.

Controls

Brush Stroke - Right Trigger
Hold down the trigger and move your controller from one point to another creating a stroke of 'paint'  through the air.

Change Saturation/Value - Left thumbstick/trackpad, Click Pointer
Changes the saturation and value of the color that will be produced by a brush stroke. Directing the 
thumbstick/trackpad up and down increases and  decreases the value, (i.e., lighter to darker color) 
respectively, and moving the input right or left increases and decreases the saturation, (i.e., greyscale to color) 
respectively. Alternatively, the user can select a specific saturation and opacity by directing to it with the pointer 
and clicking on it.

Change Hue/Opacity - Left Thumbstick/Trackpad + Grip, Click Pointer
Changes the hue and opacity of the color that will be produced by a brush stroke. Directing the thumbstick/trackpad up 
and down increases and decreases hue, (i.e., the color chosen) respectively, and moving the input right or left 
increases and decreases the opacity, respectively. Alternatively, the user can select a specific hue or opacity by 
directing to it with the pointer and clicking on it.

Change Brush Scale - Right Thumbstick/Trackpad
Changes the size and shape of the brush, which alters the amount of area covered in a brush stroke. Moving the input 
right and left increases and decreases the diameter of the brush, respectively, while moving it forward or back 
increases and decreases the hight of the brush, respectively.

Change Scene Scale and Position - Right Button 1 + Move Hands
By holding button one on the right controller, and changing the distance between the controllers, the scale of the 
scene will change. Moving the controllers away from one another will increases the scene scale, and moving them closer 
will decrease it. Changing their position, but keeping the distance between them constant will move the scene relative 
the position of the controllers.

Toggle Pointer - Right Button 2
Turns the input pointer on and off. This is used to interact with the UI by pointing to it and clicking on it.

Pointer Click - Right Trigger when Pointer is set to On
When the pointer is toggled on, it produces a beam which turns from red to green when it makes contact with a UI menu. 
When it is pointed at an interactable element, such as a button or the pallete, pressing the right trigger will 
interact with that UI element.

Saving and Loading

To save or load a scene configuration, use the UI to navigate to the save/load menu. When saving and loading, make sure
to to be looking toward the horizon. Use the pointer to click on either the save or load button. A keyboard will appear.
Using the controller pointers, type in the name of the file you wish to save or load. When you are finished typing, 
click the 'done' button. If a complexe scene is loaded, the program may take some time to load. Simnply wait for it to
finish. Save files will be placed in the 'SaveFiles' folder.

User Notes:
When the user's head is near a given region of the scene, voxels painted into that region will become invisible, as to
not obscure user vision.