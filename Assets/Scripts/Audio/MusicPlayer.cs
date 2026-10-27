using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Plays each scene's track from the MusicLibrary. Creates itself when the game starts and survives scene loads,
// so it can fade from one scene's track into the next instead of cutting off.
public class MusicPlayer : MonoBehaviour
{
    private MusicLibrary library;
    private AudioSource source;
    private Coroutine fade;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        MusicLibrary library = Resources.Load<MusicLibrary>("MusicLibrary");
        if (library == null) return;

        GameObject go = new GameObject("MusicPlayer");
        DontDestroyOnLoad(go);
        MusicPlayer player = go.AddComponent<MusicPlayer>();
        player.library = library;

        player.source = go.AddComponent<AudioSource>();
        player.source.loop = true;
        player.source.playOnAwake = false;
        player.source.spatialBlend = 0f; // 2D, same volume wherever the camera is
        player.source.volume = 0f;
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    // covers the first scene in case it finished loading before OnEnable subscribed
    private void Start() => PlayFor(SceneManager.GetActiveScene());

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => PlayFor(scene);

    private void PlayFor(Scene scene)
    {
        AudioClip clip = library.ClipFor(scene.name);
        if (clip == source.clip && (source.isPlaying || clip == null)) return; // same track keeps going

        if (fade != null) StopCoroutine(fade);
        fade = StartCoroutine(SwitchTo(clip));
    }

    // unscaled time so the fade still runs while the inventory has the game paused
    private IEnumerator SwitchTo(AudioClip clip)
    {
        if (source.isPlaying)
        {
            float startVolume = source.volume;
            for (float t = 0f; t < library.fadeTime; t += Time.unscaledDeltaTime)
            {
                source.volume = Mathf.Lerp(startVolume, 0f, t / library.fadeTime);
                yield return null;
            }
            source.Stop();
        }

        source.clip = clip;
        source.volume = 0f;
        if (clip == null)
        {
            fade = null;
            yield break;
        }

        source.Play();
        for (float t = 0f; t < library.fadeTime; t += Time.unscaledDeltaTime)
        {
            source.volume = Mathf.Lerp(0f, library.volume, t / library.fadeTime);
            yield return null;
        }
        source.volume = library.volume;
        fade = null;
    }
}
