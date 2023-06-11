using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public static Audio instance { get; private set; }

    private AudioSource source;

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();

    }

    public void Play(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
    public void Play(AudioClip clip, bool loop)
    {
        source.loop = loop;
        source.PlayOneShot(clip);
    }

    public void Stop(AudioClip clip)
    {
        source.clip = clip;
        source.Stop();
    }
}
