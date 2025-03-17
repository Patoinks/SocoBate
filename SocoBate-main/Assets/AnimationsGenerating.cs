using UnityEngine;
using UnityEngine.UI;  // Required to work with UI components like Button

public class AnimationsGenerating : MonoBehaviour
{
    private Animator animator;

    // Reference to the button in the UI
    public Button playButton;
    public string effectName = "Fire";  // Set the animation name you want to trigger (e.g., "Fire")

    void Start()
    {
        animator = GetComponent<Animator>();  // Get the Animator component
    }

    // Method to play the effect animation by its name
    public void PlayEffect(string effectName)
    {
        if (animator != null)
        {
            animator.Play(effectName);  // Play the effect animation by name
        }
        else
        {
            Debug.LogWarning("Animator not found!");
        }
    }
}
