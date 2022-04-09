using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
    public int maxHealth = 100;
    [SyncVar]
    public int currentHealth;
    private int counter;
    GameObject player;
    Rigidbody rb;

    public HealthBar healthBar;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (!hasAuthority)
        {
            return;
        }
        if (collision.gameObject.name == "WhiteBloodCell")
        {
            
        }
        else if (!collision.gameObject.name.Contains("BloodVessel"))
        {
            CmdTakeDamage(20);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.name.Contains("BloodVessel"))
        {
            if(collision.impulse.magnitude > 25f)
            {
                CmdTakeDamage(10);
            }
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rb.velocity.magnitude);
    }

    void FixedUpdate()
    {

    }

    [Command]
    void CmdTakeDamage(int val)
    {
        currentHealth -= val;
        RpcSetHealth(currentHealth);
    }


    [Command]
    void CmdSetHealth(int val)
    {
        currentHealth = val;
        RpcSetHealth(val);
    }

    [ClientRpc]
    void RpcSetHealth(int val)
    {
        healthBar.SetHealth(val);
    }

}
