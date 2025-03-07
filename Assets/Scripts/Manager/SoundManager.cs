using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonDestroy<SoundManager>
{
    private SoundObject BGMAudioObject;
    private float current, percent;

    public AudioClip BGM;

    public void Init()
    {
        ChangeBGM(BGM);
    }

    public void ChangeBGM(AudioClip audioClip)
    {
        if (!BGMAudioObject)
        {
            BGMAudioObject = PlaySound(audioClip, true);
            BGMAudioObject.AudioSource.loop = true;
            BGMAudioObject.AudioSource.pitch = 1.5f;
            BGMAudioObject.name = "BGM Object";

        }
        else
            StartCoroutine("ChangeBGMClip", audioClip);
    }

    public SoundObject PlaySound(AudioClip audioClip, bool imortal = false)
    {
        if (PoolManager.Instance.soundPool.GetPoolObject().TryGetComponent<SoundObject>(out SoundObject soundObject))
        {
            soundObject.Init(audioClip, imortal);
            return soundObject;
        }
        return null;
    }

    IEnumerator ChangeBGMClip(AudioClip newClip)
    {
        current = percent = 0f;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / 1.0f;
            BGMAudioObject.AudioSource.volume = Mathf.Lerp(1f, 0f, percent);
            yield return null;
        }

        BGMAudioObject.AudioSource.clip = newClip;
        BGMAudioObject.AudioSource.Play();
        current = percent = 0f;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / 1.0f;
            BGMAudioObject.AudioSource.volume = Mathf.Lerp(0f, 1f, percent);
            yield return null;
        }

    }
}
