using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace

public class MusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;      // Audio Source to play music
    public AudioClip[] tracks;           // List of music tracks
    public TMP_Text trackNameText;       // TMP text for track name
    public Slider progressSlider;        // Slider to show music progress
    public Slider volumeSlider;          // Volume control slider
    public Button playPauseButton;       // Play/Pause button
    public Sprite playIcon;              // Play icon
    public Sprite pauseIcon;             // Pause icon

    private int currentTrackIndex = 0;   // Current track index
    private bool isPlaying = false;      // Play/Pause state

    void Start()
    {
        Application.runInBackground = true;
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("No AudioSource found!");
                return;
            }
        }

        if (tracks.Length > 0)
        {
            audioSource.clip = tracks[currentTrackIndex];
            trackNameText.text = audioSource.clip.name;
            audioSource.Play(); // <----- FORCE PLAY
        }
    }


    void Update()
    {
        if (audioSource.isPlaying)
        {
            progressSlider.value = audioSource.time / audioSource.clip.length;
        }

        // Auto-play next track when current one ends
        if (!audioSource.isPlaying && isPlaying)
        {
            NextTrack();
        }

        
    }

    public void PlayPause()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPlaying = false;
            playPauseButton.image.sprite = playIcon;
        }
        else
        {
            audioSource.Play();
            isPlaying = true;
            playPauseButton.image.sprite = pauseIcon;
        }
    }

    public void Stop()
    {
        audioSource.Stop();
        isPlaying = false;
        playPauseButton.image.sprite = playIcon;
    }

    public void NextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
        PlayTrack();
    }

    public void PreviousTrack()
    {
        currentTrackIndex = (currentTrackIndex - 1 + tracks.Length) % tracks.Length;
        PlayTrack();
    }

    private void PlayTrack()
    {
        if (tracks.Length == 0) return;

        audioSource.clip = tracks[currentTrackIndex];

        if (audioSource.clip == null)
        {
            Debug.LogError("Audio clip is null!");
            return;
        }

        trackNameText.text = audioSource.clip.name;
        audioSource.Play(); // Ensure the audio actually plays
        isPlaying = true;
    }


    public void ChangeVolume()
    {
        audioSource.volume = volumeSlider.value;
    }

    public void SeekMusic()
    {
        audioSource.time = progressSlider.value * audioSource.clip.length;
    }

    private IEnumerator UpdateProgressBar()
    {
        while (true)
        {
            if (audioSource.isPlaying)
            {
                progressSlider.value = audioSource.time / audioSource.clip.length;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
