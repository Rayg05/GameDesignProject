using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset gameWindowTemplate;    //references

    private VisualElement windowContainer; 
    private Dictionary<string, GameWindow> windows = new(); //fullscreen element and window lookup

    private void Awake()
    {
        var root = uiDocument.rootVisualElement;
        windowContainer = new VisualElement() { name = "window-container"}; 

        windowContainer.style.position = Position.Absolute;
        windowContainer.style.width = Length.Percent(100);
        windowContainer.style.height = Length.Percent(100);

        windowContainer.pickingMode = PickingMode.Ignore; //container doesnt intercept clicks

        root.Add(windowContainer);
    }

    public GameWindow CreateWindow(string id, string title, Vector2 defaultPosition)
    {
        float x = PlayerPrefs.GetFloat($"window_{id}_x", defaultPosition.x);
        float y = PlayerPrefs.GetFloat($"window_{id}_y", defaultPosition.y);

        var window = new GameWindow(gameWindowTemplate, windowContainer, title);
        window.SetPosition(x,y);

        windows[id] = window;

        return window;
    }

    public void ToggleWindow(string id)
    {
        if(!windows.TryGetValue(id, out var window)) return;

        if(window.isVisible)
        {
            SaveWindowPosition(id, window);
            window.Hide();
        }
        else
        {
            window.Show();
        }
    }

    public void CloseAllWindows()
    {
        foreach(var kvp in windows)
        {
            if(kvp.Value.isVisible)
            {
                SaveWindowPosition(kvp.Key, kvp.Value);
                kvp.Value.Hide();
            }
        }
    }

    private void SaveWindowPosition(string id, GameWindow window)
    {
        var pos = window.GetPostion();
        PlayerPrefs.SetFloat($"window_{id}_x", pos.x);
        PlayerPrefs.SetFloat($"window_{id}_y", pos.y);
    }

    private void SaveAllPositions()
    {
        foreach(var kvp in windows)
        {
            if(kvp.Value.isVisible) SaveWindowPosition(kvp.Key, kvp.Value);
        }

        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SaveAllPositions();
    }
}
