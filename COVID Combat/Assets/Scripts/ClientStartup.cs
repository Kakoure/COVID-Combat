using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using PlayFab.Networking;


public class ClientStartup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void BeginClientStartup()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = SystemInfo.deviceUniqueIdentifier
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
    }

    void OnPlayFabLoginSuccess(LoginResult loginResult)
    {
        Debug.Log("Login Success!");

        RequestMultiplayerServer();
    }

    private void RequestMultiplayerServer()
    {
        Debug.Log("[ClientStartup].RequestMultiplayerServer");
        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest
        {
            BuildId = "354a0d98-2a94-463c-8435-a7d81a1048d8",
            SessionId = "951BC03B-3DDB-4000-85EA-04529895C3B5",
            PreferredRegions = new List<string> { "EastUS" }
        };

        PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
    }

    void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        if(response == null)
        {
            return;
        }

        Debug.Log("======= THESE ARE YOUR DETAILS ======= -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);

        UnityNetworkServer.Instance.networkAddress = response.IPV4Address;
        UnityNetworkServer.Instance.GetComponent<kcp2k.KcpTransport>().Port = (ushort)response.Ports[0].Num;

        UnityNetworkServer.Instance.StartClient();
    }

    void OnRequestMultiplayerServerError(PlayFabError playFabError)
    {
        Debug.Log("Error requesting server");
    }

    void OnLoginError(PlayFabError playFabError)
    {
        Debug.Log("Login Error!");
    }
}
