using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ShooterControls : NetworkBehaviour
{
    public float maxAimDistance;
    public LayerMask aimLayers;

    public float shootCooldown;
    public float projSpeed;
    GameObject pointerTargetObject = null;
    public GunRotation gunRot;
    GameObject cameraObj;

    public string shootButton;
    bool shootPressed;
    float timeLastShot;
    Vector3 aimPoint;
    public Transform shotOrigin;
    public GameObject shotObj;

    // Start is called before the first frame update
    void Start()
    {
        cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        timeLastShot = Time.time;
    }



    void FixedUpdate()
    {
        
        if (!hasAuthority)
        {
            return;
        }
        HandleAim();
        HandleShoot();
        
    }


    void HandleAim()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraObj.transform.position, cameraObj.transform.forward, out hit, maxAimDistance, aimLayers))
        {
            // GameObject detected in front of the camera.
            if (pointerTargetObject != hit.transform.gameObject)
            {
                // New GameObject.
                if (pointerTargetObject != null)
                {
                    pointerTargetObject?.SendMessage("OnPointerExit");
                }
                pointerTargetObject = hit.transform.gameObject;
                pointerTargetObject.SendMessage("OnPointerEnter");
            }
            aimPoint = hit.point;
            CmdUpdateAimPoint(hit.point);
        }
        else
        {
            // No GameObject detected in front of the camera.
            if(pointerTargetObject != null)
            {
                pointerTargetObject?.SendMessage("OnPointerExit");
            }
            pointerTargetObject = null;
            aimPoint = cameraObj.transform.position + (cameraObj.transform.forward * maxAimDistance);
            CmdUpdateAimPoint(aimPoint);
        }
    }

    void HandleShoot()
    {
        if (Input.GetButton(shootButton) && Time.time - timeLastShot > shootCooldown)
        {
            var dirVect = (aimPoint - shotOrigin.position).normalized;
            CmdShootAntibody(dirVect);
            Debug.Log("Pew");
            timeLastShot = Time.time;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdUpdateAimPoint(Vector3 aimPoint)
    {
        gunRot.UpdateAim(aimPoint);
    }


    [Command(requiresAuthority = false)]
    public void CmdShootAntibody(Vector3 dir)
    {
        GameObject newProj = Instantiate(shotObj, shotOrigin.position, Quaternion.LookRotation(dir, Vector3.up));
        var cntrl = newProj.GetComponent<AntibodyController>();
        cntrl.SetupProjectile();
        NetworkServer.Spawn(newProj);
        cntrl.RpcStartParticles();

    }
}
