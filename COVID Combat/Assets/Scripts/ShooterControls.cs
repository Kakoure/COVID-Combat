using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShooterControls : NetworkBehaviour
{
    public float maxAimDistance;
    public LayerMask aimLayers;
    GameObject pointerTargetObject = null;
    public GunRotation gunRot;
    GameObject cameraObj;

    // Start is called before the first frame update
    void Start()
    {
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!hasAuthority)
        {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(cameraObj.transform.position, cameraObj.transform.forward, out hit, maxAimDistance, aimLayers))
        {
            // GameObject detected in front of the camera.
            if (pointerTargetObject != hit.transform.gameObject)
            {
                // New GameObject.
                pointerTargetObject?.SendMessage("OnPointerExit");
                pointerTargetObject = hit.transform.gameObject;
                pointerTargetObject.SendMessage("OnPointerEnter");
            }

            CmdUpdateAimPoint(hit.point);
        }
        else
        {
            // No GameObject detected in front of the camera.
            pointerTargetObject?.SendMessage("OnPointerExit");
            pointerTargetObject = null;
            CmdUpdateAimPoint(cameraObj.transform.position + (cameraObj.transform.forward * maxAimDistance));
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateAimPoint(Vector3 aimPoint)
    {
        gunRot.UpdateAim(aimPoint);
    }

}
