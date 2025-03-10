using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Models;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class FloatingEffect : MonoBehaviour
{
    public TMP_Text text;
    public float floatSpeed = 1.5f; // How fast it floats upwards
    public float fadeSpeed = 1f; // How fast it fades

    private float timer = 0f;

    public void Setup(string message, Color color)
    {
        text.text = message;
        text.color = color;

        // Start the floating and fading animation
        StartCoroutine(FloatingTextEffect());
    }

    private IEnumerator FloatingTextEffect()
    {
        while (timer < 1f)
        {
            timer += Time.deltaTime * floatSpeed;
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(1f, 0f, timer)); // Fade out
            transform.position += Vector3.up * Time.deltaTime * 0.5f; // Move upwards
            yield return null;
        }

        Destroy(gameObject); // Destroy the object after the effect is complete
    }
}
