using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotControls : MonoBehaviour
{
    public Rigidbody rb;
    public float force;
    GameObject mainCamera;
    float horMove;
    float vertMove;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        horMove = 0;
        vertMove = 0;
    }

    // Update is called once per frame
    void Update()
    {
        horMove = Input.GetAxis("Horizontal");
        vertMove = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        Vector3 forwardVect = mainCamera.transform.forward * vertMove;
        Vector3 sideVect = mainCamera.transform.right * horMove;
        Vector3 moveVect = forwardVect + sideVect;
        rb.AddForce(moveVect * force * Time.fixedDeltaTime);
    }
}
