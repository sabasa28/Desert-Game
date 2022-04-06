using System;
using UnityEngine;

public class AudioManager : PersistentMonoBehaviourSingleton<AudioManager>
{
    public enum GameplaySFXs
    {
        Step,
        Moo
    }

    public enum Songs
    {
        Pre_Gameplay,
        Gameplay
    }

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicAudioSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] gameplaySFXs;
    [SerializeField] AudioClip[] music;

    [Header("Sound Options")]
    [SerializeField] bool soundOn = true;
    [SerializeField] bool musicOn = true;
    [Space]
    [SerializeField, Range(0.0f, 1.0f)] float soundBaseVolume = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float musicBaseVolume = 1.0f;


    public bool SoundOn { set { soundOn = value; } get { return soundOn; } }
    public bool MusicOn { set { musicOn = value; } get { return musicOn; } }
    public Songs CurrentSong { private set; get; }

    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        musicAudioSource.volume = musicBaseVolume;
    }

    //void UpdateSoundVolume(float volume) { sfxAudioSources.volume = volume * soundBaseVolume; }

    void UpdateMusicVolume(float volume) => musicAudioSource.volume = volume * musicBaseVolume;

    public void PlayGameplaySFX(GameplaySFXs sfx, AudioSource audioSource)
    {
        if (!soundOn) return;

        audioSource.clip = gameplaySFXs[(int)sfx];
        audioSource.Play();
    }

    public void PlayMusic(Songs song)
    {
        musicAudioSource.clip = music[(int)song];
        CurrentSong = song;

        if (musicOn) musicAudioSource.Play();
    }

    public void ToggleSound() => soundOn = !soundOn;

    public void ToggleMusic()
    {
        musicOn = !musicOn;

        if (musicOn) musicAudioSource.Play();
        else musicAudioSource.Stop();
    }
}