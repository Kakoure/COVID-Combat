using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CellMoveNetwork : NetworkBehaviour
{
    Rigidbody rb;
    float movex;
    float movey;
    float movez;
    public float despawnDistance;
    public GameObject playerObj;
    CellSpawner.ObjectPool pool;


    // Start is called before the first frame update
    void Start()
    {
        if (!isServer)
        {
            return;
        }
        rb = GetComponent<Rigidbody>();
        movex = Random.Range(0f, 10f);
        movey = Random.Range(0f, 10f);
        movez = Random.Range(0f, 10f);
        BloodCellMove();

    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }
        var distance = (playerObj.transform.position - transform.position).magnitude;
        if (distance >= despawnDistance)
        {
            RpcHideObj();
            gameObject.SetActive(false);
            //pool.pooledObjects.Enqueue(gameObject);
            NetworkServer.UnSpawn(gameObject);
        }
    }
    void BloodCellMove()
    {
        rb.AddForce(movex, movey, movez, ForceMode.Impulse);


    }
    private void OnTriggerEnter(Collision collision)
    {

    }

    [ClientRpc]
    void RpcHideObj()
    {
        gameObject.SetActive(false);

    }
    [ClientRpc]
    public void RpcShowObject()
    {
        gameObject.SetActive(true);
    }

    public void SetPool(CellSpawner.ObjectPool pool)
    {
        this.pool = pool;
    }

}
