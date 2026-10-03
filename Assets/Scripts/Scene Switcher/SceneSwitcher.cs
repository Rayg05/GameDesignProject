using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    #pragma warning disable 0649
        [SerializeField] private string sceneName;
    #pragma warning disable 0649

        private void OnTriggerEnter2D(Collider2D collision)
    {
        Basic_Movement player = collision.gameObject.GetComponent<Basic_Movement>();
        if (player)
            SceneManager.LoadScene(sceneName);
    }
}
