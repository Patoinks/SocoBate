// UnitAnimationController.cs (Definitive, with Correct Stacking)
using System;
using System.Collections;
using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
    public static UnitAnimationController Instance { get; private set; }
    
    [Header("VFX & UI Prefabs")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private GameObject meleeSlashEffectPrefab;
    
    void Awake() { if (Instance != null && Instance != this) Destroy(gameObject); else Instance = this; }

    public void RequestFloatingText(string text, Color color, UnitFacade target)
    {
        if (FloatingTextQueue.Instance != null)
        {
            FloatingTextQueue.Instance.AddToQueue(text, color, target);
        }
    }

    // --- THIS IS THE DEFINITIVE, CORRECTED METHOD ---
    public void InstantiateFloatingText(string text, Color color, UnitFacade target, float yOffset)
    {
        if (UIManager.IsFightSkipped) return;
        if (floatingTextPrefab == null || target == null) return;
        
        // 1. Instantiate the prefab and parent it to the target unit.
        GameObject textGO = Instantiate(floatingTextPrefab, target.transform);

        // 2. THIS IS THE FIX. Set the localPosition directly using '=', not '+='.
        // This ensures the stacking offset calculated by the queue is respected exactly.
        textGO.transform.localPosition = new Vector3(0, yOffset, 0);
        
        FloatingEffect floatingEffect = textGO.GetComponent<FloatingEffect>();
        if (floatingEffect != null)
        {
            floatingEffect.Owner = target;
            floatingEffect.Setup(text, color);
            FloatingTextQueue.Instance.RegisterActiveText(floatingEffect);
        }
    }

    #region Unchanged Methods
    public IEnumerator AnimateMeleeAttack(UnitFacade attacker, UnitFacade target, float duration, Action onImpact)
    {
        if (UIManager.IsFightSkipped) { onImpact?.Invoke(); yield break; }
        Transform attackerTransform = attacker.transform;
        Vector3 originalPosition = attackerTransform.position;
        Vector3 attackPosition = target.transform.position;
        yield return StartCoroutine(MoveTransform(attackerTransform, originalPosition, attackPosition, duration));
        PlayVFX(meleeSlashEffectPrefab, target.transform);
        onImpact?.Invoke();
        yield return new WaitForSeconds(0.15f);
        yield return StartCoroutine(MoveTransform(attackerTransform, attackPosition, originalPosition, duration));
    }
    
    public IEnumerator AnimateDeath(UnitFacade deadUnit, float duration)
    {
        if (deadUnit.HealthBar != null) deadUnit.HealthBar.gameObject.SetActive(false);
        if (UIManager.IsFightSkipped) { deadUnit.gameObject.SetActive(false); yield break; }
        Transform unitTransform = deadUnit.transform;
        Vector3 startPos = unitTransform.position;
        Vector3 endPos = startPos - new Vector3(0, 2, 0);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            unitTransform.position = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        deadUnit.gameObject.SetActive(false);
    }
    
    public void PlayVFX(GameObject vfxPrefab, Transform target)
    {
        if (UIManager.IsFightSkipped) return;
        if (vfxPrefab == null || target == null) return;
        GameObject vfxInstance = Instantiate(vfxPrefab, target.position, Quaternion.identity);
        Destroy(vfxInstance, 2.0f);
    }
    
    private IEnumerator MoveTransform(Transform transformToMove, Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transformToMove.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transformToMove.position = end;
    }
    #endregion
}