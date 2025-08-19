// FloatingTextAnimator.cs
using UnityEngine;
using TMPro;

public class FloatingTextAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float moveSpeed = 60f;     // How fast the text moves up in screen pixels per second.
    [SerializeField] private float fadeDuration = 1.2f; // How long it takes to fade out completely.

    private TextMeshProUGUI textMesh;
    private float timeElapsed = 0f;
    private Color startColor;

    // This method is called by the UnitAnimationController right after the prefab is created.
    public void Setup(string message, Color color)
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if (textMesh == null)
        {
            Debug.LogError("FloatingTextAnimator prefab is missing its TextMeshProUGUI component!");
            Destroy(gameObject); // Can't function without this.
            return;
        }

        textMesh.text = message;
        textMesh.color = color;
        startColor = color;
    }

    // Update is called once per frame to handle the animation.
    void Update()
    {
        if (textMesh == null) return;

        timeElapsed += Time.deltaTime;

        // Move the RectTransform on the canvas.
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // Calculate the new alpha (transparency) value based on how much time has passed.
        float alpha = 1.0f - (timeElapsed / fadeDuration);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Clamp01(alpha));
        
        // Destroy the object once it has fully faded.
        if (timeElapsed >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}