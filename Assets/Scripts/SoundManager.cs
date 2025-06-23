using System.Collections;
using UnityEngine;

public enum SOUNDTYPE { indoor, outdoor }

[System.Serializable]
public class AudioLabel
{
    public SOUNDTYPE soundType;
    public AudioSource audioSource;
    public float initialVolume;
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] bandMusic; // Array of music clips
    [SerializeField] private float loopIntervalTime = 2.0f; // Time between music loop iterations
    [SerializeField] private AudioSource indoorBandMusic; // AudioSource for indoor music
    [SerializeField] private AudioSource outdoorBandMusic; // AudioSource for outdoor music
    [SerializeField] private AudioLabel[] audioLabels; // Array for managing background music

    private readonly float audioFadeDuration = 1.0f; // Duration for audio fade in/out
    private int musicIndex = 0; // Current index of the music being played

    private void Start()
    {
        // Initialize initial volumes for all background music
        foreach (var music in audioLabels)
        {
            if (music.audioSource != null)
                music.initialVolume = music.audioSource.volume;
        }

        // Set initial audio type
        SetAudio(SOUNDTYPE.outdoor);

        // Start the music loop
        if (bandMusic != null && bandMusic.Length > 0)
            PlayMusicLoop();
        else
            Debug.LogWarning("No band music clips assigned!");
    }

    public void SetAudio(SOUNDTYPE _soundType)
    {
        // Switch audio levels based on the given sound type
        foreach (var music in audioLabels)
        {
            if (music.audioSource != null)
            {
                if (music.soundType == _soundType)
                    StartCoroutine(AudioSwitcher(music.audioSource, music.initialVolume));
                else
                    StartCoroutine(AudioSwitcher(music.audioSource, 0f));
            }
        }
    }

    private void PlayMusicLoop()
    {
        // Ensure there is a valid music clip at the current index
        if (bandMusic == null || bandMusic.Length == 0)
            return;

        AudioClip currentClip = bandMusic[musicIndex];
        if (currentClip == null)
        {
            Debug.LogWarning($"Music clip at index {musicIndex} is null!");
            return;
        }

        // Play the current music clip on both indoor and outdoor AudioSources
        if (indoorBandMusic != null)
            indoorBandMusic.PlayOneShot(currentClip);
        if (outdoorBandMusic != null)
            outdoorBandMusic.PlayOneShot(currentClip);

        // Schedule the next music clip to play after the current one finishes
        float clipLength = currentClip.length;
        StartCoroutine(WaitAndPlayNextClip(clipLength + loopIntervalTime));
    }

    private IEnumerator WaitAndPlayNextClip(float _waitTime)
    {
        yield return new WaitForSeconds(_waitTime);

        // Update the music index and loop the music
        musicIndex = (musicIndex >= bandMusic.Length - 1) ? 0 : musicIndex + 1;

        PlayMusicLoop();
    }

    private IEnumerator AudioSwitcher(AudioSource _audioSource, float _targetVolume)
    {
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource is null in AudioSwitcher!");
            yield break;
        }

        float startVolume = _audioSource.volume;
        float timer = 0f;

        while (!Mathf.Approximately(_audioSource.volume, _targetVolume))
        {
            timer += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, _targetVolume, timer / audioFadeDuration);
            yield return null;
        }

        _audioSource.volume = _targetVolume;
    }
}