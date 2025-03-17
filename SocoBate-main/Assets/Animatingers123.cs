using UnityEngine;
using UnityEngine.UI;  // Required for UI elements

public class PlayBubbleAnimationOnClick : MonoBehaviour
{
    public Animator bubbleAnimator;  // Reference to the Animator component
    public Button playButton;        // Reference to the Button

    void Start()
    {
        // Add a listener to the button to call PlayAnimation method on click
        playButton.onClick.AddListener(PlayAnimation);
    }

    void PlayAnimation()
    {
        // Play the animation once (ensure the animation is set to not loop)
        bubbleAnimator.Play("Poison");
    }
}
