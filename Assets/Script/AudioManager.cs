using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    [Header ("PanelPopSound")]
    public AudioClip bloom;
    public AudioClip TakeDamage;
    public AudioClip clicked;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PopSound()
    {
        audioSource.PlayOneShot(bloom);
    }
    public void TakeSound()
    {
        audioSource.PlayOneShot(TakeDamage);
    }
    public void ClickSound()
    {
        audioSource.PlayOneShot(clicked);
    }
}
