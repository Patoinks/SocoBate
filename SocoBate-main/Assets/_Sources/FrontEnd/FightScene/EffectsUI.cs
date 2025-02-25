using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectsUI : MonoBehaviour
{
    public static EffectsUI Instance;
    public GameObject effectTextPrefab; // Assign a floating text prefab
    public Canvas uiCanvas; // Assign the UI Canvas in the Inspector

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowEffect(Vector2 worldPosition, string text, Color color)
    {
        if (effectTextPrefab == null || uiCanvas == null)
        {
            Debug.LogError("EffectTextPrefab or UICanvas is not assigned!");
            return;
        }

        // Instantiate inside the Canvas
        GameObject effectInstance = Instantiate(effectTextPrefab, uiCanvas.transform);
        Text effectText = effectInstance.GetComponentInChildren<Text>();
        RectTransform rectTransform = effectInstance.GetComponent<RectTransform>();

        if (effectText != null)
        {
            effectText.text = text;
            effectText.color = color;
        }

        // Convert world position to UI position
        rectTransform.anchoredPosition = worldPosition;

        StartCoroutine(FadeAndMove(effectInstance, rectTransform));
    }

    private IEnumerator FadeAndMove(GameObject effect, RectTransform rectTransform)
    {
        Text effectText = effect.GetComponentInChildren<Text>();
        CanvasGroup canvasGroup = effect.GetComponent<CanvasGroup>();

        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, 50f); // Moves up in UI space
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(effect);
    }
}
