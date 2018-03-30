using UnityEngine;
using System.Collections;

/// <summary>
/// Main manager class for the collision, UI, instantiation, and power ups.
/// </summary>
public class CollisionDetection : MonoBehaviour {

    //Integer for keep track of player state for collisions
    public int playerMode;

    //GameObjects in scene to calculate collision
    public GameObject player;
    private GameObject[] bullets;
    private GameObject[] enemies;
    private GameObject[] secondOrderEnemies;
    private GameObject bossFOE;
    private GameObject[] normalBossBullets;
    private GameObject[] homingBossBullets;
    private GameObject[] wallBossBullets;

    //Reference to second Order FOE prefabs for instantiation once a first order FOE hits a bullet. 
    public GameObject[] secondOrderPrefabs;

    //GUI Text to communicate current score.
    public UnityEngine.UI.Text scoreText;

    //GUI Images for life and bomb count
    public UnityEngine.UI.Image[] lifeIcons;
    public UnityEngine.UI.Image[] bombIcons;

    //GUI slide for the boss's health bar.
    public UnityEngine.UI.Slider bossHealthBar;
 
    //Number of lives and bomb player has left.
    public int lives;
    public int bombs;

    //Variables to determine whether or not the shield is active, and when it's about to run out.
    private bool isShielded;
    private float shieldTimer;

    //Integer for the boss's current health.
    public int bossHealth;

    //The player's score.
    public int score;

    //Timer for brief invincibility window after player gets hit.
    private float invulnTimer;

    //Timer for player's chance to press the 'z' key to negate damage from collision, and destroy everything in the area.
    private float collisionShieldTimer;

    // Use this for initialization
    void Start() {
        score = 0;
        playerMode = 0;
        bossHealthBar.enabled = false;
        shieldTimer = 0;
        isShielded = false;

    }

