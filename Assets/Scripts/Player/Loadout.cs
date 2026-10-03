using UnityEngine;

public class Loadout : MonoBehaviour
{
    // The main active slot that holds your selected prefab
    [SerializeField] private GameObject weapon;

    // A collection of prefabs you drag into the Inspector beforehand
    [SerializeField] private GameObject ability;
    [SerializeField] private GameObject item;

    void Update()
    {
        //// Change the active reference depending on user input
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    weapon = ability;
        //    Debug.Log("Switched active slot ro Ability!");
        //}
        //else if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    weapon = item;
        //    Debug.Log("Switched active slot to Item!");
        //}
    }
}