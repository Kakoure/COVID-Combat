# COVID-Combat
## CS 6334 Project - Team 15

Demo video link: **Need final video link here**


Our StartMenu scene is the start of the game for the players. It then transitions to the GameScene, which is where the rest of the game takes place.

The VR equipment we are using are the Google Cardboard and Bluetooth Controller provided in class.

### Interaction Techniques:
Menu:
-Select options by looking at the desired button and pressing X.


In game:
Player 1: 
-Uses Google VR SDK to see
-Can move left/right/up/down with joystick
-Automatically moves forward
-Hold Trigger to slow down and stop

Player 2:
-Uses Google VR SDK to see and aim
-Presses Trigger to shoot
-Presses Y to use power up
-Presses A to switch antibody gun

The game is configured so that it automatically connects to a specified dedicated server when running the game. This means that the server must be running and on an available network. Sometimes, UTD's firewall blocks connection to the server. In this instance, have all the devices connect to a mobile hotspot. 

To operate with multiple devices while the server is running, simply press the "Join Game" button in the Start Menu, and the devices will automatically join the same session. The server holds two players at once (one game session), and the server is only up when we are actively using/testing it (limited number of free hours for the server to be running). To test the game without the server running, build the project for pc. Then run one instance of the game from the unity editor and run another instance of the game on the executable. Then, with the mouse, select from the UI located on the top left of the screen called "Host (Server + Client)" button for one instance and click client for the other. Note that the health bar and power up bar have been made display properly on the phone, so they appear to be in a strange postion when running on the computer.

### Implemented features:
- 2 player multiplayer
- voice chat
- Start menu
- Character selection/tutorial menu
- Death Screen (can restart the game from this screen)
- Win Screen (can restart the game from this screen)
- Improved blood vessel course
- All interactable objects:
   - COVID virus:
     - does the most damage when player collides with it
     - when shot, it increases the score
     - Explodes when shot
   - Megakaryocyte cell:
     - player collisions do significant damage to the player
     - player collisions slow the player down
     - player collisions shake the player
     - has a sound effect for collisions
     - gets pushed away when shot
   - Red Blood cell:
     - player collisions do some damage to the player
     - player collisions slow the player down
     - player collisions shake the player
     - has the same sound effect as megakaryocyte cell
   - B Cell
     - move at a very fast pace to make them challenging to obtain
     - collisions regenerate player health
     - collisions do not slow the player down
     - has a power up sound
   - T Cell
     - move at a very fast pace to make them challenging to obtain
     - player collisions charge the bleach power up
     - player collisions do not slow the player down
     - has the same power up sound as the B Cell
   - Blood vessel course
     - player collisions cause damage to the player only if the player hits the course directly
     - a glancing hit doesn't hurt the player
   - Bleach power up
     - destroys all nearby objects (excluding the course)
     - causes the screen to flash white
   - Antibody gun 1
     - shoots antibodies
     - antibodies destroy COVID viruses and push other cells out of the way
     - fires at a rapid rate, but the projectiles move at a slower rate
   - Antibody gun 2
     - shoots a laser
     - laser destroys COVID viruses
     - fires at a slower rate, but the laser reaches its target almost instantly
- Player movement for the pilot (including the ability to slow down)
- Shooter can aim and shoot
- Health bar
  - players are taken to the death screen once it is depleted
- Bleach power up
  - charges by collecting T cells
  - once fully charged, it kills all nearby objects when used
- All prefabs have been created (all cells, the virus, course components, player)

