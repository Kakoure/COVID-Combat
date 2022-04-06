using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    private int counter;
    GameObject player;

    public HealthBar healthBar;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "WhiteBloodCell")
        {
            
        }
        else if (!collision.gameObject.name.Contains("BloodVessel"))
        {
            TakeDamage(20);
            Destroy(collision.gameObject);
        }
        else
        {
            //Debug.Log("Ouch");
            TakeDamage(10);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    void FixedUpdate()
    {

    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);
    }
}
