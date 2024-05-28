using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnMouseOver : MonoBehaviour
{
    public AudioClip hoverSound; // Assign your sound effect in the Unity Editor
    private AudioSource audioSource;

    void Start()
    {
        // Add an AudioSource component to the GameObject
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = hoverSound;
        audioSource.playOnAwake = false;
    }

    void OnMouseEnter()
    {
        // Play the sound effect when the mouse enters the object
        audioSource.Play();
    }
}