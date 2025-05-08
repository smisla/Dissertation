using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
public class CatAudioController : MonoBehaviour
{
    public AudioClip[] meowClips; 
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartRandomMeow()
    {
        float randomDelay = Random.Range(0f, 20f);
        Invoke(nameof(PlayMeow), randomDelay);
    }

    void PlayMeow()
    {
        if (meowClips.Length == 0 || audioSource == null) return;

        AudioClip clip = meowClips[Random.Range(0, meowClips.Length)];
        audioSource.PlayOneShot(clip);
    }
}


