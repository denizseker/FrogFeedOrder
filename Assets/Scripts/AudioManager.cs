using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip clip, Entity entity)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }


}
