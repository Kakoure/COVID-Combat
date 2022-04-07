using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using PlayFab.Networking;
public class RoleManager : NetworkBehaviour
{
    [SyncVar]
    NetworkIdentity pilotIdentity;
    [SyncVar]
    NetworkIdentity shooterIdentity;

    public GameObject pilotHook;
    public GameObject shooterHook;
    public GameObject shipObj;

    public Button pilotButton;
    public Button shooterButton;

    public PilotControls pilotCntrl;
    public ShooterControls shooterCntrl;

    // Start is called before the first frame update
    void Start()
    {
        UnityNetworkServer.Instance.OnServerDisconnectEvent.AddListener(OnPlayerRemoved);
        SetButtonVals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnJoinPilotPressed()
    {
        if (pilotIdentity == null)
        {
            CmdJoinAsPilot();
        }
    }

    public void OnJoinShooterPressed()
    {
        if (shooterIdentity == null)
        {
            CmdJoinAsShooter();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinAsPilot(NetworkConnectionToClient sender = null)
    {
        if(pilotIdentity == null)
        {
            pilotIdentity = sender.identity;
            shipObj.GetComponent<NetworkIdentity>().AssignClientAuthority(sender);
            pilotCntrl.netIdentity.AssignClientAuthority(sender);
            DisablePilotButton();
            TargetSetupPilot(sender);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdJoinAsShooter(NetworkConnectionToClient sender = null)
    {
        if (shooterIdentity == null)
        {
            shooterIdentity = sender.identity;
            //shipObj.GetComponent<NetworkIdentity>().AssignClientAuthority(sender);
            shooterCntrl.netIdentity.AssignClientAuthority(sender);
            DisableShooterButton();
            TargetSetupShooter(sender);
        }
    }

    [ClientRpc]
    public void DisablePilotButton()
    {
        pilotButton.interactable = false;
    }

    [ClientRpc]
    public void EnablePilotButton()
    {
        pilotButton.interactable = true;
    }

    [ClientRpc]
    public void DisableShooterButton()
    {
        shooterButton.interactable = false;
    }

    [ClientRpc]
    public void EnableShooterButton()
    {
        shooterButton.interactable = true;
    }

    void SetButtonVals()
    {
        shooterButton.interactable = (shooterIdentity == null);
        pilotButton.interactable = (pilotIdentity == null);
    }

    [TargetRpc]
    public void TargetSetupPilot(NetworkConnection target)
    {
        var cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.transform.parent = pilotHook.transform;
        cam.transform.localPosition = Vector3.zero;

        pilotCntrl.enabled = true;
        
    }

    [TargetRpc]
    public void TargetSetupShooter(NetworkConnection target)
    {
        var cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.transform.parent = shooterHook.transform;
        cam.transform.localPosition = Vector3.zero;

        shooterCntrl.enabled = true;

    }


    public void OnPlayerRemoved(NetworkConnectionToClient conn)
    {
        if (conn.identity.Equals(pilotIdentity))
        {
            ResetPilot();
        } else if (conn.identity.Equals(shooterIdentity))
        {
            ResetShooter();
        }


        SetButtonVals();
    }

    void ResetPilot()
    {
        pilotIdentity = null;
        shipObj.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        pilotCntrl.netIdentity.RemoveClientAuthority();
    }

    void ResetShooter()
    {
        shooterIdentity = null;
        shooterCntrl.netIdentity.RemoveClientAuthority();
    }
}
