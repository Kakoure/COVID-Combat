using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AntibodyController : NetworkBehaviour
{
    public float maxLifetime;
    public float speed;
    float startTime;
    [SerializeField]
    GameObject deathParticles;

    // Update is called once per frame
    void Update()
    {
        if(isServer)
        startTime += Time.deltaTime;
        if(startTime >= maxLifetime)
        {

            NetworkServer.Destroy(gameObject);
        }
    }

    public void SetupProjectile()
    {
        startTime = 0;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
        {
            return;
        }
        //Debug.Log("Hit obj");
        if (collision.gameObject.CompareTag("virus"))
        {
            var cellCntrl = collision.gameObject.GetComponent<CellMoveNetwork>();
            cellCntrl.ReturnCellToPool();
            GameObject.Find("Score").GetComponent<ScoreTracker>().IncreaseScore();
        }
        NetworkServer.Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcStartParticles()
    {
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Play();
        }
    }

    [ClientRpc]
    public void RpcExplodeParticles()
    {
        Instantiate(deathParticles, transform.position, Quaternion.identity);
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Instantiate(deathParticles, transform.position, Quaternion.identity);
    }
}


