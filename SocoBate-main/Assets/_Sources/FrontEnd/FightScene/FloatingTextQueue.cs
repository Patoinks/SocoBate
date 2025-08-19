// FloatingTextQueue.cs (Definitive, Per-Unit Queue System)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloatingTextQueue : MonoBehaviour
{
    public static FloatingTextQueue Instance { get; private set; }

    [Header("Timing")]
    [SerializeField] private float displayDelay = 0.2f;

    private Dictionary<UnitFacade, Queue<string[]>> _unitQueues;
    private HashSet<UnitFacade> _processingUnits;
    private Dictionary<UnitFacade, List<FloatingEffect>> _activeTexts;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        _unitQueues = new Dictionary<UnitFacade, Queue<string[]>>();
        _processingUnits = new HashSet<UnitFacade>();
        _activeTexts = new Dictionary<UnitFacade, List<FloatingEffect>>();
    }

    public void AddToQueue(string text, Color color, UnitFacade target)
    {
        if (!_unitQueues.ContainsKey(target))
        {
            _unitQueues[target] = new Queue<string[]>();
        }
        _unitQueues[target].Enqueue(new string[] { text, ColorUtility.ToHtmlStringRGB(color) });
        if (!_processingUnits.Contains(target))
        {
            StartCoroutine(ProcessUnitQueueCoroutine(target));
        }
    }

    private IEnumerator ProcessUnitQueueCoroutine(UnitFacade unit)
    {
        _processingUnits.Add(unit);
        while (_unitQueues.ContainsKey(unit) && _unitQueues[unit].Count > 0)
        {
            string[] request = _unitQueues[unit].Dequeue();
            string text = request[0];
            ColorUtility.TryParseHtmlString("#" + request[1], out Color color);
            float yOffset = GetNextYOffsetForUnit(unit);
            UnitAnimationController.Instance.InstantiateFloatingText(text, color, unit, yOffset);
            yield return new WaitForSeconds(displayDelay);
        }
        _processingUnits.Remove(unit);
    }
    
    private float GetNextYOffsetForUnit(UnitFacade unit)
    {
        float initialOffset = 100f;
        float stackingOffset = 25f;
        if (!_activeTexts.ContainsKey(unit) || _activeTexts[unit].Count == 0)
        {
            return initialOffset;
        }
        float highestY = _activeTexts[unit].Max(t => t.transform.localPosition.y);
        return highestY + stackingOffset;
    }

    public void RegisterActiveText(FloatingEffect textEffect)
    {
        UnitFacade owner = textEffect.Owner;
        if (!_activeTexts.ContainsKey(owner))
        {
            _activeTexts[owner] = new List<FloatingEffect>();
        }
        _activeTexts[owner].Add(textEffect);
    }
    
    public void NotifyTextDestroyed(FloatingEffect textEffect)
    {
        UnitFacade owner = textEffect.Owner;
        if (owner != null && _activeTexts.ContainsKey(owner))
        {
            _activeTexts[owner].Remove(textEffect);
        }
    }
}