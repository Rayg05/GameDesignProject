using System.Collections;
using UnityEngine;

// Flashes the boss white and spawns a damage number whenever BossHealth takes damage.
// BossHealth adds this automatically, so no setup is needed.
[RequireComponent(typeof(BossHealth))]
public class BossHitFeedback : MonoBehaviour
{
    [Header("Flash")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color flashColor = Color.white;

    [Header("Damage Number")]
    [SerializeField] private float numberHeightAboveTop = 0.5f;
    [SerializeField] private Color numberColor = Color.white;
    [SerializeField] private float numberFontSize = 12f;

    private BossHealth health;
    private SpriteRenderer[] renderers;
    private Material[] originalMaterials;
    private Material flashMaterial;
    private Coroutine flashRoutine;

    private void Awake()
    {
        health = GetComponent<BossHealth>();
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalMaterials = new Material[renderers.Length];

        Shader flashShader = Shader.Find("Custom/SpriteFlash");
        if (flashShader != null)
        {
            flashMaterial = new Material(flashShader);
            flashMaterial.SetColor("_FlashColor", flashColor);
        }
        else
        {
            Debug.LogWarning("BossHitFeedback: Custom/SpriteFlash shader not found, hit flash disabled.");
        }
    }

    private void OnEnable() => health.Damaged += HandleDamaged;
    private void OnDisable() => health.Damaged -= HandleDamaged;

    private void OnDestroy()
    {
        if (flashMaterial != null) Destroy(flashMaterial);
    }

    private void HandleDamaged(float amount)
    {
        DamagePopup popup = DamagePopup.Spawn(GetPopupPosition(), amount, numberColor, numberFontSize);
        // Draw on the boss's sorting layer so the number isn't hidden behind it
        if (renderers.Length > 0 && renderers[0] != null)
            popup.GetComponent<MeshRenderer>().sortingLayerID = renderers[0].sortingLayerID;

        if (flashMaterial == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        else SaveOriginalMaterials();
        flashRoutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        foreach (SpriteRenderer sr in renderers)
            if (sr != null) sr.sharedMaterial = flashMaterial;

        yield return new WaitForSeconds(flashDuration);

        RestoreOriginalMaterials();
        flashRoutine = null;
    }

    private void SaveOriginalMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) originalMaterials[i] = renderers[i].sharedMaterial;
    }

    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].sharedMaterial = originalMaterials[i];
    }

    private Vector3 GetPopupPosition()
    {
        float top = transform.position.y;
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            top = spriteRenderer.bounds.max.y;
        else if (TryGetComponent(out Collider2D col))
            top = col.bounds.max.y;

        // Small random sideways offset so rapid hits don't stack exactly on top of each other
        float x = transform.position.x + Random.Range(-0.4f, 0.4f);
        return new Vector3(x, top + numberHeightAboveTop, transform.position.z);
    }
}
