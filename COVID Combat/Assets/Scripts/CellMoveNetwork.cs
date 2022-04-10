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
            gameObject.SetActive(false);
        }
    }
    void BloodCellMove()
    {
        rb.AddForce(movex, movey, movez, ForceMode.Impulse);


    }
    private void OnTriggerEnter(Collision collision)
    {

    }


}
