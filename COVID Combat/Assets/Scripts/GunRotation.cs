using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GunRotation : NetworkBehaviour
{
    public Transform gunPivot;
    public float lerpSpeed;
    //Allows the rotation of the gun to be synced across players
    [SyncVar]
    Vector3 aimPoint;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Move towards desired aim direction
        var dirVect = aimPoint - gunPivot.position;
        var targetRot = Quaternion.LookRotation(dirVect);
        gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, targetRot, lerpSpeed * Time.deltaTime);
    }

    public void UpdateAim(Vector3 newPos)
    {
        aimPoint = newPos;
    }
}
