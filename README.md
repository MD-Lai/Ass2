# Ass2
Ass2

1) What the application does : 
The application is a maze game, imagining the Surface tablet as the board, used to roll a ball around the board.
Game doesn't fully mirror the behaviour of the Surface being a board,
i.e. you cannot drop the ball by flipping the board upside down,
but is an acceptable emulation of a real board.

2) How to use it : 
The Startup menu has a few elements,
A Start Button : Starts the game with settings made on main menu page
Keyboard Mode Button : Sets game to use keyboard mode, will also default to this mode if device has no accelerometer
Tablet Mode Button : Sets game to use Tablet mode, tilt for control, tap and drag for moving camera, pinch to zoom for perspective of camera
Starting Width Slider : Sets the starting width of the board, continues increasing by this amount upon every successful completion of the maze
Starting Height Slider : Same as Width Slider but for the height aspect of the board.

Gameplay has two modes, Keyboard and Tablet mode.
As their names imply, Keyboard mode allows the user to "tilt" the board 
based on WASD and the directional keys and hence cause the ball to roll in the corresponding direction.
Tablet mode alows users to actually tilt the surface and use it intuitively as the board itself. 
(in reality the ball is being controlled and the board is static)

The Goal of the game is to get from the top left hand corner of the board to the bottom right.

3) How objects and entities are modelled : 
The board is procedurally generated using 3 prefabs, Walls, Floors, and Pins, all as standard unity squares with different axis scales.
Usign depth first search on an unconnected maze (i.e. each tile has all walls still up), 
some walls will be marked as false and will not be placed for rendering.
pins are required as the walls do not overlap, and hence corners will have a small gap, the pins are just there to fill out the gap.
It is important to note that the entire maze IS connected, and every point is reachable from any point in the maze, 
hence every maze is solvable. 
The ball is simply a ball as a standard unity asset. 

4) How Graphics and camera motion is handled
The Shaders used are mostly based off the phong shaders given in the labs, with a few modifications.
The Shader used on the board have a light falloff function, where the attenuation factor is based off the distance of the pixel and the light source
Hence, once the maze gets larger, only small sections of it will be lit, the rest will be in darkness. 
The Specular aspects of the shader has a slower falloff than the diffuse aspects,
hence, some highlights will still be visible while it is not lighted strictly coloured. (this is intentional)
Partly inspired by the subtle glow a player has in Dark Souls, 
this also creates a situation where the player has to actually explore the maze rather than simply glancing at it for a solution.

The ball is shaded using a modified version of a phong shader and incorporates rim highlighting, 
where the rims of highlights are coloured differently to the highlight itself,
The light falloff is also less than that of the board, so even when moving the focus of the camera away from the ball,
it will be possible to see a faint outline of the ball. 

Camera motion is handled in two modes, keyboard mode and tablet mode.
In Keyboard mode, the mouse translates the camera parallel to the plane so the player can view more of the maze before progressing.
Scroll wheel on the mouse is used to zoom the camera (affecting the FOV). 
In Tablet mode, touch movements are used to translate the camera in a similar fashion to the above,
with the touch movements being directly proportional to movement along the board.
Camera can be zoomed by pinch and zoom. 

5) Code used from other sources
Most code is original. Some code, particularly for pinch and zoom are made with assistance from the tutorials on unity3d.com and the provided lab materials, respectively






Game is in its final state, based on time of submission.
In the end, the game didn't 100% mirror the initial idea, which was mostly due to inexperience and lack of time.
Given maybe 2 more weeks the game could be a lot more polished, 
and can implement a lot more features that would've taken too long to implement in the time span of the assignment.

