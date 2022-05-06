# COVID-Combat
## CS 6334 Project - Team 15

Demo video link: https://youtu.be/J9V_2wlUCT4


Our StartMenu scene is the start of the game for the players. It then transitions to the GameScene, which is where the rest of the game takes place.

The VR equipment we are using are the Google Cardboard and Bluetooth Controller provided in class.

### Interaction Techniques:
Menu:
-Select options by looking at the desired button and pressing X.


In game:
Player 1: 
- Uses Google VR SDK to see
- Can move left/right/up/down with joystick
- Automatically moves forward
- Hold Trigger to slow down and stop

Player 2:
- Uses Google VR SDK to see and aim
- Presses Trigger to shoot
- Presses Y to use power up
- Presses A to switch antibody gun

The game is configured so that it automatically connects to a specified dedicated server when running the game. This means that the server must be running and on an available network. Sometimes, UTD's firewall blocks connection to the server. In this instance, have all the devices connect to a mobile hotspot. 

To operate with multiple devices while the server is running, simply press the "Join Game" button in the Start Menu, and the devices will automatically join the same session. The server holds two players at once (one game session), and the server is only up when we are actively using/testing it (limited number of free hours for the server to be running). To test the game without the server running, build the project for pc. Then run one instance of the game from the unity editor and run another instance of the game on the executable. Then, with the mouse, select from the UI located on the top left of the screen called "Host (Server + Client)" button for one instance and click client for the other. Note that the health bar and power up bar have been made display properly on the phone, so they appear to be in a strange postion when running on the computer.

### Implemented features:
- 2 player multiplayer
- Voice chat
- Start menu
- Character selection/tutorial menu
- Death Screen (can restart the game from this screen)
- Win Screen (can restart the game from this screen)
- Improved blood vessel course
- All interactable objects:
   - COVID virus:
     - Does the most damage when player collides with it
     - When shot, it increases the score
     - Explodes when shot
   - Megakaryocyte cell:
     - Player collisions do significant damage to the player
     - Player collisions slow the player down
     - Player collisions shake the player
     - Has a sound effect for collisions
     - Gets pushed away when shot
   - Red Blood cell:
     - Player collisions do some damage to the player
     - Player collisions slow the player down
     - Player collisions shake the player
     - Has the same sound effect as megakaryocyte cell
   - B Cell
     - Move at a very fast pace to make them challenging to obtain
     - Collisions regenerate player health
     - Collisions do not slow the player down
     - Has a power up sound
   - T Cell
     - Move at a very fast pace to make them challenging to obtain
     - Player collisions charge the bleach power up
     - Player collisions do not slow the player down
     - Has the same power up sound as the B Cell
   - Blood vessel course
     - Player collisions cause damage to the player only if the player hits the course directly
     - A glancing hit doesn't hurt the player
   - Bleach power up
     - Destroys all nearby objects (excluding the course)
     - Causes the screen to flash white
   - Antibody gun 1
     - Shoots antibodies
     - Antibodies destroy COVID viruses and push other cells out of the way
     - Fires at a rapid rate, but the projectiles move at a slower rate
   - Antibody gun 2
     - Shoots a laser
     - Laser destroys COVID viruses
     - Can also destroy other cells in the way
     - Fires at a slower rate, but the laser reaches its target almost instantly
- Player movement for the pilot (including the ability to slow down)
- Shooter can aim and shoot
- Health bar
  - Players are taken to the death screen once it is depleted
- Bleach power up
  - Charges by collecting T cells
  - Once fully charged, it kills all nearby objects when used
- All prefabs have been created (all cells, the virus, course components, player)

