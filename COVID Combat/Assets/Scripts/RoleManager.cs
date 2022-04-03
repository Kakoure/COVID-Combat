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

    // Start is called before the first frame update
    void Start()
    {
        UnityNetworkServer.Instance.OnServerDisconnectEvent.AddListener(OnPlayerRemoved);
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

    [Command(requiresAuthority = false)]
    public void CmdJoinAsPilot(NetworkConnectionToClient sender = null)
    {
        if(pilotIdentity == null)
        {
            pilotIdentity = sender.identity;
            shipObj.GetComponent<NetworkIdentity>().AssignClientAuthority(sender);
            DisablePilotButton();
            TargetSetupPilot(sender);
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

    [TargetRpc]
    public void TargetSetupPilot(NetworkConnection target)
    {
        var cam = GameObject.FindGameObjectWithTag("MainCamera");
        cam.transform.parent = pilotHook.transform;
        cam.transform.localPosition = Vector3.zero;

        pilotHook.GetComponentInChildren<PilotControls>().enabled = true;
        
    }


    public void OnPlayerRemoved(NetworkConnectionToClient conn)
    {
        if (conn.identity.Equals(pilotIdentity))
        {
            ResetPilot();
        }
    }

    void ResetPilot()
    {
        pilotIdentity = null;
        shipObj.GetComponent<NetworkIdentity>().RemoveClientAuthority();
        EnablePilotButton();
    }
}
