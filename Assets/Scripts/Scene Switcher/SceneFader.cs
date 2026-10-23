using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    private const float FadeDuration = 1f;

    private static SceneFader instance;

    private Image overlay;

    public static bool IsTransitioning { get; private set; }

    public static void LoadScene(string sceneName, float delay = 0f)
    {
        if (IsTransitioning) return;
        IsTransitioning = true;
        GetInstance().StartCoroutine(instance.Transition(sceneName, delay));
    }

    private static SceneFader GetInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("SceneFader");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<SceneFader>();
            instance.BuildOverlay();
        }
        return instance;
    }

    private void BuildOverlay()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;
        gameObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("Overlay");
        imageObject.transform.SetParent(transform, false);
        overlay = imageObject.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 0f);
        overlay.raycastTarget = false;

        RectTransform rect = overlay.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private IEnumerator Transition(string sceneName, float delay)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);

        overlay.raycastTarget = true;
        yield return Fade(0f, 1f);

        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return Fade(1f, 0f);
        overlay.raycastTarget = false;
        IsTransitioning = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        for (float t = 0f; t < FadeDuration; t += Time.unscaledDeltaTime)
        {
            overlay.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, t / FadeDuration));
            yield return null;
        }
        overlay.color = new Color(0f, 0f, 0f, to);
    }
}
