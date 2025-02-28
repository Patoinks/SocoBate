using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.SceneManagement; // For scene management

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;  // Singleton instance

    public AudioSource audioSource;      // Audio Source to play music
    public AudioClip[] tracks;           // List of music tracks
    public TMP_Text trackNameText;       // TMP text for track name
    public Slider progressSlider;        // Slider to show music progress
    public Slider volumeSlider;          // Volume control slider
    public Slider pitchSlider;           // Pitch control slider
    public Button playPauseButton;       // Play/Pause button
    public Button nextButton;            // Next button
    public Button prevButton;            // Previous button
    public Button stopButton;            // Stop button
    public Sprite playIcon;              // Play icon
    public Sprite pauseIcon;             // Pause icon

    private Transform canvasTransform;   // To hold the reference to the Canvas
    private bool isInCanvas = false;     // To track if MusicPlayer is in the Canvas

    private int currentTrackIndex = 0;   // Current track index
    private bool isPlaying = false;      // Play/Pause state
    private bool isDraggingProgress = false; // Check if user is dragging progress slider

    private void Awake()
    {
        // Ensure there's only one instance of the MusicPlayer
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep MusicPlayer persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }


    }

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
            audioSource.Play(); // Start playing the music
            isPlaying = true;
        }

        // Set default slider values
        if (volumeSlider != null) volumeSlider.value = audioSource.volume;
        if (pitchSlider != null) pitchSlider.value = audioSource.pitch;

        StartCoroutine(UpdateProgressBar());
    }

    void Update()
    {
        if (!isDraggingProgress && audioSource.isPlaying) // Only update slider if user isn't dragging
        {
            progressSlider.value = audioSource.time / audioSource.clip.length;
        }

        // Auto-play next track when current one ends
        if (!audioSource.isPlaying && isPlaying)
        {
            NextTrack();
        }

        // Move back to canvas when pressing M
        if (Input.GetKeyDown(KeyCode.M))
        {
            MoveToCanvas();
        }

        // Move out of canvas when pressing N
        if (Input.GetKeyDown(KeyCode.N))
        {
            MoveOutOfCanvas();
        }
    }

    public void MoveToCanvas()
    {

        // Automatically find and assign the Canvas if it is not assigned in the Inspector
        if (canvasTransform == null)
        {
            GameObject canvas = GameObject.Find("Canvas"); // Try to find Canvas by name
            if (canvas != null)
            {
                canvasTransform = canvas.transform;
            }
            else
            {
                Debug.LogError("No Canvas found in the scene!");
            }
        }
        if (canvasTransform != null && !isInCanvas)
        {
            // Move MusicPlayer out of the scene object hierarchy temporarily
            transform.SetParent(canvasTransform, false); // Move inside the Canvas
            isInCanvas = true;

            // Set the position and size of the MusicPlayer
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(600f, -433f);  // Set the position
            rectTransform.sizeDelta = new Vector2(700f, 300f); // Set the size

            EnableButtons(true); // Enable buttons when inside the canvas
        }
        else
        {
            Debug.LogError("Canvas not assigned in MusicPlayer or already in Canvas!");
        }
    }

    public void MoveOutOfCanvas()
    {
        if (isInCanvas)
        {
            // Remove MusicPlayer from the canvas hierarchy (won't be destroyed)
            transform.SetParent(null, false); // Move out of the Canvas
            isInCanvas = false;
            EnableButtons(false); // Disable buttons when outside the canvas
        }
        else
        {
            Debug.Log("Already outside the canvas!");
        }
    }

    // Enable or disable buttons
    private void EnableButtons(bool enable)
    {
        if (playPauseButton.interactable != enable)
            playPauseButton.interactable = enable;

        if (nextButton.interactable != enable)
            nextButton.interactable = enable;

        if (prevButton.interactable != enable)
            prevButton.interactable = enable;

        if (stopButton.interactable != enable)
            stopButton.interactable = enable;
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
        audioSource.Play(); // Ensure the audio plays
        isPlaying = true;
    }

    public void ChangeVolume()
    {
        audioSource.volume = volumeSlider.value;
    }

    public void ChangePitch()
    {
        audioSource.pitch = pitchSlider.value;
    }

    public void StartSeeking()
    {
        isDraggingProgress = true;
    }

    public void SeekMusic()
    {
        if (audioSource.clip != null)
        {
            audioSource.time = progressSlider.value * audioSource.clip.length;
        }
        isDraggingProgress = false;
    }

    private IEnumerator UpdateProgressBar()
    {
        while (true)
        {
            if (!isDraggingProgress && audioSource.isPlaying)
            {
                progressSlider.value = audioSource.time / audioSource.clip.length;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
