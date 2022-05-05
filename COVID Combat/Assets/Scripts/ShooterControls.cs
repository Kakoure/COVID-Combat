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
    
    public GameObject cameraObj;

    public string shootButton;
    public string bleachButton;
    public string swapWeapon;
    bool shootPressed;
    float timeLastShot;
    Vector3 aimPoint;
    public Transform shotOrigin;
    public GameObject shotObj;

    [SerializeField]
    bool usingAltWeapon;
    [SerializeField]
    Material weaponMat;
    [SerializeField]
    Material altWeaponMat;
    [SerializeField]
    GameObject gunObj;
    [SerializeField]
    LayerMask laserMask;
    [SerializeField]
    GameObject laserTrail;
    [SerializeField]
    GameObject laserHit;

    public PowerBar powerBar;

    public bleachpower bleachCntrl;

    public bool shootingEnabled;

    // Start is called before the first frame update
    void Start()
    {
        //cameraObj = GameObject.FindGameObjectWithTag("MainCamera");
        timeLastShot = Time.time;
        usingAltWeapon = false;
    }



    void Update()
    {
        
        if (!hasAuthority)
        {
            return;
        }
        HandleAim();
        HandleShoot();
        HandleBleach();
        HandleWeaponSwitch();
        
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
        if (!shootingEnabled)
        {
            return;
        }
        if (Input.GetButton(shootButton) && Time.time - timeLastShot > shootCooldown)
        {
            var dirVect = (aimPoint - shotOrigin.position).normalized;
            if (!usingAltWeapon)
            {                
                CmdShootAntibody(dirVect);
                Debug.Log("Pew");
                
            } else if (usingAltWeapon)
            {
                CmdShootLaser(dirVect);
            }
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

    [Command(requiresAuthority =false)]
    public void CmdShootLaser(Vector3 dir)
    {
        RaycastHit laserHit;
        var isHit = Physics.CapsuleCast(shotOrigin.position, shotOrigin.position + Vector3.forward, 2f, dir, out laserHit, 1000f, laserMask, QueryTriggerInteraction.Collide);
        if (isHit)
        {
            RpcGenerateLaser(shotOrigin.position, laserHit.point);

            if (laserHit.collider.CompareTag("virus"))
            {
                GameObject.Find("Score").GetComponent<ScoreTracker>().IncreaseScore();

                if (laserHit.collider.gameObject.name.Contains("Tutorial") && laserHit.collider.gameObject.name.Contains("Shoot"))
                {
                    GameflowManager.Instance.tutorialCompleted = true;
                }
            }

            if(laserHit.collider.CompareTag("virus") || laserHit.collider.CompareTag("rbc") || laserHit.collider.CompareTag("bc") || laserHit.collider.CompareTag("tc") || laserHit.collider.CompareTag("mgc"))
            {
                var cellCntrl = laserHit.collider.GetComponent<CellMoveNetwork>();
                if (cellCntrl.deathClip != null)
                {
                    GameObject.FindGameObjectWithTag("ship").GetComponent<AudioSource>().PlayOneShot(cellCntrl.deathClip.clip);
                }
                cellCntrl.ReturnCellToPool();
            }


        } else
        {
            RpcGenerateLaser(shotOrigin.position, shotOrigin.position + (dir * 1000f));
        }
        
        
    }

    [ClientRpc]
    public void RpcGenerateLaser(Vector3 origin, Vector3 end)
    {
        var distance = ((int)(origin - end).magnitude) + 1;
        var dir = (end - origin).normalized;
        var pos = origin;
        for(int i = 0; i < distance; i++)
        {
            var newLaserTrail = Instantiate(laserTrail, pos, Quaternion.identity);
            newLaserTrail.transform.LookAt(end);
            pos += dir;
        }
        Instantiate(laserHit, end, Quaternion.identity);

    }

    void HandleWeaponSwitch()
    {
        if (Input.GetButtonDown(swapWeapon))
        {
            CmdHandleWeaponSwap();
        }
    }

    void HandleBleach()
    {
        if (Input.GetButtonDown(bleachButton))
        {
            CmdHandleBleach();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdHandleBleach()
    {
        if(bleachCntrl.slider.value == bleachCntrl.slider.maxValue)
        {
            powerBar.SetPower(0);
            RpcSetPower(0);
            bleachCntrl.unleashbleach();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdHandleWeaponSwap()
    {
        usingAltWeapon = !usingAltWeapon;
        RpcWeaponSwap(usingAltWeapon);

    }

    [ClientRpc]
    void RpcWeaponSwap(bool val)
    {
        usingAltWeapon = val;
        if (usingAltWeapon)
        {
            gunObj.GetComponent<MeshRenderer>().material = altWeaponMat;
            shootCooldown = 2f;
        } else
        {
            gunObj.GetComponent<MeshRenderer>().material = weaponMat;
            shootCooldown = .5f;
        }
    }

    [Command]
    void CmdSetPower(int val)
    {
        RpcSetPower(val);
    }

    [ClientRpc]
    void RpcSetPower(int val)
    {
        powerBar.SetPower(val);
    }
}
