namespace PlayFab.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Mirror;
    using UnityEngine.Events;
#if (UNITY_2018_3_OR_NEWER)
    using UnityEngine.Android;
#endif
    using agora_gaming_rtc;


    public class UnityNetworkServer : NetworkManager
    {
        public bool voiceChatEnabled;
        public string voiceChatroomName;
        public string agoraAppID;
        public string agoraToken;
        private IRtcEngine mRtcEngine = null;
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
#if (UNITY_2018_3_OR_NEWER)
            if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {

            }
            else
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#endif
            mRtcEngine = IRtcEngine.GetEngine(agoraAppID);

            mRtcEngine.OnJoinChannelSuccess += (string channelName, uint uid, int elapsed) =>
            {
                string joinSuccessMessage = string.Format("joinChannel callback uid: {0}, channel: {1}, version: {2}", uid, channelName, getAgoraSdkVersion());
                Debug.Log(joinSuccessMessage);
              
            };

            mRtcEngine.OnLeaveChannel += (RtcStats stats) =>
            {
                string leaveChannelMessage = string.Format("onLeaveChannel callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}", stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate);
                Debug.Log(leaveChannelMessage);
               
            };

            mRtcEngine.OnUserJoined += (uint uid, int elapsed) =>
            {
                string userJoinedMessage = string.Format("onUserJoined callback uid {0} {1}", uid, elapsed);
                Debug.Log(userJoinedMessage);
            };

            mRtcEngine.OnUserOffline += (uint uid, USER_OFFLINE_REASON reason) =>
            {
                string userOfflineMessage = string.Format("onUserOffline callback uid {0} {1}", uid, reason);
                Debug.Log(userOfflineMessage);
            };

            mRtcEngine.OnVolumeIndication += (AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume) =>
            {
                if (speakerNumber == 0 || speakers == null)
                {
                    Debug.Log(string.Format("onVolumeIndication only local {0}", totalVolume));
                }

                for (int idx = 0; idx < speakerNumber; idx++)
                {
                    string volumeIndicationMessage = string.Format("{0} onVolumeIndication {1} {2}", speakerNumber, speakers[idx].uid, speakers[idx].volume);
                    Debug.Log(volumeIndicationMessage);
                }
            };

            mRtcEngine.OnUserMutedAudio += (uint uid, bool muted) =>
            {
                string userMutedMessage = string.Format("onUserMuted callback uid {0} {1}", uid, muted);
                Debug.Log(userMutedMessage);
            };

            mRtcEngine.OnWarning += (int warn, string msg) =>
            {
                string description = IRtcEngine.GetErrorDescription(warn);
                string warningMessage = string.Format("onWarning callback {0} {1} {2}", warn, msg, description);
                Debug.Log(warningMessage);
            };

            mRtcEngine.OnError += (int error, string msg) =>
            {
                string description = IRtcEngine.GetErrorDescription(error);
                string errorMessage = string.Format("onError callback {0} {1} {2}", error, msg, description);
                Debug.Log(errorMessage);
            };

            mRtcEngine.OnRtcStats += (RtcStats stats) =>
            {
                string rtcStatsMessage = string.Format("onRtcStats callback duration {0}, tx: {1}, rx: {2}, tx kbps: {3}, rx kbps: {4}, tx(a) kbps: {5}, rx(a) kbps: {6} users {7}",
                    stats.duration, stats.txBytes, stats.rxBytes, stats.txKBitRate, stats.rxKBitRate, stats.txAudioKBitRate, stats.rxAudioKBitRate, stats.userCount);
                Debug.Log(rtcStatsMessage);

                int lengthOfMixingFile = mRtcEngine.GetAudioMixingDuration();
                int currentTs = mRtcEngine.GetAudioMixingCurrentPosition();

                string mixingMessage = string.Format("Mixing File Meta {0}, {1}", lengthOfMixingFile, currentTs);
                Debug.Log(mixingMessage);
            };

            mRtcEngine.OnAudioRouteChanged += (AUDIO_ROUTE route) =>
            {
                string routeMessage = string.Format("onAudioRouteChanged {0}", route);
                Debug.Log(routeMessage);
            };

            mRtcEngine.OnRequestToken += () =>
            {
                string requestKeyMessage = string.Format("OnRequestToken");
                Debug.Log(requestKeyMessage);
            };

            mRtcEngine.OnConnectionInterrupted += () =>
            {
                string interruptedMessage = string.Format("OnConnectionInterrupted");
                Debug.Log(interruptedMessage);
            };

            mRtcEngine.OnConnectionLost += () =>
            {
                string lostMessage = string.Format("OnConnectionLost");
                Debug.Log(lostMessage);
            };

            mRtcEngine.SetLogFilter(LOG_FILTER.INFO);

            mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_COMMUNICATION);

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
            base.OnApplicationQuit();
            if (mRtcEngine != null)
            {
                IRtcEngine.Destroy();
            }
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
            if (voiceChatEnabled)
            {
                JoinVoiceChannel();
            }
            

        }

        public override void OnStartServer()
        {
            
            base.OnStartServer();

        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            if (voiceChatEnabled)
            {
                LeaveVoiceChannel();
            }

        }

        public override void OnStopServer()
        {
            base.OnStopServer();
           
        }


        public void JoinVoiceChannel()
        {


            Debug.Log(string.Format("tap joinChannel with channel name {0}", voiceChatroomName));

            if (string.IsNullOrEmpty(voiceChatroomName))
            {
                return;
            }

            mRtcEngine.JoinChannelByKey(agoraToken, voiceChatroomName, "extra", 0);
        }

        public void LeaveVoiceChannel()
        {
            mRtcEngine.LeaveChannel();
            Debug.Log(string.Format("left channel name {0}", voiceChatroomName));
        }

        public string getAgoraSdkVersion()
        {
            string ver = IRtcEngine.GetSdkVersion();
            return ver;
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