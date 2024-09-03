using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audio;
    public AudioClip bloom;

    public static AudioManager caudio;

    private void Start()
    {
        caudio = this;
        audio = GetComponent<AudioSource>();
    }
    public void PopSound()
    {
        audio.PlayOneShot(bloom);
    }
}
