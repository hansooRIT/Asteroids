using UnityEngine;
using System.Collections;

/// <summary>
/// Class for the movement of the player created bullet
/// </summary>
public class BulletBehavior : MonoBehaviour {

    //Variable for bullet's travel speed.
    public float speed;

    //Float to limit the amount of distance a bullet can travel. If it goes too far, it is destroyed
    private float distanceLimit;

    //Variables for the bullet's world position, and the direction in which it is moving.
    private Vector3 bulletPos;
    private Vector3 direction;

	// Use this for initialization
	void Start () {
        direction = (transform.rotation * Vector3.up).normalized;
        bulletPos = transform.position;
        distanceLimit = 0;
	}
	
	// Update is called once per frame
	void Update () {
        Move();
        SetTransform();
	}

    /// <summary>
    /// Moves the bullet in the specified direction at the specified speed.
    /// Is framerate indendent.
    /// Also destroys the bullet if it moved too much without colliding with anything.
    /// </summary>
    void Move()
    {
        bulletPos += direction * speed * Time.deltaTime;
        distanceLimit += speed * Time.deltaTime;
        if (distanceLimit >= 20)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Wraps the bullet to the other side of the screen if it goes off screen.
    /// </summary>
    /// <summary>
    /// Sets the transform of the bullet object to be equal to the new position that was calculated.
    /// </summary>
    void SetTransform()
    {
        transform.position = bulletPos;
    }
}
