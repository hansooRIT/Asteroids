using UnityEngine;
using System.Collections;

/// <summary>
/// Class to instantiate user created bullets.
/// </summary>
public class ShootBullet : MonoBehaviour {

    //Reference to bullet prefab for instantiation
    public GameObject bulletPrefab;

    //Player movement mode, which determines amount of bullets fired.
    private int playerMode;

    //Timer variable to ensure that rapid fire isn't possible
    private float timeLock;

	// Use this for initialization
	void Start () {
        timeLock = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
        playerMode = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CollisionDetection>().playerMode;
	    if (Input.GetKey(KeyCode.Space))
        {
            Shoot();
        }
        timeLock += Time.deltaTime;
	}

    /// <summary>
    /// Creates a bullet. Depending on the player's current mode, you will either have a single
    /// shot that recharges quickly, or a cluster of bullets with a slower recharge rate.
    /// </summary>
    void Shoot()
    {
        //Checks player mode
        if (playerMode == 0)
        {
            //Then sees if the bullet has recharged since the last one was fired.
            if (timeLock >= 0.5)
            {
                //If it was, fire a new bullet, and start recharging.
                Instantiate(bulletPrefab, gameObject.transform.position, gameObject.transform.rotation);
                timeLock = 0;
            }
        }
        //If the player is in focused mode
        else
        {
            //There's a longer recharge rate
            if (timeLock >= 1.25)
            {
                //And more bullets are fired
                Instantiate(bulletPrefab, gameObject.transform.position, gameObject.transform.rotation);
                Instantiate(bulletPrefab, gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, 5));
                Instantiate(bulletPrefab, gameObject.transform.position, gameObject.transform.rotation * Quaternion.Euler(0, 0, -5));
                timeLock = 0;
            }
        }
    }
}
