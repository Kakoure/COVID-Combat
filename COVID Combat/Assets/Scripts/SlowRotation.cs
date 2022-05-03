using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotation : MonoBehaviour
{
    public float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, rotationSpeed * Time.deltaTime, 0f));
    }
}