    // Update is called once per frame
    void Update() {
        //If the current scene is the Game scene.
        if (Application.loadedLevelName == "Game_EZ" || Application.loadedLevelName == "Game_Normal" || Application.loadedLevelName == "Game_Hard")
        {
            //Mode changing statement.
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                if (playerMode == 0)
                {
                    playerMode = 1;
                }
                else
                {
                    playerMode = 0;
                }
            }
            //Creates arrays of all bullets and enemies in scene for collision checking.
            bullets = GameObject.FindGameObjectsWithTag("Bullet");
            enemies = GameObject.FindGameObjectsWithTag("FirstOrderFOE");
            secondOrderEnemies = GameObject.FindGameObjectsWithTag("SecondOrderFOE");

            //Adds timer to invulnerability variable, so invulnerability after getting hit wears off.
            invulnTimer += Time.deltaTime;
            
            //Only checks for collisions if player isn't shielded.
            if (!isShielded)
            {
                //Player colliding with enemy.
                PlayerEnemyCollision();
            }
            //Checks for bullet collisions with enemy
            BulletEnemyCollision();

            //Updates the player's score.
            scoreText.text = "Score: " + score;

            //If the boss currently exists in the scene.
            if (GameObject.FindGameObjectsWithTag("BossFOE").Length > 0)
            {
                //Show the boss's health bar
                bossHealthBar.enabled = true;

                //And get references to the boss and his bullets.
                bossFOE = GameObject.FindGameObjectWithTag("BossFOE");
                normalBossBullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
                homingBossBullets = GameObject.FindGameObjectsWithTag("HomingBullet");
                wallBossBullets = GameObject.FindGameObjectsWithTag("WallBullet");
                
                //Only checks collision if user isn't shielded
                if (!isShielded)
                {
                    BulletPlayerCollision();
                    BulletBossCollision();
                }
            }
            else
            {
                bossHealthBar.enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.Z) && bombs > 0)
            {
                Shield();
            }
            if (lives == 0)
            {
                Application.LoadLevel("GameOver_Loss");
            }
            shieldTimer += Time.deltaTime;
            if (shieldTimer >= 1.5)
            {
                isShielded = false;
            }
        }
        if (Application.loadedLevelName == "GameOver_Loss")
        {
            score = 0;
        }
    }

    /// <summary>
    /// AABBCollision detection between two GameObjects
    /// </summary>
    /// <param name="a">The spaceShip GameObject</param>
    /// <param name="b">One of the planet GameObjects</param>
    /// <returns></returns>
    bool AABBCollision(GameObject a, GameObject b)
    {
        //Gets bounds of both objects.
        Bounds aBounds = a.gameObject.GetComponent<SpriteRenderer>().bounds;
        Bounds bBounds = b.gameObject.GetComponent<SpriteRenderer>().bounds;
        //Variables to check for potential collisions to determine whether or not we need to check for collision.
        bool horizontalOverlap = false;
        bool verticalOverlap = false;
        //Calculations for checking if x or y bounds are intersecting.
        if (bBounds.center.x + bBounds.min.x < aBounds.center.x + aBounds.max.x || bBounds.center.x + bBounds.max.x > aBounds.center.x + aBounds.min.x)
        {
            horizontalOverlap = true;
        }
        if (bBounds.center.y + bBounds.max.y > aBounds.center.y + aBounds.min.y || bBounds.center.y + bBounds.min.y < aBounds.center.y + aBounds.max.y)
        {
            verticalOverlap = true;
        }
        //If there are potential collisions...
        if (horizontalOverlap || verticalOverlap)
        {
            //Check if the bounds are intersecting
            if (aBounds.Intersects(bBounds))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// CircleCollision collision detection between 2 game objects.
    /// </summary>
    /// <param name="a">spaceShip GameObject</param>
    /// <param name="b">A planet GameObject</param>
    /// <returns></returns>
    bool CircleCollision(GameObject a, GameObject b)
    {
        //Gets the centers of each object
        Vector2 aCenter = a.GetComponent<SpriteRenderer>().bounds.center;
        Vector2 bCenter = b.GetComponent<SpriteRenderer>().bounds.center;
        //Calculates both of their radii.
        float aRadius = a.gameObject.GetComponent<SpriteRenderer>().bounds.max.y - aCenter.y;
        float bRadius = b.gameObject.GetComponent<SpriteRenderer>().bounds.max.y - bCenter.y;
        //Then calculates the distance between the two objects.
        float dist = Mathf.Abs((aCenter - bCenter).magnitude);
        //If the distance is greater than the sum of the radii, they're not intersecting.
        if (dist > aRadius + bRadius)
        {
            return false;
        }
        return true;
    }

    void PlayerEnemyCollision()
    {
        //If there are enemies in the scene.
        if (enemies.Length > 0)
        {
            foreach (GameObject enemy in enemies)
            {
                //If the player is in unfocused mode...
                if (playerMode == 0)
                {
                    //Do AABBCollision between player and enemy, assuming the invulnerability window is done.
                    if (AABBCollision(player, enemy) && invulnTimer > 1.5)
                    {
                        //Brief time window when the player is hit for them to shield themselves last minute, at the cost of 2 bombs.
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                            if (lives == 0)
                            {
                                Application.LoadLevel("GameOver_Loss");
                            }
                        }
                    }
                }
                else
                {
                    //For focused movement, checks circleCollision between the smaller hitbox and the enemy.
                    if (CircleCollision(player.transform.GetChild(0).gameObject, enemy) && invulnTimer > 1.5)
                    {
                        //Same code as above, otherwise.
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                        }
                    }
                }
            }
        }
        //Same procedure as first order enemies, only with second order enemies.
       if (secondOrderEnemies.Length > 0)
        {
            foreach (GameObject enemy in secondOrderEnemies)
            {
                if (playerMode == 0)
                {
                    if (AABBCollision(player, enemy) && invulnTimer > 1.5)
                    {
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                        }
                    }
                }
                else
                {
                    if (CircleCollision(player.transform.GetChild(0).gameObject, enemy) && invulnTimer > 1.5)
                    {
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                        }
                    }
                }
            }
        }
       //And again for the boss FOE.
       if (bossFOE != null)
        {
            if (playerMode == 0)
            {
                if (AABBCollision(player, bossFOE) && invulnTimer > 1.5)
                {
                    if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                    {
                        Shield();
                        collisionShieldTimer = 0;
                        invulnTimer = 0.5f;
                    }
                    collisionShieldTimer += Time.deltaTime;
                    if (collisionShieldTimer > 0.5)
                    {
                        lives -= 1;
                        lifeIcons[lives].enabled = false;
                        collisionShieldTimer = 0;
                        invulnTimer = 0;
                        if (lives == 0)
                        {
                            Application.LoadLevel("GameOver_Loss");
                        }
                    }
                }
            }
            else
            {
                if (CircleCollision(player.transform.GetChild(0).gameObject, bossFOE) && invulnTimer > 1.5)
                {
                    if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                    {
                        Shield();
                        collisionShieldTimer = 0;
                        invulnTimer = 0.5f;
                    }
                    collisionShieldTimer += Time.deltaTime;
                    if (collisionShieldTimer > 0.5)
                    {
                        lives -= 1;
                        lifeIcons[lives].enabled = false;
                        collisionShieldTimer = 0;
                        invulnTimer = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Class for the player's bullets colliding with the enemy.
    /// </summary>
    void BulletEnemyCollision()
    {
        //Loops through each bullet and first order enemy
        foreach (GameObject bullet in bullets) {
            foreach (GameObject enemy in enemies)
            {
                //If they collide...
                if (CircleCollision(bullet, enemy))
                {

                    //Second order FOEs are created, with approximately the same diretion.
                    //Type of FOE is randomly determined, and amount is based on difficulty.
                    int random1 = Random.Range(0, 3);
                    int random2 = Random.Range(0, 3);
                    GameObject firstChild = (GameObject) Instantiate(secondOrderPrefabs[random1], enemy.transform.position, Quaternion.identity);
                    SFOEBehavior firstChildBehavior = firstChild.GetComponent<SFOEBehavior>();
                    firstChildBehavior.direction = Quaternion.Euler(0, 0, (float)Random.Range(-15, 15)) * enemy.GetComponent<FOEBehavior>().direction;
                    GameObject secondChild = (GameObject)Instantiate(secondOrderPrefabs[random2], enemy.transform.position, Quaternion.identity);
                    SFOEBehavior secondChildBehavior = secondChild.GetComponent<SFOEBehavior>();
                    secondChildBehavior.direction = Quaternion.Euler(0, 0, (float)Random.Range(-15, 15)) * enemy.GetComponent<FOEBehavior>().direction;
                    if (Application.loadedLevelName == "Game_Normal")
                    {
                        int random3 = Random.Range(0, 3);
                        GameObject thirdChild = (GameObject)Instantiate(secondOrderPrefabs[random3], enemy.transform.position, Quaternion.identity);
                        SFOEBehavior thirdChildBehavior = thirdChild.GetComponent<SFOEBehavior>();
                        thirdChildBehavior.direction = Quaternion.Euler(0, 0, (float)Random.Range(-15, 15)) * enemy.GetComponent<FOEBehavior>().direction;

                    }
                    else if (Application.loadedLevelName == "Game_Hard")
                    {
                        int random3 = Random.Range(0, 3);
                        GameObject thirdChild = (GameObject)Instantiate(secondOrderPrefabs[random3], enemy.transform.position, Quaternion.identity);
                        SFOEBehavior thirdChildBehavior = thirdChild.GetComponent<SFOEBehavior>();
                        thirdChildBehavior.direction = Quaternion.Euler(0, 0, (float)Random.Range(-15, 15)) * enemy.GetComponent<FOEBehavior>().direction;
                        int random4 = Random.Range(0, 2);
                        GameObject fourthChild = (GameObject)Instantiate(secondOrderPrefabs[random4], enemy.transform.position, Quaternion.identity);
                        SFOEBehavior fourthChildBehavior = fourthChild.GetComponent<SFOEBehavior>();
                        fourthChildBehavior.direction = Quaternion.Euler(0, 0, (float)Random.Range(-15, 15)) * enemy.GetComponent<FOEBehavior>().direction;
                    }
                    //The player earns some points
                    score += 20;
                    //Then both the first order FOE and bullet are destroyed.
                    Destroy(enemy);
                    Destroy(bullet);
                }
            }
            //For second order enemies...
            foreach (GameObject secondOrderEnemy in secondOrderEnemies)
            {
                if (CircleCollision(bullet, secondOrderEnemy))
                {
                    //They are simply destroyed, and the player gets more points.
                    score += 50;
                    Destroy(secondOrderEnemy);
                    Destroy(bullet);
                }
            }
        }
    }

    /// <summary>
    /// Checks if the enemy bullets collide with the player, and deals damage accordingly
    /// </summary>
    void BulletPlayerCollision()
    {
        //If there are any of the boss's normal bullets on screen.
        if (normalBossBullets.Length > 0)
        {
            //Loop through all of them...
            foreach (GameObject bossBullet in normalBossBullets)
            {
                //And checks collisions identically to the player/enemy collision check.
                if (playerMode == 0)
                {
                    if (AABBCollision(player, bossBullet) && invulnTimer > 1.5)
                    {
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0 && bombs > 0 && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                            if (lives == 0)
                            {
                                Application.LoadLevel("GameOver_Loss");
                            }
                        }
                    }
                }
                else
                {
                    if (CircleCollision(player.transform.GetChild(0).gameObject, bossBullet) && invulnTimer > 1.5)
                    {
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                        }
                    }
                }
            }
        }
        //Same thing for the homing bullets
        if (homingBossBullets.Length > 0)
        {
            foreach (GameObject bossBullet in homingBossBullets)
            {
                if (playerMode == 0)
                {
                    if (AABBCollision(player, bossBullet) && invulnTimer > 1.5)
                    {
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                            if (lives == 0)
                            {
                                Application.LoadLevel("GameOver_Loss");
                            }
                        }
                    }
                }
                else
                {
                    if (CircleCollision(player.transform.GetChild(0).gameObject, bossBullet) && invulnTimer > 1.5)
                    {
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                        }
                    }
                }
            }
        }
        //The wall bullets are a special case.
        if (wallBossBullets.Length > 0)
        {
            foreach (GameObject bossBullet in wallBossBullets)
            {
                if (playerMode == 0)
                {
                    if (AABBCollision(player, bossBullet) && invulnTimer > 1.5)
                    {
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                            if (lives == 0)
                            {
                                Application.LoadLevel("GameOver_Loss");
                            }
                        }
                    }
                }
                //The wall bullets are a special case due to their oblong shape. AABB collision will be used, for more accuracy.
                else
                {
                    if (AABBCollision(player.transform.GetChild(0).gameObject, bossBullet) && invulnTimer > 1.5)
                    {
                        if (collisionShieldTimer < 0.5 && Input.GetKeyDown(KeyCode.Z) && bombs > 0)
                        {
                            Shield();
                            collisionShieldTimer = 0;
                            invulnTimer = 0.5f;
                        }
                        collisionShieldTimer += Time.deltaTime;
                        if (collisionShieldTimer > 0.5)
                        {
                            lives -= 1;
                            lifeIcons[lives].enabled = false;
                            collisionShieldTimer = 0;
                            invulnTimer = 0;
                        }
                    }
                }
            }
        }
    }

    //Checks for player bullets colliding with the boss.
    void BulletBossCollision()
    {
        //Loops through each bullet
        foreach (GameObject bullet in bullets)
        {
            //And if they collide.
            if (CircleCollision(bullet, bossFOE))
            {
                if (Application.loadedLevelName == "Game_EZ")
                {
                    bossHealth -= 10;
                    Destroy(bullet);
                }
                else if (Application.loadedLevelName == "Game_Normal")
                {
                    bossHealth -= 5;
                    Destroy(bullet);
                }
                else
                {
                    bossHealth -= 2;
                    Destroy(bullet);
                }
                //The boss takes damage, and the bullet is destroyed.
                
            }
            //And deletes the boss if it's health is below 0.
            if (bossHealth <= 0)
            {
                Destroy(bossFOE);
            }
        }
    }

    /// <summary>
    /// Creates a shield for the player at the cost of a resource.
    /// </summary>
    void Shield()
    {
        isShielded = true;
        shieldTimer = 0;
        bombs -= 1;
        bombIcons[bombs].enabled = false;
    }
}
