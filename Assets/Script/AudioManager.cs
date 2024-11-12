using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    [Header ("PanelPopSound")]
    public AudioClip bloom;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PopSound()
    {
        audioSource.PlayOneShot(bloom);
    }
}
