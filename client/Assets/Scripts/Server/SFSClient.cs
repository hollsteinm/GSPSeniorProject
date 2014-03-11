using UnityEngine;
using System.Collections;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using Sfs2X;
using System.Collections.Generic;

[System.Serializable]
public class SFSClient : IClient , IEventMessenger{
    private SmartFox SFSInstance;
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
    private List<IEventListener> listeners = new List<IEventListener>();

    //names of 'levels/scenes' for each respective part.
    public string lobby;
    public string login;
    public string game;

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
        SFSInstance = new SmartFox(debug);

        if (Application.isWebPlayer || Application.isEditor) {
            Security.PrefetchSocketPolicy(server, port, 500);
        }

        SFSInstance.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        SFSInstance.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

        SFSInstance.AddEventListener(SFSEvent.LOGIN, OnLogin);
        SFSInstance.AddEventListener(SFSEvent.LOGOUT, OnLogout);

        SFSInstance.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionReponse);
        SFSInstance.AddEventListener(SFSEvent.PUBLIC_MESSAGE, OnPublicMessage);

        SFSInstance.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
        SFSInstance.AddEventListener(SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError);
        SFSInstance.AddEventListener(SFSEvent.ROOM_ADD, OnRoomAdd);
        SFSInstance.AddEventListener(SFSEvent.ROOM_REMOVE, OnRoomRemove);
        SFSInstance.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        SFSInstance.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);

        SFSInstance.AddLogListener(logLevel, OnDebugMessage);
        SFSInstance.InitUDP();
    }

    //SFS callbacks
    private void OnExtensionReponse(BaseEvent evt) {
        string cmd = (string)evt.Params["cmd"];
        OnEvent("server", (BaseEvent)evt);
    }

    private void OnLogout(BaseEvent evt) {
    }

    private void OnPublicMessage(BaseEvent evt) {
    }

    private void OnJoinRoom(BaseEvent evt) {
    }

    private void OnRoomCreationError(BaseEvent evt) {
    }

    private void OnRoomAdd(BaseEvent evt) {
    }

    private void OnRoomRemove(BaseEvent evt) {
    }

    private void OnUserEnterRoom(BaseEvent evt) {
    }

    private void OnUserExitRoom(BaseEvent evt) {
    }

    private void OnLogin(BaseEvent evt) {
        if (evt.Params.Contains("success") && !(bool)evt.Params["success"]) {
            // Login failed
            currentMessage = (string)evt.Params["errorMessage"];
            Debug.Log("Login error: " + currentMessage);
        } else {
            // On to the lobby
            currentMessage = "Successful Login.";
            Application.LoadLevel(lobby);
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
    //start client interface methods
    public void Login(string username, string password) {
        this.username = username;
        //TODO: implement using a password.
        SFSInstance.Send(new LoginRequest(this.username, "", "StarboundAces"));
    }

    public void Disconnect() {
        if(SFSInstance.IsConnected){
            SFSInstance.Disconnect();
        }
    }

    public void Connect(string server, int port) {
        if (!SFSInstance.IsConnected) {
            SFSInstance.Connect(server, port);
        }
    }

    public void Send(DataType type, object data){
        SFSObject sfso = new SFSObject();
        switch (type) {
            case DataType.transform:
                Transform t = data as Transform;
                float[] position = {t.position.x, t.position.y, t.position.z};
                float[] rotation = {t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w};

                sfso.PutFloatArray("position", position);
                sfso.PutFloatArray("rotation", rotation);

                SFSInstance.Send(new ExtensionRequest("transform", sfso, SFSInstance.LastJoinedRoom, true));
                
                break;
            case DataType.charmessage:
                string message = data as string;
                SFSInstance.Send(new PublicMessageRequest(message, null, SFSInstance.LastJoinedRoom));

                break;
            default:
                Debug.LogError("Should not reach this point in Send(GameObject, SendType, object)");
                break;
        }
    }

    public void Update() {
        SFSInstance.ProcessEvents();
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
    //end client interface methods
    //start eventmessenger interface methods
    public void Register(IEventListener listener){
        if(!listeners.Contains(listener)){
            listeners.Add(listener);
        }
    }

    public void Unregister(IEventListener listener){
        if(listeners.Contains(listener)){
            listeners.Remove(listener);
        }
    }

    public void OnEvent(string eventType, object o){
        lock(mutex){
            foreach(IEventListener el in listeners){
                el.Notify(eventType, o);
            }
        }
    }
    //end eventmessenger interface methods
}