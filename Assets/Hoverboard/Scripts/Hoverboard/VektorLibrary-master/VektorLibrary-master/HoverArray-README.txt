This class was taken from an active project and by extension from a larger framework. It is not designed to be a simple monobehavior
and must be instantiated and updated from within a large vehicle controller behavior. Though modifying it to work as a monobehavior would be
fairly simple.

How to use:
1. Create an inspector field for the hover array and configure the parameters in the Unity Inspector.
  - Example: [SerializeField] private HoverArray _hoverArray = new HoverArray();

2. Initialize the hover array in your controller's Start method by passing in the rigidbody of the vehicle.
  - Example: _hoverArray.Initialize(_vehicleBody);
  
3. Update the hover array in your controller FixedUpdate method as it depends on the physics engine.
  - Example: _hoverArray.Update();
  
4. Tweak the settings of the hover array and your vehicle's rigidbody settings to your liking.

Example config:
--Rigidbody--
Mass: 50
Drag: 0.25
Angular Drag: 3
Use Gravity: true

--Hover Array--
Hover Engines:
  Size: 4
  Front, Back, Left, Right (these are just placeholder transforms)
Max Thrust: 2,000
Hover Distance: 1.33
Trace Distance: 2.66 (The max distance of each engine's raycast)
Trace Layer: LevelGeometry (custom layer that holds all the static level geometry)
