﻿using UnityEngine;
using System.Collections;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using Sfs2X.Entities;
using Sfs2X;
using System.Collections.Generic;

[System.Serializable]
public class SFSClient : IClientController{
    private SmartFox SFSInstance;
    private static SFSClient client;
    public LogLevel logLevel = LogLevel.DEBUG;
    private static object mutex = new object();
    public bool debug;
    private string username = "";
    private string server = "";
    private int port = 0;
    private string zone = "";
    private string room = "";
    private string currentMessage;
    private List<IEventListener> listeners = new List<IEventListener>();
    private bool useUDP = false;

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
        SFSInstance.AddEventListener ( SFSEvent.UDP_INIT, OnUDPInit );

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
    }

    //SFS callbacks
    private void OnExtensionReponse(BaseEvent evt) {
        string cmd = (string)evt.Params["cmd"];
        SFSObject sfsdata = (SFSObject)evt.Params["params"];

        switch ( cmd ) {
            case "transform":
                int id = sfsdata.GetInt ( "player" );

                //clever nullreference check, or annoying statement?
                float px = (sfsdata.GetFloat ( "position.x" ) != null ? sfsdata.GetFloat("position.x") : 0.0f);
                float py = (sfsdata.GetFloat ( "position.y" ) != null ? sfsdata.GetFloat("position.y") : 0.0f);
                float pz = (sfsdata.GetFloat ( "position.z" ) != null ? sfsdata.GetFloat("position.z") : 0.0f);
                float rx = (sfsdata.GetFloat ( "rotation.x" ) != null ? sfsdata.GetFloat("rotation.x") : 0.0f);
                float ry = (sfsdata.GetFloat ( "rotation.y" ) != null ? sfsdata.GetFloat("rotation.y") : 0.0f);
                float rz = (sfsdata.GetFloat ( "rotation.z" ) != null ? sfsdata.GetFloat("rotation.z") : 0.0f);
                float rw = (sfsdata.GetFloat ( "rotation.w" ) != null ? sfsdata.GetFloat("rotation.w") : 0.0f);

                //ignore my own updates of position from server
                if (id != SFSInstance.MySelf.Id) {
                    if (!GameManager.gameManager.Players.ContainsKey(id)) {
                        Debug.LogError("Player does not exist! What happened!?!?");
                    }

                    GameObject other = GameManager.gameManager.Players[id];

                    other.transform.position = new Vector3(px, py, pz);
                    other.transform.rotation = new Quaternion(rx, ry, rz, rw);
                } else {
                    //server is my boss, tell me where to go (mostly for spawns)
                    GameObject player = GameManager.gameManager.ClientPlayer;
                    player.transform.position = new Vector3(px, py, pz);
                    player.transform.rotation = new Quaternion(rx, ry, rz, rw);
                }

                break;
            default:
                break;
        }       
    }

    private void OnUDPInit ( BaseEvent evt ) {
        if ( (bool)evt.Params["success"]) {
            Debug.Log ( "UPD Initialized" );
            useUDP = true;
        } else {
            Debug.LogWarning ( "UPD Not Initialized, using TCP" );
        }
    }

    private void OnLogout(BaseEvent evt) {
        Debug.Log ( "[User Logged Out: " + ( ( User ) evt.Params[ "user" ] ).Name +"]");
    }

    private void OnPublicMessage(BaseEvent evt) {
        Debug.Log("[Public Message]: " + (string)evt.Params["message"]);
    }

    private void OnJoinRoom(BaseEvent evt) {
        Room room = ( Room ) evt.Params[ "room" ];
        Debug.Log ( "[Room Joined: " + room.Name + "]" );
        if ( room.IsGame ) {
            Application.LoadLevel ( "multiplayer" );
        }
        this.room = room.Name;
    }

    private void OnRoomCreationError(BaseEvent evt) {
        Debug.LogError ( "[Room Creation Error" + ( string ) evt.Params[ "error" ] + "]" );
    }

    private void OnRoomAdd(BaseEvent evt) {
        Debug.Log ( "[Room Added: " + ( ( Room ) evt.Params[ "room" ] ).Name + "]" );
        OnEvent ( "roomadd", ( string ) ( ( Room ) evt.Params[ "room" ] ).Name );
    }

    private void OnRoomRemove(BaseEvent evt) {
        Debug.Log ( "[Room Removed: " + ( ( Room ) evt.Params[ "room" ] ).Name + "]" );
    }

    private void OnUserEnterRoom(BaseEvent evt) {
        User user = (User)evt.Params["user"];
        Debug.Log ( "[User Enter Room: " + user.Name + "]" );
        Room room = (Room)evt.Params["room"];
 
        if (room.IsGame) {
            int id = user.Id;
            if (!GameManager.gameManager.Players.ContainsKey(id) && id != SFSInstance.MySelf.Id) {
                GameManager.gameManager.AddRemotePlayer(id, user.Name);
            }
        }
    }

    private void OnUserExitRoom(BaseEvent evt) {
        Debug.Log ( "[User Exit Room: " + ( ( User ) evt.Params[ "user" ] ).Name + "]" );
    }

    private void OnLogin(BaseEvent evt) {
        if (evt.Params.Contains("success") && !(bool)evt.Params["success"]) {
            // Login failed
            currentMessage = (string)evt.Params["errorMessage"];
            Debug.Log("Login error: " + currentMessage);
        } else {
            // On to the lobby
            currentMessage = "Successful Login.";
            Application.LoadLevel ( "lobby" );
        }
    }

    private void OnConnection(BaseEvent evt) {
        bool success = (bool)evt.Params["success"];
        if (success) {
            currentMessage = "Connection successful.";
        } else {
            currentMessage = "Cannot connect to the server.";
        }
        SFSInstance.InitUDP (server, port);
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
        SFSInstance.Send(new LoginRequest(this.username, password, "StarboundAces"));
    }

    public void Disconnect() {
        if(SFSInstance.IsConnected){
            SFSInstance.Disconnect();
        }
    }

    public void Connect(string server, int port) {
        this.server = server;
        this.port = port;
        if (!SFSInstance.IsConnected) {
            SFSInstance.Connect(server, port);
            while ( SFSInstance.IsConnecting ) {
                //block and show some kind of connection graphics
                Debug.Log ( "Connecting...\n" );
            }
        }
    }

    public void Send(DataType type, object data){
        switch (type) {
            case DataType.TRANSFORM:
                SendTransform ( data );                
                break;

            case DataType.CHARMESSAGE:
                SendCharMessage ( data );
                break;

            case DataType.ROOMREQUEST:
                SendRoomRequest ( data );
                break;

            case DataType.ROOMJOIN:
                SendRoomJoinRequest ( data );
                break;

            default:
                Debug.LogError("Should not reach this point in Send( SendType, object)");
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
    //methods for send()
    private void SendTransform ( object data ) {
        SFSObject sfso = new SFSObject ();
        Transform t = data as Transform;

        sfso.PutFloat ( "position.x", t.position.x );
        sfso.PutFloat ( "position.y", t.position.y );
        sfso.PutFloat ( "position.z", t.position.z );
        sfso.PutFloat ( "rotation.x", t.rotation.x );
        sfso.PutFloat ( "rotation.y", t.rotation.y );
        sfso.PutFloat ( "rotation.z", t.rotation.z );
        sfso.PutFloat ( "rotation.w", t.rotation.w );

        SFSInstance.Send ( new ExtensionRequest ( "server.transform", sfso, null, useUDP) );
    }

    private void SendCharMessage ( object data ) {
        if ( SFSInstance.LastJoinedRoom == null ) {
            Debug.LogError ( "Cannot send CharMessage request as not in room" );
        } else {
            string message = data as string;
            SFSInstance.Send ( new PublicMessageRequest ( message, null ) );
        }
    }

    private void SendRoomRequest ( object data ) {
        RoomSettings settings = new RoomSettings ( SFSInstance.MySelf.Name + "'s game." );
        settings.Extension = new RoomExtension("starboundaces", "com.gspteama.main.StarboundAcesExtension");
        settings.MaxUsers = 2;
        settings.IsGame = true;
        SFSInstance.Send ( new CreateRoomRequest ( settings, false ) );
        SendRoomJoinRequest ( settings.Name );
    }

    private void SendRoomJoinRequest ( object data ) {
        if ( SFSInstance.LastJoinedRoom != null ) {
            //no private games/passwords supported as of yet.
            SFSInstance.Send ( new JoinRoomRequest ( data, "", SFSInstance.LastJoinedRoom.Id, false ) );
        } else {
            SFSInstance.Send ( new JoinRoomRequest ( data ) );
        }
    }
    //end methods for send()
}