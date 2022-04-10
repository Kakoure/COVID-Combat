using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide_effect : MonoBehaviour
{
    /*float currenty;*/
    /*Rigidbody rb;*/
    //public GameObject disappear;
    // Start is called before the first frame update


     void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag!="ship")
        {
            
            Destroy(other.gameObject);
           
        }
       // );
       
        /*Destroy(gameObject);*/
    }
/*    void destroycell()
    {
        currenty = rb.transform.position.y;
        if (currenty < -100f)
        {
            Destroy(rb);
        }
    }*/

}
