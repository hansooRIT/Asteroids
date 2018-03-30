using UnityEngine;
using System.Collections;

/// <summary>
/// Class for movement of the second order FOEs.
/// </summary>
public class SFOEBehavior : MonoBehaviour {

    //Public float for the foe's movement speed.
    public float speed;

    //Vectors for the movement direction and world position of the FOE.
    private Vector3 foePos;
    public Vector3 direction;

    // Use this for initialization
    void Start()
    {
        foePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Wrap();
        SetTransform();
    }

    /// <summary>
    /// Moves FOE in specified direction at specified speed.
    /// Is framerate independent, thanks to Time.deltaTime.
    /// </summary>
    void Move()
    {
        foePos += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// Wraps FOe around screen if it gets too close to the edge of the screen.
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

    /// <summary>
    /// Sets the world position of the FOE equal to the newly calculated position
    /// </summary>
    void SetTransform()
    {
        transform.position = foePos;
    }
}
