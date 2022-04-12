# COVID-Combat
 CS 6334 Project - Team 15

Demo video link: https://youtu.be/tZ0ybF2Znz8


Our StartMenu scene is the start of the game for the players. It then transitions to the GameScene, which is where the rest of the game takes place.

The VR equipment we are using are the Google Cardboard and Bluetooth Controller provided in class.

Interaction Techniques:
Menu:
	-Select options by looking at the desired button and pressing X.


In game:
Player 1: 
	-Uses Google VR SDK to see
	-Can move left/right/up/down with joystick
	-Automatically moves forward
	-Hold Trigger to stop moving
Player 2:
	-Uses Google VR SDK to see and aim
	-Presses Trigger to shoot

The game is configured so that it automatically connects to a specified dedicated server when running the game. To operate with multiple devices, simply press the "Join Game" button in the Start Menu, and the devices should automatically join the same session. However, in its current state, the server only holds two players at once (one game session), and the server is only up when we are actively using/testing it (limited number of free hours for the server to be running). If running the project through the unity editor, however, a local version of the game can be tested by using the UI located on the top left of the screen and selecting the "Host (Server + Client)" button.





--------------------------
Development Notes:

If you get the error "The name 'PlayFabMultiplayerAgentAPI1 does not exist in the current context, go to Edit -> Projet Settings -> Player -> Other Settings -> Scripting Define Symbols, click the + to add a new define called ENABLE_PLAYFABSERVER_API

This adds in the necessary directive


Prefab Info:
The prefabs folder has the prefabs to use. Inside that folder is another folder called BlenderFiles, which is where I am storing
all the files I used to make the prefabs, so don't use those in the game.

The blood vessels can be put together like legos to construct the course.

This should automatically be included, but if not, the Healthbar max health can be set to 100. On the WhiteBloodCell's PilotControl
script, Force = 100 and turnRate = 1 give good values for the controls.
