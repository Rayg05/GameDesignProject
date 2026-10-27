using System;
using UnityEngine;

// Which music track plays in which scene. Lives in Resources so MusicPlayer can find it without being placed in a scene.
[CreateAssetMenu(menuName = "Game/Music Library")]
public class MusicLibrary : ScriptableObject
{
    [Serializable]
    public class SceneTrack
    {
        public string sceneName; // must match the scene file name, e.g. "HubWorld"
        public AudioClip clip;
    }

    public SceneTrack[] tracks;
    [Range(0f, 1f)] public float volume = 0.6f;
    public float fadeTime = 1f; // seconds to fade out the old track and fade in the new one

    // null if the scene has no music
    public AudioClip ClipFor(string sceneName)
    {
        foreach (SceneTrack track in tracks)
            if (track.sceneName == sceneName) return track.clip;
        return null;
    }
}
