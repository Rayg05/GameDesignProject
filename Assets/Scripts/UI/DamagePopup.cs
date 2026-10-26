using TMPro;
using UnityEngine;

// Floating "-25" style number that rises, fades out and destroys itself.
// It's its own object so it keeps animating even if the thing that got hit is destroyed.
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float riseSpeed = 1.5f;
    [SerializeField] private float popScale = 1.5f; // starts bigger and shrinks to normal

    private TextMeshPro text;
    private Color startColor;
    private float timer;

    public static DamagePopup Spawn(Vector3 position, float amount, Color color, float fontSize = 12f)
    {
        GameObject go = new GameObject("Damage Popup");
        go.transform.position = position;

        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = "-" + Mathf.RoundToInt(amount);
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        tmp.outlineWidth = 0.25f;
        tmp.outlineColor = Color.black;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.rectTransform.sizeDelta = new Vector2(8f, 3f);
        go.GetComponent<MeshRenderer>().sortingOrder = 100;

        DamagePopup popup = go.AddComponent<DamagePopup>();
        popup.text = tmp;
        popup.startColor = color;
        return popup;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifetime;

        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        transform.localScale = Vector3.one * Mathf.Lerp(popScale, 1f, Mathf.Clamp01(t * 4f));

        // Stay solid for the first half, then fade out
        Color c = startColor;
        c.a = Mathf.Lerp(startColor.a, 0f, Mathf.Clamp01((t - 0.5f) * 2f));
        text.color = c;

        if (timer >= lifetime) Destroy(gameObject);
    }
}
