namespace PlayFab.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Mirror;
    using UnityEngine.Events;
    using Adrenak.UniVoice.InbuiltImplementations;
    using Adrenak.UniVoice;

    public class UnityNetworkServer : NetworkManager
    {
        ChatroomAgent agent;
        public string voiceChatroomName;
        public static UnityNetworkServer Instance { get; private set; }

        public PlayerEvent OnPlayerAdded = new PlayerEvent();
        public PlayerEvent OnPlayerRemoved = new PlayerEvent();

        public class NetworkConnectionToClientEvent : UnityEvent<NetworkConnectionToClient> { };
        public NetworkConnectionToClientEvent OnServerDisconnectEvent = new NetworkConnectionToClientEvent();

        public List<UnityNetworkConnection> Connections
        {
            get { return _connections; }
            private set { _connections = value; }
        }
        private List<UnityNetworkConnection> _connections = new List<UnityNetworkConnection>();

        public class PlayerEvent : UnityEvent<string> { }

        // Use this for initialization
        public override void Awake()
        {
            base.Awake();
            Instance = this;
            NetworkServer.RegisterHandler<ReceiveAuthenticateMessage>(OnReceiveAuthenticate);
            #if !UNITY_SERVER
            InitializeAgent();
            #endif
        }

        public void StartListen()
        {

            NetworkServer.Listen(maxConnections);
        }

        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            NetworkServer.Shutdown();
        }

        private void OnReceiveAuthenticate(NetworkConnectionToClient nconn, ReceiveAuthenticateMessage message)
        {
            var conn = _connections.Find(c => c.ConnectionId == nconn.connectionId);
            if (conn != null)
            {
                conn.PlayFabId = message.PlayFabId;
                conn.IsAuthenticated = true;
                OnPlayerAdded.Invoke(message.PlayFabId);
            }
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);

            Debug.LogWarning("Client Connected");
            var uconn = _connections.Find(c => c.ConnectionId == conn.connectionId);
            if (uconn == null)
            {
                _connections.Add(new UnityNetworkConnection()
                {
                    Connection = conn,
                    ConnectionId = conn.connectionId,
                    LobbyId = PlayFabMultiplayerAgentAPI.SessionConfig.SessionId
                });
            }
        }

        public override void OnServerError(NetworkConnectionToClient conn, Exception ex)
        {
            base.OnServerError(conn, ex);

            Debug.Log(string.Format("Unity Network Connection Status: exception - {0}", ex.Message));
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            OnServerDisconnectEvent.Invoke(conn);

            base.OnServerDisconnect(conn);

            var uconn = _connections.Find(c => c.ConnectionId == conn.connectionId);
            if (uconn != null)
            {
                if (!string.IsNullOrEmpty(uconn.PlayFabId))
                {
                    OnPlayerRemoved.Invoke(uconn.PlayFabId);
                }
                _connections.Remove(uconn);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            //Host/Join the voice chat room

            Debug.Log("Hosting Room");
            agent.Network.HostChatroom(voiceChatroomName);

        }

        public override void OnStartServer()
        {
            
            base.OnStartServer();
            /*
            hostingVCRoom = true;
            //Host the voice chat room
            if (agent.CurrentMode == ChatroomAgentMode.Unconnected)
            {
                Debug.Log("Hosting Room");
                agent.Network.HostChatroom(voiceChatroomName);

            }
            
            agent.MuteSelf = true;
            */
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            agent.Network.LeaveChatroom();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            agent.Network.CloseChatroom();
        }
        void InitializeAgent()
        {
            // 167.71.17.13:11000 is a test server hosted by the creator of UniVoice.
            // This server should NOT be used in any serious application or production as it
            // is neither secure nor gaurunteed to be online which will cause your
            // apps to fail. 
            // 
            // Host your own signalling server on something like DigitalOcean, AWS etc.
            // The test server is node based and its code is here: github.com/adrenak/airsignal
            agent = new InbuiltChatroomAgentFactory("ws://167.71.17.13:11000").Create();

            // HOSTING
            agent.Network.OnCreatedChatroom += () => {
                var chatroomName = agent.Network.CurrentChatroomName;
                ShowMessage($"Chatroom \"{chatroomName}\" created!\n" +
                $" You are Peer ID 0");
            };

            agent.Network.OnChatroomCreationFailed += ex => {
                ShowMessage("Chatroom creation failed");
                Debug.Log("Joining Room");
                agent.Network.JoinChatroom(voiceChatroomName);
                agent.MuteSelf = false;
            };

            agent.Network.OnlosedChatroom += () => {
                ShowMessage("You closed the chatroom! All peers have been kicked");

            };

            // JOINING
            agent.Network.OnJoinedChatroom += id => {
                var chatroomName = agent.Network.CurrentChatroomName;
                ShowMessage("Joined chatroom " + chatroomName);
                ShowMessage("You are Peer ID " + id);

            };

            agent.Network.OnChatroomJoinFailed += ex => {
                ShowMessage(ex);
            };

            agent.Network.OnLeftChatroom += () => {
                ShowMessage("You left the chatroom");

            };

            // PEERS

            /*
            agent.Network.OnPeerJoinedChatroom += id => {
                var view = Instantiate(peerViewTemplate, peerViewContainer);
                view.IncomingAudio = !agent.PeerSettings[id].muteThem;
                view.OutgoingAudio = !agent.PeerSettings[id].muteSelf;

                view.OnIncomingModified += value =>
                    agent.PeerSettings[id].muteThem = !value;

                view.OnOutgoingModified += value =>
                    agent.PeerSettings[id].muteSelf = !value;

                peerViews.Add(id, view);
                view.SetPeerID(id);
            };

            agent.Network.OnPeerLeftChatroom += id => {
                var peerViewInstance = peerViews[id];
                Destroy(peerViewInstance.gameObject);
                peerViews.Remove(id);
            };
            */
        }
        void ShowMessage(object obj)
        {
            Debug.Log("<color=blue>" + obj + "</color>");
            //menuMessage.text = obj.ToString();
            //if (agent.CurrentMode != ChatroomAgentMode.Unconnected)
            //    chatroomMessage.text = obj.ToString();
        }
    }

    [Serializable]
    public class UnityNetworkConnection
    {
        public bool IsAuthenticated;
        public string PlayFabId;
        public string LobbyId;
        public int ConnectionId;
        public NetworkConnection Connection;
    }

    public class CustomGameServerMessageTypes
    {
        public const short ReceiveAuthenticate = 900;
        public const short ShutdownMessage = 901;
        public const short MaintenanceMessage = 902;
    }

    public struct ReceiveAuthenticateMessage : NetworkMessage
    {
        public string PlayFabId;
    }

    public struct ShutdownMessage : NetworkMessage { }

    [Serializable]
    public struct MaintenanceMessage : NetworkMessage
    {
        public DateTime ScheduledMaintenanceUTC;
    }
}