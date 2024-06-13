using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_SFX;
    [SerializeField]
    private AudioSource m_BackgrpundMusic;


    public void PlayBackgroundSound(AudioClip clip)
    {
        m_BackgrpundMusic.clip = clip;
        if (!m_BackgrpundMusic.isPlaying)
            m_BackgrpundMusic.Play();
        m_BackgrpundMusic.loop = true;
    }

    public void PlaySFXSound(AudioClip clip)
    {
        m_SFX.PlayOneShot(clip);
    }

    public IEnumerator FadeBackgroundInto(AudioClip audio, float time)
    {
        float t = 0;
        float currentVolume = m_BackgrpundMusic.volume;
        if (m_BackgrpundMusic.isPlaying)
        {
            while (t < 1)
            {
                t += Time.deltaTime / time;
                m_BackgrpundMusic.volume = currentVolume * (1 - t);
                yield return null;
            }
        }

        t = 0;
        m_BackgrpundMusic.clip = audio;
        m_BackgrpundMusic.volume = 0;

        if (audio == null)
        {
            m_BackgrpundMusic.Stop();
            m_BackgrpundMusic.volume = currentVolume;
            yield break;
        }

        m_BackgrpundMusic.Play();

        while (t < 1)
        {
            t += Time.deltaTime / time;
            m_BackgrpundMusic.volume = currentVolume * t;
            yield return null;
        }
    }

    public void SetSFXVolume(float volume)
    {
        m_SFX.volume = volume;
    }

    public void SetBackgroundVolume(float volume)
    {
        m_BackgrpundMusic.volume = volume;
    }

}
