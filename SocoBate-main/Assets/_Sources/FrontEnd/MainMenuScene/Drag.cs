using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DisableDragScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect scrollRect;
    public RectTransform content; // Assign your Content RectTransform in Inspector

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        ForceLayoutUpdate();
        scrollRect.verticalNormalizedPosition = 1f; // Start at the top
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * 0.2f;
            scrollRect.verticalNormalizedPosition += scrollAmount;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) { }
    public void OnDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) { }

    public void ForceLayoutUpdate()
    {
        if (content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }
    }
}
