using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HasHealth is the core dealing with all DeathEvent! It deals with basic combats!
public class HasHealth : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("HP")]
    public int maxHP = 8;
    public int hp;

    public delegate void DeathEffect();
    public DeathEffect ExecuteDeath;

    public delegate void HealthEffect(int amount);
    public HealthEffect DamageExtra;
    public HealthEffect HealExtra;

    void Start()
    {
        hp = maxHP;
    }

    private void ModifyHealth(int amount)
    {
        hp += amount;
        //Debug.Log("Health Modified:" + amount);
        //Debug.Log("Current Health: " + hp);
    }

    public bool TakeDamage(int amount)
    {
        //Return true is the damage is successfully excecuted.
        if (amount < 0)
        {
            Debug.Log("Please enter a positive amount for damage!");
            return false;
        }
        bool isSuccess = amount != 0;
        ModifyHealth(-amount);
        if (isSuccess)
        {
            DamageExtra(amount);
        }
        if (hp <= 0)
        {
            Die();
           
        }
        //Excecute On Damage Effect


        return isSuccess;
    }


    public bool Heal(int amount)
    {
        //Return true is the damage is successfully healed.
        bool isSuccess = amount != 0;
        if (amount < 0)
        {
            Debug.Log("Please enter a positive amount for healing!");
            return false;
        }
        ModifyHealth(amount);
        //Excecute On Heal Effect
        if (isSuccess)
        {
            HealExtra(amount);
        }
        return isSuccess;
    }
    public bool HealToFull()
    {
        bool isSuccess = hp != maxHP;
        isSuccess = Heal(maxHP - hp);
        return isSuccess;
    }
    public void Die()
    {
        EventBus.Publish(new DeathEvent(gameObject.name));
        ExecuteDeath();
    }
    
}
public class DeathEvent{
    public string name;
    public DeathEvent (string name_in)
    {
        name = name_in;
    }

}
