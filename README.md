# COVID-Combat
 CS 6334 Project - Team 15

Our StartMenu scene is the start of the game for the players. It then transitions to the GameScene, which is where the rest of the game takes place.

-----NEED TO EXPLAIN HOW TO OPERATE WITH MULTIPLE DEVICES-----





--------------------------
Development Notes:

If you get the error "The name 'PlayFabMultiplayerAgentAPI1 does not exist in the current context, go to Edit -> Projet Settings -> Player -> Other Settings -> Scripting Define Symbols, click the + to add a new define called ENABLE_PLAYFABSERVER_API

This adds in the necessary directive


Prefab Info:
The prefabs folder has the prefabs to use. Inside that folder is another folder called BlenderFiles, which is where I am storing
all the files I used to make the prefabs, so don't use those in the game.

The blood vessels can be put together like legos to construct the course.

This should automatically be included, but if not, the Healthbar max health can be set to 100. On the WhiteBloodCell's PilotControl
script, Force = 25 and turnRate = 1 give good values for the controls.
