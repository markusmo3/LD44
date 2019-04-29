using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class MusicController : MonoBehaviour {
    public AudioMixerSnapshot outOfCombat;
    public AudioMixerSnapshot inCombat;
    public AudioClip[] musics;
    public float bpm = 128;
    public bool playNext;

    private float m_TransitionIn;
    private float m_TransitionOut;
    private float m_QuarterNote;
    private AudioSource audioSource;

    private int _curMusicIndex;
    public int curMusicIndex {
        get { return _curMusicIndex; }
        set {
            _curMusicIndex = value;
            audioSource.clip = musics[curMusicIndex];
            audioSource.Play();
        }
    }

    // Use this for initialization
    void Start() {
        audioSource = Camera.main.GetComponent<AudioSource>();
        m_QuarterNote = 60 / bpm;
        m_TransitionIn = m_QuarterNote;
        m_TransitionOut = m_QuarterNote * 32;
        // start random
        _curMusicIndex = Random.Range(0, musics.Length);
    }

    private void Update() {
        if (!audioSource.isPlaying || playNext) {
            PlayNextTrack();
            playNext = false;
        }
    }

    public void OnEnter() {
        inCombat.TransitionTo(0.1f);
    }

    public void OnExit() {
        outOfCombat.TransitionTo(0.1f);
    }

    public void PlayNextTrack() {
        curMusicIndex = (curMusicIndex+1) % musics.Length;
    }
}