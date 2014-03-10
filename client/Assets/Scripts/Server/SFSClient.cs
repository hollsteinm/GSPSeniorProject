using UnityEngine;
using System.Collections;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X;

[System.Serializable]
public class SFSClient : IClient{
    private SmartFox SFSInstance = new SmartFox();
    private static SFSClient client;
    public LogLevel logLevel = LogLevel.DEBUG;
    private static object mutex = new object();
    public bool debug;
    public string server;
    public int port;
    public string username;
    private string room;
    private string zone;
    private string currentMessage;

    //singleton caller
    public static SFSClient Singleton {
        get {
            if (client == null) {
                lock (mutex) {
                    if (client == null) {
                        client = new SFSClient();
                    }
                }
            }
            return client;
        }
    }

    private SFSClient() {
        if (Application.isWebPlayer) {
            Security.PrefetchSocketPolicy(server, port, 500);
        }

        SFSInstance.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        SFSInstance.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        SFSInstance.AddEventListener(SFSEvent.LOGIN, OnLogin);

        SFSInstance.AddLogListener(logLevel, OnDebugMessage);
    }

    //SFS callbacks
    private void OnLogin(BaseEvent evt) {
        if (evt.Params.Contains("success") && !(bool)evt.Params["success"]) {
            // Login failed
            currentMessage = (string)evt.Params["errorMessage"];
            Debug.Log("Login error: " + currentMessage);
        } else {
            // On to the lobby
            currentMessage = "Successful Login.";
            Application.LoadLevel("lobby");
        }
    }

    private void OnConnection(BaseEvent evt) {
        bool success = (bool)evt.Params["success"];
        if (success) {
            currentMessage = "Connection successful.";
        } else {
            currentMessage = "Cannot connect to the server.";
        }
        Debug.Log(currentMessage);
    }

    private void OnConnectionLost(BaseEvent evt) {
        currentMessage = "Connection lost; reason: " + (string)evt.Params["reason"];
    }

    private void OnDebugMessage(BaseEvent evt) {
        Debug.Log("[SFS DEBUG]: " + (string)evt.Params["message"]);
    }

    //end SFS callbacks
    //interface methods
    public void Login(string username, string password) {
        this.username = username;
        //TODO: implement using a password.
        SFSInstance.Send(new LoginRequest(this.username, "", "StarboundAces"));
    }

    public void Disconnect() {
        SFSInstance.Disconnect();
    }

    public void Connect(string server, int port) {
        if (!SFSInstance.IsConnected) {
            SFSInstance.Connect(server, port);
        }
    }

    public void SendRequest(string name, object data) {
    }

    public void Update() {
        SFSInstance.ProcessEvents();
    }

    public void OnResponse() {
    }

    public string Username {
        get {
            return username;
        }
    }

    public string Server {
        get {
            return server;
        }
    }

    public int Port {
        get {
            return port;
        }
    }

    public string Room {
        get {
            return room;
        }
    }

    public string Zone {
        get {
            return zone;
        }
    }
    //end interface methods
}