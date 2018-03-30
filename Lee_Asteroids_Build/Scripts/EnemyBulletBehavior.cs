using UnityEngine;
using System.Collections;

/// <summary>
/// Class for the enemy created bullets.
/// </summary>
public class EnemyBulletBehavior : MonoBehaviour {

    //Variable for bullet's travel speed.
    public float speed;

    private bool directionSet;

    //Float to limit the amount of distance a bullet can travel. If it goes too far, it is destroyed
    private float distanceLimit;

    private Vector3 bulletPos;
    private Vector3 direction;

    // Use this for initialization
    void Start () {
        direction = (transform.rotation * Vector3.down).normalized;
        bulletPos = transform.position;
        distanceLimit = 0;
        directionSet = false;
    }
	
	// Update is called once per frame
	void Update () {
        Move();
        SetTransform();
	    if (gameObject.tag == "WallBullet")
        {
            if (distanceLimit >= 4 && directionSet == false)
            {
                SetWallDirection();
                directionSet = true;
            }
        }
        if (gameObject.tag == "HomingBullet" && directionSet == false)
        {
            if (distanceLimit >= 3)
            {
                SetHomingDirection();
                directionSet = true;
            }
        }
	}


    void SetHomingDirection()
    {
        direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
    }

    void SetWallDirection()
    {
        direction = Vector3.down;
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
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z);
    }
}
