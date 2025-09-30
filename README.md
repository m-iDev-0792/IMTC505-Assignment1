# Assignment1

Explanation about how the code works

## 1 Player control system
### Input reading
Horizontal input is read from the input system (Input.GetAxis("Horizontal") and Input.GetAxis("Vertical")), producing a 2D vector h,v in the XZ plane. Input is sampled in Update() , but physics changes are applied in FixedUpdate() so they stay in sync with Unity’s physics timestep.
### Rigidbody physics
The player uses a Rigidbody. Instead of moving the Transform directly, I change the rigidbody’s velocity or apply forces so physics (gravity, collisions) still work. To decide when the player can jump, a ground check is used. Ground check is small, positioned near the player’s feet, and usually ignores trigger colliders. It prevents double jumps and gives reliable jump timing.

### Jump implementation
When isGrounded is true and the player presses jump (space), the script sets the vertical component of the velocity and generate an instant upward velocity.

## 2 Collectible coins
Coins are cubes with collidors. When the Player touches it, the game score increases, a sound plays, and the coin is removed.
The runtime logic executed when another Collider enters trigger event, it only reacts to objects tagged "Player". Then GameManager handle score increase. At the end of the trigger event, I play sound by calling AudioSource.PlayClipAtPoint and delete the coin from the scene.

## 3 Bomb moving and Bomb damage
### Bomb moving
This function makes the bomb patrol back and forth between two points (pointA and pointB) on the horizontal plane. Each physics step it computes the direction toward the current target, builds a velocity-like move vector (dir.normalized * speed) and advances the rigidbody using Rigidbody.MovePosition(...) so the bomb follows that path at a roughly constant speed.

### Bomb damage
When there is a collision event, the bomb first checks if the other collidor is tagged with "Player". So it only process collisions with the player. If GameManager.Instance exists it calls GameManager.Instance.DamagePlayer(damage) to reduce player lives. Then it get the player Rigidbody via collision.rigidbody. If present it calculates a horizontal push direction dir from the bomb toward the player and builds an impulse vector. To bounce the player away, it applies that impulse with prb.AddForce(impulse, ForceMode.Impulse). (So the force is applied as an impulse and is affected by the player mass.) To indicate that the user has been hit, it tries to flash the player by triggering the DamageFlash component and playing a crash sound.

## 4 Damage flash
When Flash() is called it runs a coroutine for duration seconds that drives a per-frame animated blend between the original color and flashColor.
The animation uses a sine wave (frequency) to produce a repeated “flash/ flicker” intensity (value 0→1→0), and that intensity is used as the lerp mix to compute the tint each frame.
Colors are applied with MaterialPropertyBlock per renderer, so the script doesn’t create new material instances or modify shared materials.
At the end it writes the original colors/emission back to the renderers to restore the look.

## 5 GameManager
This part is relatively simple. Game manager controls the game flow and manages some internal states like score and live. When these states have been changed, game manager also updates UIs to show correct values. The UIs are made with TextMeshPro UI objects which are attached to the UI canvas.