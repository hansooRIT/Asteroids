using UnityEngine;
using System.Collections;

/// <summary>
/// Creates the enemies in the game scene.
/// </summary>
public class InstantiateFOEs : MonoBehaviour {

    //Reference to the enemy prefabs for instantiation;
    public GameObject[] foePrefabs;
    public GameObject bossPrefab;

    public GameObject player;

    //Level integer to determine which enemies to create, and to send to victory screen.
    public int level;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        //Resets level if the current scene is not one of the game scenes.
        if (Application.loadedLevelName != "Game_EZ" && Application.loadedLevelName != "Game_Normal" && Application.loadedLevelName != "Game_Hard")
        {
            level = 1;
        }
        //Else...
        if (Application.loadedLevelName == "Game_EZ" || Application.loadedLevelName == "Game_Normal" || Application.loadedLevelName == "Game_Hard")
        {
            //If there are no enemies on screen
            if (GameObject.FindGameObjectsWithTag("FirstOrderFOE").Length == 0 && GameObject.FindGameObjectsWithTag("SecondOrderFOE").Length == 0 && GameObject.FindGameObjectsWithTag("BossFOE").Length == 0)
            {
                //Spawns boss if the level is right
                if (level == 4)
                {
                    CreateBoss();
                    level += 1;
                }
                //Goes to win screen if you beat the boss
                else if (level > 4)
                {
                    Application.LoadLevel("GameOver_Win");
                }
                else
                {
                    //Or spawns the appropriate amount of enemies, based on difficulty level.
                    if (Application.loadedLevelName == "Game_EZ")
                    {
                        for (int i = 0; i < 1 + level; i++)
                        {
                            CreateFOEs();
                        }
                    }
                    else if (Application.loadedLevelName == "Game_Normal")
                    {
                        for (int i = 0; i < 3 + level; i++)
                        {
                            CreateFOEs();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 5 + level; i++)
                        {
                            CreateFOEs();
                        }
                    }
                    level += 1;
                }
            }
        }
	}

    /// <summary>
    /// Randomly creates FOEs in the screen. They will naturally avoid the center to make sure the user doesn't
    /// get hit as soon as the game starts.
    /// </summary>
    void CreateFOEs()
    {
        //Creates random numbers for FOE type, and position
        int random = Random.Range(0, 3);
        float xPos = Random.Range(-5f, 5f);
        float yPos = Random.Range(-4f, 4f);
        
        //Checking to avoid spawning on top of the player.
        if (Vector3.Distance(player.transform.position, new Vector3(xPos, yPos, 0)) < 1.5f)
        {
            while (Vector3.Distance(player.transform.position, new Vector3(xPos, yPos, 0)) < 1.5f)
            {
                xPos = Random.Range(-5f, 5f);
                yPos = Random.Range(-4f, 4f);
            }
        }
        //Then creates FOE when the position is valid.
        Instantiate(foePrefabs[random], new Vector3(xPos, yPos, 0), Quaternion.identity);
    }

    /// <summary>
    /// Creates the big boss FOE.
    /// Will always have a static position when spawned.
    /// </summary>
    void CreateBoss()
    {
        Instantiate(bossPrefab, new Vector3(0, 2.5f, 0), Quaternion.identity);
    }
}
