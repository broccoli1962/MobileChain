using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource audioSource2;
    [Header ("PanelPopSound")]
    public AudioClip bloom;
    public AudioClip TakeDamage;

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

    public void SoundPlay(string Name, AudioClip clip)
    {
        GameObject gameObject = new GameObject(Name + "Sound");
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(gameObject, clip.length);
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
        audioSource2.PlayOneShot(TakeDamage);
    }
}
