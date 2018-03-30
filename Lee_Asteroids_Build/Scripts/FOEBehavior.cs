using UnityEngine;
using System.Collections;

/// <summary>
/// Class for the FOE's movement
/// </summary>
public class FOEBehavior : MonoBehaviour {

    //Float for the FOE's movement speed.
    public float speed;

    //Vectors for the FOE's world position and direction in which it's moving.
    private Vector3 foePos;
    public Vector3 direction;

	// Use this for initialization
	void Start () {
        foePos = transform.position;
        direction = (Random.rotation * Vector3.up).normalized;
        direction.z = 0;
	}

    // Update is called once per frame
    void Update()
    {
        Move();
        Wrap();
        SetTransform();
    }

    /// <summary>
    /// Moves the FOE in the specified direction at the specified speed.
    /// Is framerate independent thanks to Time.deltaTime;
    /// </summary>
    void Move()
    {
        foePos += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// Wraps the FOE around the screen if it gets too close to the borders.
    /// </summary>
    void Wrap()
    {
        if (foePos.x > 7)
        {
            foePos.x = -7;
        }
        else if (foePos.x < -7)
        {
            foePos.x = 7;
        }
        if (foePos.y > 5)
        {
            foePos.y = -5;
        }
        else if (foePos.y < -5)
        {
            foePos.y = 5;
        }
    }

    //Sets the FOE's world position to the new position that was calculated.
    void SetTransform()
    {
        transform.position = foePos;
    }
}
