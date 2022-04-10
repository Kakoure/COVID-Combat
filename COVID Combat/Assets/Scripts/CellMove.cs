using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMove : MonoBehaviour
{
    Rigidbody rb;
    float movex;
    float movey;
    float movez;
   
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movex = Random.Range(0f, 10f);
        movey = Random.Range(0f, 10f);
        movez = Random.Range(0f,10f);
        BloodCellMove();

    }

    // Update is called once per frame
/*    void Update()
    {
       

    }*/

    void BloodCellMove()
    {
        rb.AddForce(movex, movey, movez, ForceMode.Impulse);
        

    }
    private void OnCollisionEnter(Collision collision)
    {
        //rb.AddForce(collision.impulse, ForceMode.Impulse);
    }


}
