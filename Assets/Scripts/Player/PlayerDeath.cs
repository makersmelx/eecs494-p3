using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HasHealth))]
public class PlayerDeath : MonoBehaviour
{
    // Start is called before the first frame update
    //This is a test code that is temporary to automatically respawn the main player.
    Vector3 initPos;
    HasHealth healthBar;

    GameObject panelHealth;
    Text healthText;
    void Start()
    {
        initPos = transform.position;
        healthBar = GetComponent<HasHealth>();
        healthBar.ExecuteDeath += Die;
        panelHealth = Resources.Load<GameObject>("Prefabs/UI/PlayerStats");
        panelHealth = Instantiate(panelHealth);
        healthBar.DamageExtra += UpdateHealth;
        healthBar.HealExtra += UpdateHealth;
        healthText = panelHealth.transform.Find("PanelPlayerStats").Find("healthText").GetComponent<Text>();
        healthText.text = "Health: " + healthBar.hp;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10f)
        {
            healthBar.Die();
        }
    }
    void Die()
    {
        transform.position = initPos ;
        healthBar.HealToFull();
    }

    void UpdateHealth(int amount = 0)
    {
        Debug.Log("We have update the health to " + healthBar.hp);
        healthText.text = "Health: " + healthBar.hp;
    }
}
