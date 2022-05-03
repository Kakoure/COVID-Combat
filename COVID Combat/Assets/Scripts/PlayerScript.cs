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
        else if (collision.gameObject.CompareTag("rbc"))
        {
            CmdTakeDamage(10);
            CmdHitRBC();
            var cellCntrl = collision.gameObject.GetComponent<CellMoveNetwork>();
            cellCntrl.CmdReturnCellToPool();
        }

        else if (collision.gameObject.CompareTag("mgc"))
        {
            CmdTakeDamage(20);
            CmdHitMGC();
            var cellCntrl = collision.gameObject.GetComponent<CellMoveNetwork>();
            cellCntrl.CmdReturnCellToPool();
        }

        else if (collision.gameObject.CompareTag("tc"))
        {
            CmdGetPower(30);
            CmdHitTC();
            var cellCntrl = collision.gameObject.GetComponent<CellMoveNetwork>();
            cellCntrl.CmdReturnCellToPool();
        }


        else if (collision.gameObject.CompareTag("bc"))
        {
            CmdTakeDamage(-10);
            CmdHitBC();
            var cellCntrl = collision.gameObject.GetComponent<CellMoveNetwork>();
            cellCntrl.CmdReturnCellToPool();
        }

        else if (collision.gameObject.CompareTag("virus"))
        {
            CmdTakeDamage(20);
            CmdHitVirus();
        }


        else if (!collision.gameObject.name.Contains("BloodVessel"))
        {
            CmdTakeDamage(20);
            CmdShakeCam(1f);
            //Destroy(collision.gameObject);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        outline.OutlineWidth = Mathf.Max(0f, outline.OutlineWidth -= 10 * Time.deltaTime); 
        outline.OutlineColor = outline.OutlineColor = new Color(outline.OutlineColor.r, outline.OutlineColor.g, outline.OutlineColor.b, Mathf.Max(0f, outline.OutlineColor.a - 1 * Time.deltaTime));
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
    void CmdHitVirus()
    {
        RpcColorShip(Color.gray);
        RpcShakeCam(1f);
        //RpcHitVirus();
    }

    [Command]
    void CmdHitRBC()
    {
        RpcColorShip(Color.red);
        RpcShakeCam(1f);
        RpcHitRBC();
    }

    [Command]
    void CmdHitMGC()
    {
        RpcColorShip(Color.yellow);
        RpcShakeCam(1f);
        RpcHitMGC();
    }

    [Command]
    void CmdHitTC()
    {
        RpcColorShip(Color.blue);
        //RpcShakeCam(1f);
        RpcHitTC();
    }


    [Command]
    void CmdHitBC()
    {
        RpcColorShip(Color.green);
        //RpcShakeCam(1f);
        RpcHitBC();
    }

    [ClientRpc]
    void RpcHitRBC()
    {
        source_rbc.PlayOneShot(source_rbc.clip);
    }

    [ClientRpc]
    void RpcHitMGC()
    {
        source_mgc.PlayOneShot(source_mgc.clip);
    }

    [ClientRpc]
    void RpcHitTC()
    {
        source_tc.PlayOneShot(source_tc.clip);
    }

    [ClientRpc]
    void RpcHitBC()
    {
        source_tc.PlayOneShot(source_tc.clip);
    }

    [Command]
    void CmdColorShip(Color color)
    {
        RpcColorShip(color);
    }

    [ClientRpc]
    void RpcColorShip(Color color)
    {
        outline.enabled = true;
        outline.OutlineWidth = 10f;
        outline.OutlineColor = color;
    }

    [Command]
    void CmdGetPower(int val)
    {
        currentPower = (int)powerbar.slider.value;
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
        currentHealth = (int)Mathf.Clamp(currentHealth, 0f, healthBar.GetMaxHealth());
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
