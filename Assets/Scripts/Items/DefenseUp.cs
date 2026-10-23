using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class DefenseUp : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private EquipmentData defenseUp;
    [SerializeField] private int usages = 3;
    [SerializeField] private float buffDuration = 10f;
    [SerializeField] private float cooldownDuration = 10f;

    private float baseMult;
    private bool isActive = false;
    private bool onCooldown = false;
    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        if (health == null) return;

        if (Input.GetKeyDown(KeyCode.Q) && Loadout.IsEquipped(defenseUp) && Time.timeScale > 0f)
        {
            if (!isActive && !onCooldown && usages > 0)
            {
                baseMult = health.damageMult;
                health.damageMult = 0.5f;//reduces incoming damage by 1/2
                isActive = true;
                timer = 0f;
                usages--;
                Debug.Log("damage reduced");
            }
            else if(onCooldown)
            {
                Debug.Log("on cooldown");
            }
        }

        if(isActive)
        {
            timer  += Time.deltaTime;
            if(timer >= buffDuration)
            {
                health.damageMult = baseMult;
                isActive = false;
                onCooldown = true;
                timer = 0f;
                Debug.Log("buff on cooldown");
            }
        }
        else if (onCooldown)
        {
            timer += Time.deltaTime;
            if(timer >= cooldownDuration)
            {
                onCooldown = false;
                timer = 0f;
                if(usages > 0)
                {
                    Debug.Log("cooldown finished");
                }
                else
                {
                    Debug.Log("out of defense up");
                }
            }
        }
    }
}
