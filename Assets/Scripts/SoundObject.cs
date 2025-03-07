using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : PoolObject
{
    private AudioSource audioSource;
    public AudioSource AudioSource => audioSource;

    private void Awake()
    {
        TryGetComponent<AudioSource>(out audioSource);
    }

    public void Init(AudioClip clip, bool imortal)
    {
        audioSource.clip = clip;
        audioSource.Play();
        if(!imortal) Invoke("ReturnToPool", audioSource.clip.length);
    }
}
