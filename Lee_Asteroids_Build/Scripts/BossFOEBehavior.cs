using UnityEngine;
using System.Collections;

/// <summary>
/// Class for the behavior of the big boss FOE
/// </summary>
public class BossFOEBehavior : MonoBehaviour {

    //Float for the FOE's movement speed.
    public float speed;

    //References to the different bullet types the FOE can shoot.
    public GameObject[] bullets;

    //Vectors for the FOE's direction movement and current world position.
    private Vector3 direction;
    private Vector3 foePos;

    //Timer float for determining when the boss should shoot.
    private float attackTimer;

	// Use this for initialization
	void Start () {
        foePos = transform.position;
        direction = Vector3.right;
        attackTimer = 0;
        //Difficulty determines boss's movement speed
        if (Application.loadedLevelName == "Game_EZ")
        {
            speed = 1;
        }
        else if (Application.loadedLevelName == "Game_Normal")
        {
            speed = 2;
        }
        else
        {
            speed = 3;
        }
    }
	
	// Update is called once per frame
	void Update () {
        Move();
        SetTransform();
        //Timer for the boss's attacks. Attack frequency is determined by difficulty.
        if (Application.loadedLevelName == "Game_EZ")
        {
            if (attackTimer >= 2)
            {
                Attack();
                attackTimer = 0;
            }
        }
        else if (Application.loadedLevelName == "Game_Normal")
        {
            if (attackTimer >= 1.5)
            {
                Attack();
                attackTimer = 0;
            }
        }
        else
        {
            if (attackTimer >= 1)
            {
                Attack();
                attackTimer = 0;
            }
        }
        attackTimer += Time.deltaTime;
	}

    /// <summary>
    /// Moves the boss FOE left to right.
    /// If it gets too close to the end of the screen, the direction reverses.
    /// </summary>
    void Move()
    {
        foePos += direction * speed * Time.deltaTime;
        if (foePos.x < -4.5)
        {
            direction = Vector3.right;
        }
        if (foePos.x > 4.5)
        {
            direction = Vector3.left;
        }
    }

    /// <summary>
    /// Sets the boss's world position to the newly calculated position.
    /// </summary>
    void SetTransform()
    {
        transform.position = foePos;
    }

    /// <summary>
    /// Creates the bullets to attack the player with.
    /// </summary>
    void Attack()
    {
        //First, creates a random number to determine which type of bullet to shoot.
        int random = Random.Range(0, 3);
        //Depending on the number, a different bullet will be shot in a fan pattern, dictated by the different quaternions.
        if (random == 0)
        {
            Instantiate(bullets[0], gameObject.transform.position, gameObject.transform.rotation);
            Instantiate(bullets[0], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -45));
            Instantiate(bullets[0], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -30));
            Instantiate(bullets[0], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -15));
            Instantiate(bullets[0], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 15));
            Instantiate(bullets[0], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 30));
            Instantiate(bullets[0], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 45));
        }
        else if (random == 1)
        {
            Instantiate(bullets[1], gameObject.transform.position, gameObject.transform.rotation);
            Instantiate(bullets[1], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -45));
            Instantiate(bullets[1], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -30));
            Instantiate(bullets[1], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -15));
            Instantiate(bullets[1], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 15));
            Instantiate(bullets[1], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 30));
            Instantiate(bullets[1], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 45));
        }
        else
        {
            Instantiate(bullets[2], gameObject.transform.position, gameObject.transform.rotation);
            Instantiate(bullets[2], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -45));
            Instantiate(bullets[2], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -30));
            Instantiate(bullets[2], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -15));
            Instantiate(bullets[2], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 15));
            Instantiate(bullets[2], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 30));
            Instantiate(bullets[2], gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 45));
        }
    }
}
