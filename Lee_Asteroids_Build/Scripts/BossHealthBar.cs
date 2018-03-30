using UnityEngine;
using System.Collections;

/// <summary>
/// Script for the slider for the boss FOE's health bar.
/// </summary>
public class BossHealthBar : MonoBehaviour {

    //Reference to the health bar slider in the UI.
    public UnityEngine.UI.Slider slider;

    //Reference to the GameManager. Used to reference CollisionDetection.cs,
    //which contains the boss's current health.
    private GameObject manager;

	// Use this for initialization
	void Start () {
        manager = GameObject.FindGameObjectWithTag("GameManager");
	}
	
	// Update is called once per frame
	void Update () {
        slider.value = (float) manager.GetComponent<CollisionDetection>().bossHealth;
	}
}
