using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    AudioSource aud;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    public void play_sound()
    {
        aud.Play();
    }

}
