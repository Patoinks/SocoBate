// FloatingEffect.cs (Definitive, with Rotation Counteraction)
using UnityEngine;
using TMPro;

public class FloatingEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float travelDistance = 2.0f; 
    [SerializeField] private float lifetime = 1.5f;

    public UnitFacade Owner { get; set; }

    private float timeElapsed = 0f;
    private Color startColor;
    private Vector3 startLocalPosition;

    public void Setup(string message, Color color)
    {
        if (text == null) text = GetComponentInChildren<TMP_Text>();
        text.text = message;
        text.color = color;
        startColor = color;
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (text == null) return;
        timeElapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(timeElapsed / lifetime);
        transform.localPosition = Vector3.Lerp(startLocalPosition, startLocalPosition + new Vector3(0, travelDistance, 0), progress);
        float alpha = 1.0f - progress;
        text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        if (timeElapsed >= lifetime) Destroy(gameObject);
    }

    /// <summary>
    // This is the definitive fix for the rotation problem.
    // It runs after the parent has rotated and forces this object's rotation back to zero.
    /// </summary>
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    void OnDestroy()
    {
        if (FloatingTextQueue.Instance != null)
        {
            FloatingTextQueue.Instance.NotifyTextDestroyed(this);
        }
    }
}