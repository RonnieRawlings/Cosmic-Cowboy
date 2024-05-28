using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private bool isPaused = false;
    private AudioSource[] allAudioSources;

    // Start is called before the first frame update
    void Start()
    {
        // Get all AudioSources in the scene
        allAudioSources = FindObjectsOfType<AudioSource>();
    }

    // Method to toggle pause
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause the game
            Time.timeScale = 0f; // Set time scale to 0 to freeze the game
            Debug.Log("Game Paused");

            // Mute all audio sources
            foreach (var audioSource in allAudioSources)
            {
                audioSource.Pause();
            }
        }
        else
        {
            // Unpause the game
            Time.timeScale = 1f; // Set time scale back to 1 for normal speed
            Debug.Log("Game Unpaused");

            // Unmute all audio sources
            foreach (var audioSource in allAudioSources)
            {
                audioSource.UnPause();
            }
        }
    }
}
