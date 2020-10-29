using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public SoundManager soundManager;

    private AudioMixerGroup pitchBend;
    private AudioSource musicSource;
    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        musicSource = GetComponent<AudioSource>();
        pitchBend = Resources.Load<AudioMixerGroup>("PitchMixer");
        musicSource.outputAudioMixerGroup = pitchBend;
    }

    public IEnumerator ChangeMusicTempo()
    {
        float prevTempo = 1f;
        while (true)
        {
            float tempo = 0.982f + GameManager.manager.levelManager.enemyHolder.speed / GameManager.manager.levelManager.enemyHolder.maxSpeed;
            if (tempo - prevTempo > 0.02f)
            {
                musicSource.pitch = tempo; //Aluksi 1, lopuksi 2
                pitchBend.audioMixer.SetFloat("pitchBend", 1 / tempo);
                prevTempo = tempo;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
