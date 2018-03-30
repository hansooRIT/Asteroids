using UnityEngine;
using System.Collections;

/// <summary>
/// Script for the onclick functions of the buttons on the screens.
/// Will send to other scenes onclick.
/// </summary>
public class ScreenButtons : MonoBehaviour {
    
    /// <summary>
    /// Sends user to the Game scene onclick. 
    /// </summary>
	public void LoadGame_EZ()
    {
        Application.LoadLevel("Game_EZ");
    }

    public void LoadGame_Normal()
    {
        Application.LoadLevel("Game_Normal");
    }

    public void LoadGame_Hard()
    {
        Application.LoadLevel("Game_Hard");
    }

    public void LoadDifficulty()
    {
        Application.LoadLevel("DifficultySelect");
    }
    /// <summary>
    /// Sends user to the Title scene onclick.
    /// </summary>
    public void LoadTitle()
    {
        Application.LoadLevel("Title");
    }

    public void LoadInstructions()
    {
        Application.LoadLevel("Instructions");
    }
}
