using UnityEngine;
using System.Collections;

/// <summary>
/// Class for moving the player character, as well as augmenting certain facets of the player sprite for certain key presses.
/// </summary>
public class MoveVehicle : MonoBehaviour {
	//Vector used to calculate movement for vehicle.
	private Vector3 vehiclePos;
	private Vector3 velocity;
	private Vector3 direction;
	private Vector3 acceleration;
    private float accelerationRate = 0.1f;
	private Quaternion angleToRotate;

	//Rate of movement for vehicle
	public float speed;
	public float maxSpeed;
    public float focusedSpeed;

    //Float to check the current rotation about of the vehicle. Used for drawing rotated sprite to screen.
	private float rotation;

    //References to children of the player object. Used for displaying the appropriate sprites at specified times.
    public GameObject focusedHitbox;
    public GameObject shield;

    //Timer for the player's shield to disappear once a certain amount of time passes.
    public float shieldTimer;

    //Player's current movement mode. Used for determining whether or not to show the hitbox.
    private int mode;

	// Use this for initialization
	void Start () {
		vehiclePos = new Vector3 (0, 0, 0);
		direction = Vector3.up;
		velocity = Vector3.zero;

		angleToRotate = Quaternion.Euler (0, 0, 0);

		acceleration = new Vector3 (0, 0, 0);
		accelerationRate = 0.01f;
        //Sets initial mode to unfocused.
        mode = 0;
        //And shields and hitbox to inactive.
        shield.GetComponent<SpriteRenderer>().enabled = false;
        focusedHitbox.GetComponent<SpriteRenderer>().enabled = false;
        shieldTimer = 0;
	}
	
	// Update is called once per frame
	void Update () {
        ModeSwitch();
        if (mode == 0)
        {
            RotateVehicle();
            Drive();
        }
        else
        {
            FocusedMovement();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Shield();
        }
        shieldTimer += Time.deltaTime;
        //Once a certain amount of time passes after a shield is activated,
        if (shieldTimer >= 1.5)
        {
            //The shield will disappear.
            shield.GetComponent<SpriteRenderer>().enabled = false;
        }
		SetTransform ();
        Wrap();
	}

	/// <summary>
	/// Rotates the vehicle.
	/// Change the vehicle's direction when keys are pressed
	/// </summary>
	void RotateVehicle() {
		// Press J - Rotate 1 degree to the right
		//Press K - Rotate 1 degree to the left;
		if (Input.GetKey(KeyCode.RightArrow)) {
			angleToRotate = Quaternion.Euler (0, 0, -2);
			direction = angleToRotate * direction;
			rotation -= 2f;
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			angleToRotate = Quaternion.Euler (0, 0, 2);
			direction = angleToRotate * direction;
			rotation += 2f;
		} else {
			angleToRotate = Quaternion.Euler (0, 0, 0);
			direction = angleToRotate * direction;
		}
	}

	/// <summary>
	/// Wrap this instance.
	/// Keep this vehicle on screen at all times.
	/// Recalculates vehicle's position based on the edges of the screen.
	/// </summary>
	void Wrap() {
        if (vehiclePos.x > 7)
        {
            vehiclePos.x = -7;
        } else if (vehiclePos.x < -7)
        {
            vehiclePos.x = 7;
        }
        if (vehiclePos.y > 5)
        {
            vehiclePos.y = -5;
        }
        else if (vehiclePos.y < -5)
        {
            vehiclePos.y = 5;
        }
	}

	/// <summary>
	/// Drive this instance.
	/// Calculates this vehicle's velocity based upon acceleration and deceleration.
	/// Updates the position of the vehicle based on velocity vector.
	/// </summary>
	void Drive() {
		//Adds speed to x and y coordinates of the vehicle.
		//Movement is independent of time.
        if (Input.GetKey(KeyCode.UpArrow))
        {
            acceleration = direction * accelerationRate;
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            //velocity = direction * speed * Time.deltaTime;
        }
        //For backwards movement. Utilizes nearly the same code as abouve, but with a negative direction.
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            acceleration = -direction * accelerationRate;
            velocity += acceleration;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            //velocity = direction * speed * Time.deltaTime;
        }
        else
        {
            velocity.x = velocity.x * .95f;
            velocity.y = velocity.y * .95f;
            if (velocity.magnitude <= 0.01)
            {
                velocity = Vector3.zero;
            }
        }
        vehiclePos += velocity;
    }

    /// <summary>
    /// Moves the player character in direction specified by the arrow keys.
    /// Moves slower than the drive method, and does not take acceleration into account.
    /// </summary>
    void FocusedMovement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            vehiclePos.y += focusedSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            vehiclePos.x += focusedSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            vehiclePos.x -= focusedSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            vehiclePos.y -= focusedSpeed * Time.deltaTime;
        }
    }

	/// <summary>
	/// Sets the transform.
	/// Updates position and rotation of vehicle.
	/// </summary>
	void SetTransform() {
		transform.rotation = Quaternion.Euler (0, 0, rotation);
		transform.position = vehiclePos;
	}

    /// <summary>
    /// Switches mode upon clicking a shift key.
    /// Will result in the hitbox sprite showing or not showing,
    /// as well as determining the movement method.
    /// </summary>
    void ModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (mode == 0)
            {
                focusedHitbox.GetComponent<SpriteRenderer>().enabled = true;
                mode = 1;
            }
            else
            {
                focusedHitbox.GetComponent<SpriteRenderer>().enabled = false;
                mode = 0;
                velocity = Vector3.zero;
            }
            
        }
    }

    /// <summary>
    /// Displays the shield when the user presses the z key.
    /// The sprite's opacity is set to half, to the player could
    /// see the others sprites past it.
    /// </summary>
    void Shield()
    {
        shieldTimer = 0;
        shield.GetComponent<SpriteRenderer>().enabled = true;
        Color shieldColor = shield.GetComponent<SpriteRenderer>().color;
        shieldColor.a = 0.5f;
        shield.GetComponent<SpriteRenderer>().color = shieldColor;
    }

}
