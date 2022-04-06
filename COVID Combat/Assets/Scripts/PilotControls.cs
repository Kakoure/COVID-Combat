using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotControls : MonoBehaviour
{
    //public Rigidbody rb;
    // force = 25, turn rate = 1 is good
    public float force;
    public float turnRate;
    GameObject player;
    private Rigidbody rb;
    private CharacterController charController;
    float horMove;
    float vertMove;
    
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        charController = GetComponent<CharacterController>();
        player = gameObject;
        horMove = 0;
        vertMove = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Horiz: " +Input.GetAxis("Horizontal"));
        horMove = Input.GetAxis("Horizontal");
        vertMove = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        if (horMove >= .05 || horMove <= -.05 || vertMove >= .05 || vertMove <= -.05)
        {
           player.transform.Rotate(turnRate * vertMove, turnRate * horMove, 0);

        }

        Vector3 moveVect = player.transform.forward;
        charController.Move(moveVect * force * Time.fixedDeltaTime);
    }
}
