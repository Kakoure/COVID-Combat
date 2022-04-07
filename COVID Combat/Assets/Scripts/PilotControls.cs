using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PilotControls : NetworkBehaviour
{
    //public Rigidbody rb;
    // force = 25, turn rate = 1 is good
    public float force;
    public float turnRate;

    public bool drivingEnabled;
    public GameObject ship;
    private Rigidbody rb;
    private CharacterController charController;
    float horMove;
    float vertMove;
    
    

    // Start is called before the first frame update
    void Start()
    {
        rb = ship.GetComponent<Rigidbody>();
        horMove = 0;
        vertMove = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }
        //Debug.Log("Horiz: " +Input.GetAxis("Horizontal"));
        horMove = Input.GetAxis("Horizontal");
        vertMove = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if (!hasAuthority)
        {
            return;
        }
        if (horMove >= .05 || horMove <= -.05 || vertMove >= .05 || vertMove <= -.05)
        {
           ship.transform.Rotate(turnRate * vertMove, turnRate * horMove, 0);

        }

        Vector3 moveVect = ship.transform.forward;
        if (drivingEnabled)
        {
            rb.AddForce(moveVect * force);
        }
    }
}
