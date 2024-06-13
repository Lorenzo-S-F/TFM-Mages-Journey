using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalAudioHandler : MonoBehaviour
{
    public AudioClip m_SoundReference;

    public void PlaySFX()
    {
        MainManagers.Instance.m_AudioManager.PlaySFXSound(m_SoundReference);
    }

    public void PlayBackground()
    {
        MainManagers.Instance.m_AudioManager.PlayBackgroundSound(m_SoundReference);
    }
}
