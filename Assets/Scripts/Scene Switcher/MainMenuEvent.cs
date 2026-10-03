using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuEvent : MonoBehaviour
{
    private UIDocument _document;

    private Button _button;

    [SerializeField] private string sceneName;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        _button = _document.rootVisualElement.Q("Start") as Button;
        _button.RegisterCallback<ClickEvent>(onPlayGameClick);
    }

    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(onPlayGameClick);
    }
    private void onPlayGameClick(ClickEvent evt)
    {
        UnityEngine.Debug.Log("pressed");
        SceneManager.LoadScene(sceneName);
    }
}
