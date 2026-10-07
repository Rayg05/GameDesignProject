using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float health = 10f;

    [SerializeField] private float maxHP = 10f;

    [SerializeField] private float regen = 1f;  //regen amount
    [SerializeField] private float regenDelay = 5f; //time between between regen

    //both can be edited for desired amounts

    private float startRegenTime = 0.0f;

    private bool regenAllowed => Time.time > startRegenTime;

    private bool needsRegen = false;

    private void OnTakeDamage() //checks to see if the player has taken damage to determine if they need to regen
    {
        needsRegen = true;

        startRegenTime = Time.time + regenDelay;
    }
    
    public void Damage(float amount)
    {
        health = health - amount;
        if (health - amount <= 0)
        {
            Destroy(gameObject);
        }

        OnTakeDamage();
    }

    private void Regenerate()
    {
        health += regen * Time.deltaTime;   //re adds hp

        Debug.Log("HP is " + health);

        if(health >= maxHP)
        {
            health = maxHP;

            needsRegen = false;
        }
    }

    void Update()
    {
        if(needsRegen &&  regenAllowed) //runs the regen script
        {
            Regenerate();
        }
    }
}
