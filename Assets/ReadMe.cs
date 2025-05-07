/*Marking Scheme and how my code reflects it
 * 
 * Setting up the game:
 * 
 * Please load in two scenes to the heirarchy:
 * 1. Menu
 * 2. Echo
 * 
 * Unload Echo but don't remove it and it should load when play is pressed in the Menu.
 * 
 * 
 * 
 * CONTROLS:
 * 
 * W - Move Forward
 * D - Move Backward
 * Right-Click control the camera/rotation of the player
 * Space - Jump
 * Left-shift - Sprint
 * Tab - Throw Rock
 * E - interact
 * 
 * 
 * 
 * Hints:
 * - If you get stuck in the water on the second level and can't jump, try and find the more shallow areas and jump across (the gravity will take you higher than you think!)
 * - Please enjoy my game :)

• +30%: Physics
+ 5 %: Appropriate use of Newtonian physics ----------------------------------- Forces have been used on player movement for walk, jump, run. It is the main component. Wind mechanics in grasss etc.
■ Usage of Rigid Bodies ------------------------------------- Yes, player has a rigid body.
■ Correct application of impulses to bodies ------------------ Impulses have been used to make the player jump.
■ Objects have appropriate mass quantities ------------------- Player has a mass of 70kg. Everything else is 1 as it is a technical "Dreamscape" where its a ghost area. Ghosts dont have mass so I didn't adjust it for them ( i also removed their rigid bodies)
■ Game mechanics is physics-driven (e.g., immediate response to collisions). ----------------------- Yes, i mean this is pretty self explanatory and you can see for yourself but otherwise e.g throwing a rock at a target it will stick to trigger the target. Collisions with water etc


+5 %: Advanced Physics(multiple gravity areas / changing mass / et ceteram) --------------------- Level 2 the gravity is increased for moon jump.
■ Physics properties are changed via scripts ------------------------- Gravity is changed during second level.
■ Mass/Physics is a gameplay mechanic -------------- gravity moon jump is implemented so the player can reach higher orbs to collect.
■ Additional forces beyond simple motion-driven accelerations are provided (projectile
trajectories, gravity interfering with the velocity). ------------ throwing rocks, gravity also does interfere with players velocity in level2
■ AI uses calculations to determine projectile forces. ------------- I don't think i attempted this one :(
2


+5%: Basic Collision Volumes
■ There is at least one collision volume --- Yep. On... pretty much everything ahah.
■ There is more than one collision volume --- yes.
■ The collision volume is appropriate and matches the Game Object’s mesh. --- yes.


+5%: Advanced collision volumes.
■ A single Game Object has multiple colliders. --- yes. the player has multiple for swim detection.
■ Colliders are enabled/disabled via scripts. - yep, specifically for the final meeting with the dad or interaction with the orbs.
■ Colliders can change their position programmatically. --- yes, two npc ghosts operate on a navmesh and thus causes their colliders to move with them (which are very important for interaction).
■ Trigger volumes are used as part of player mechanics. --- yes, most colliders are trigger volumes for player interaction.


+5%: Appropriate collision response and feedback.
■ Rigidbody responds to collisions realistically ---------- yep! when you pick up the rock at the top of the hill you get 100 of them just so you can experiment with the realistic collisions (just don't aim for the target if you do want to experiment).
■ An object makes use of OnCollisionEnter/Exit --------- player when entering orb interaction areas, or water, or cutscene zones.
■ Collision Layers are used to separate out collision types ---- yes, the collision matrix has been modified for this.
■ Physics materials are used --- yep the water has one and it slows down the player when in contact.


+5%: Advanced collision response and feedback
■ Multiple physics materials appearing in the game. -- yep the terrain has one, the water has one.
■ Physics materials are changed at run-time via script -- Changing gravity at run-time? not sure about this one.
■ Trigger volumes are used to trigger gameplay events --- YES! definite yes. Large proportion of the game depends on this.


• +20%: Graphics
+ 10 %: Appropriate Use of Graphical Elements
■ Multiple textures appearing in game -- yes. A lot.
■ Appropriate use of lighting - yes. freshly baked.
■ GameObjects moving & rotating via script ---- NPCs operating on Navmesh, Orbs oscillating, flowers growing.
■ A navigable camera moving in 3D --- Yep.
+10%: Advanced Graphics
■ Environment appears to extend infinitely --- yes. although i didnt make an infinte generating game, when you reach the top of the mountain the terrain is designed to "appear" indefinite. it also comes along with a cute cutscene :)
■ A body of realistic looking water --- yes, pond & waterfall.
■ Scripted lighting/effects (e.g. weather, day/night cycle) ---- yes, rain and increasede darkness/fog via script.
■ Change object appearance via script ----------- yes, the skybox is changed via script, the flowers, the orbs, the trees.
■ Geometry that changes over time (e.g. plants growing) --------- flower trail behind the player associated with the joy emotion, there are plenty of other examples in there too but i was most proud of the flowers aha.


• +10 %: Pathfinding
■ NavMeshAgents are used ------- yep! two npc ghosts.
■ NavMeshObstacles are used --- yep. although in my case i used volumes as it was easier for me.
■ Custom pathfinding code, or external library with some modifications. ---- custom pathfinding? sure. the code is set up so pathfinding is random until it isn't and then its dependant on the player.
3
■ AI makes decisions based on pathfinding --- yep they can choose to wait, wander or follow.

• Artificial Intelligence
+10% State Machines
■ Usage of simple state-machines ---- the npc decisions are actually set up in a state machine!
■ Usage of boolean or state-driven state machines --- yep state driven.
■ Usage of object-encapsulation for modelling states ---- i have no clue, i think so?
■ Usage of hierarchical state machines or usage of external tools for generating state
machines. --- don't think so
■ State machines are triggered by external events or timeouts. ------ yes, player.
■ Usage of probabilistic/stochastic state transitions. ---- think so? ( i did this a while ago)

+10%: Advanced AI
■ Usage of Planning techniques (Real Planners, GOAPs) ---- yes
■ Usage of Non-Cooperative Game Strategies (Min-Max trees, 𝛼-𝛽 pruning) ---- nope.
■ Usage of basic Reinforcement Learning techniques. ----nope?

• +10%: Structuring NPCs
■ NPCs are not orchestrated, and mainly react directly to the player or the environment. ---- yep they can follow the player if they wish or are within a certain radius.
■ NPCs are coordinated as a group or in tandem, to fulfil the same goal or task. ------ yep they can co-ordinate or separate. They have a purpose to the story and its their goal to give you information and can choose from random conversations to do so.
■ An orchestrator or game manager is used to handle multiple and contrasting behaviours, 
thus including the generation of random events. ---- yep as mentioned just before^.


• Advanced Features
■ [+2%] Appropriate usage of Prefabs. -- yep! my orbs have a prefab when they "shatter", my rock is also a prefab.
■ [+2%] Levels / Menus are separated into distinct scenes. ---- i designed a menu just for the purpose of this aha! but yes.
■ [+3%] Evidence of code for limiting expensive computations (e.g., raycasting). --- yep! lots of raycasting.
■ [+2%] Flocking techniques. ----- didnt have time but really wanted to :(
■ [+2%] Usage of Vector Fields. ---- nope!
■ [+2%] Usage of Particle Systems. ----- yes: Rain & waterfall are both particle systems.
■ [+2%] Implementation of custom AI tools (AverageMinMax, et ceteram) ---- dont think so?




Library:
Please clearly cite any asset or external library being used to realise the project. You will be
assessed on both on the quality of the library of choice and on your ability of delivering a game
level according to the marking scheme


Assets:

**All assets have been taken from Unity Marketplace UNLESS stated explicitly otherwise**
*
- AA_Universal Characters - Two NPC models
- ChiaSeminko Madeline - The main player model & animations
- Anim - Animations from Mixamo.com for NPCs and Ghosts
- Buttons - designed by me in photoshop
- Rocks pack Lite - rocks for the pond
- Dialogue Editor - dialogue system for interaction
- HandPainted Textures - used on the ground terrain.
- IL3DN - foliage
- JC_Stylized Rocks - foliage rocks
- Kevin Iglesias BASICMOTIONS - used Avatar rigs.
- Misc - my own scripts and edited unity shapes as well as imported photoshop materials. 
- Misc, Audio - All taken from Productioncrate.com a royalty free website.
- MossyRocks - used the painted texture.
- Procedural terrain painting - used for the main terrain.
- Proxy Games - my trees
- Rock_cliff - texture for the mountain
- Sky Series - my skyboxes
- - Unity Technologies Lowpoly Rio - the "Dad" model in the game
- Stylized Vegetation pack - grass texture
- Waterfall - all modelled and made myself in Blender/Unity.
- The cutscene's music is also my own composed music snippet and I own the full rights to it.*/