using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerScript : NetworkBehaviour
{
    public int maxHealth = 100;
    public int maxPower = 100;
    [SyncVar]
    public int currentHealth;
    public int currentPower;
    public AudioSource source_rbc;
    public AudioSource source_tc;
    public AudioSource source_mgc;
    public CameraShake camShake;
    private int counter;
    public GameObject windsheild;
    Outline outline;
    GameObject player;
    Rigidbody rb;



    public PowerBar powerbar;
    public HealthBar healthBar;
    [SerializeField]
    Image fadeImage;
    bool dying;
    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currentPower = 0;
        powerbar.SetMaxPower(maxPower);
        rb = GetComponent<Rigidbody>();
        outline = windsheild.GetComponent<Outline>();
        dying = false;

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
        else if (collision.gameObject.tag == "rbc")
        {

            outline.enabled = true;
            outline.OutlineColor = Color.red;
            outline.OutlineWidth = 10f;
            source_rbc.Play();
            CmdShakeCam(1f);
        }

        else if (collision.gameObject.tag == "mgc")
        {

            outline.enabled = true;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 10f;
            source_mgc.Play();
            CmdShakeCam(1f);
        }

        else if (collision.gameObject.tag == "tc")
        {

            outline.enabled = true;
            outline.OutlineColor = Color.blue;
            outline.OutlineWidth = 10f;
            CmdGetPower(30);
            source_tc.Play();
        }


        else if (!collision.gameObject.name.Contains("BloodVessel"))
        {
            CmdTakeDamage(20);
            CmdShakeCam(1f);
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.name.Contains("BloodVessel"))
        {
            if(collision.impulse.magnitude > 25f)
            {
                CmdTakeDamage(10);
                CmdShakeCam(1f);
            }
            
        }
        
    }

    private void OnCollisionExit(Collision other)
    {
        outline.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(rb.velocity.magnitude);
        if (isServer)
        {
            if (currentHealth <= 0 && !dying)
            {
                dying = true;
                CmdDeathSequence();
                
            }
        }
    }

    void FixedUpdate()
    {

    }




    [Command]
    void CmdGetPower(int val)
    {
        currentPower += val;
        RpcSetPower(currentPower);
    }


    [Command]
    void CmdSetPower(int val)
    {
        currentPower = val;
        RpcSetPower(val);
    }

    [ClientRpc]
    void RpcSetPower(int val)
    {
        powerbar.SetPower(val);
    }


    [Command]
    void CmdShakeCam(float len)
    {
        RpcShakeCam(len);
    }

    [ClientRpc]
    void RpcShakeCam(float len)
    {
        camShake.SetShake(len);
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

    [Command]
    void CmdDeathSequence()
    {
        RpcDeathSequence();
        StartCoroutine(DeathCoroutine());
    }

    [ClientRpc]
    void RpcDeathSequence()
    {
        StartCoroutine(DeathCoroutine());
    }
    IEnumerator DeathCoroutine()
    {
        dying = true;
        camShake.SetShake(5f);
        for(float i = 0f; i < 1.2f; i += .005f)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, i);
            

            yield return new WaitForSeconds(.01f);
        }

        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("Death Screen");
        }
    }
}
