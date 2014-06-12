using UnityEngine;
using System.Collections;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Entities;
using Sfs2X;
using System.Collections.Generic;

[System.Serializable]
public class SFSClient : IClientController {
    private SmartFox SFSInstance;
    private static SFSClient client;
    public LogLevel logLevel = LogLevel.DEBUG;
    private static object mutex = new object ();
    public bool debug;
    private string username = "";
    private string server = "";
    private int port = 0;
    private string zone = "";
    private string room = "";
    private string currentMessage;
    private List<IEventListener> listeners = new List<IEventListener> ();
    private bool useUDP = false;

    //names of 'levels/scenes' for each respective part.
    public string lobby;
    public string login;
    public string game;

    public SmartFox SmartFoxInstance
    {
        get
        {
            return SFSInstance;
        }
    }

    //singleton caller
    public static SFSClient Singleton {
        get {
            if ( client == null ) {
                lock ( mutex ) {
                    if ( client == null ) {
                        client = new SFSClient ();
                    }
                }
            }
            return client;
        }
    }

    private void RegisterCallbacks ( ) {
        SFSInstance.AddEventListener ( SFSEvent.CONNECTION, OnConnection );
        SFSInstance.AddEventListener ( SFSEvent.CONNECTION_LOST, OnConnectionLost );
        SFSInstance.AddEventListener ( SFSEvent.UDP_INIT, OnUDPInit );

        SFSInstance.AddEventListener ( SFSEvent.LOGIN, OnLogin );
        SFSInstance.AddEventListener ( SFSEvent.LOGOUT, OnLogout );
        SFSInstance.AddEventListener ( SFSEvent.LOGIN_ERROR, OnLoginError );

        SFSInstance.AddEventListener ( SFSEvent.EXTENSION_RESPONSE, OnExtensionReponse );
        SFSInstance.AddEventListener ( SFSEvent.PUBLIC_MESSAGE, OnPublicMessage );

        SFSInstance.AddEventListener ( SFSEvent.ROOM_JOIN, OnJoinRoom );
        SFSInstance.AddEventListener ( SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError );
        SFSInstance.AddEventListener ( SFSEvent.ROOM_ADD, OnRoomAdd );
        SFSInstance.AddEventListener ( SFSEvent.ROOM_REMOVE, OnRoomRemove );

        SFSInstance.AddEventListener ( SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom );
        SFSInstance.AddEventListener ( SFSEvent.USER_EXIT_ROOM, OnUserExitRoom );
    }

    private void UnregisterCallbacks ( ) {
        SFSInstance.RemoveEventListener ( SFSEvent.CONNECTION, OnConnection );
        SFSInstance.RemoveEventListener ( SFSEvent.CONNECTION_LOST, OnConnectionLost );
        SFSInstance.RemoveEventListener ( SFSEvent.UDP_INIT, OnUDPInit );

        SFSInstance.RemoveEventListener ( SFSEvent.LOGIN, OnLogin );
        SFSInstance.RemoveEventListener ( SFSEvent.LOGOUT, OnLogout );
        SFSInstance.RemoveEventListener ( SFSEvent.LOGIN_ERROR, OnLoginError );

        SFSInstance.RemoveEventListener ( SFSEvent.EXTENSION_RESPONSE, OnExtensionReponse );
        SFSInstance.RemoveEventListener ( SFSEvent.PUBLIC_MESSAGE, OnPublicMessage );

        SFSInstance.RemoveEventListener ( SFSEvent.ROOM_JOIN, OnJoinRoom );
        SFSInstance.RemoveEventListener ( SFSEvent.ROOM_CREATION_ERROR, OnRoomCreationError );
        SFSInstance.RemoveEventListener ( SFSEvent.ROOM_ADD, OnRoomAdd );
        SFSInstance.RemoveEventListener ( SFSEvent.ROOM_REMOVE, OnRoomRemove );

        SFSInstance.RemoveEventListener ( SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom );
        SFSInstance.RemoveEventListener ( SFSEvent.USER_EXIT_ROOM, OnUserExitRoom );
    }

    private SFSClient ( ) {
        SFSInstance = new SmartFox ( debug );
        Application.runInBackground = true;

        RegisterCallbacks ();

        SFSInstance.AddLogListener ( logLevel, OnDebugMessage );
    }

    //SFS callbacks
    public void OnExtensionReponse ( BaseEvent evt ) {
        string cmd = ( string ) evt.Params[ "cmd" ];
        SFSObject sfsdata = ( SFSObject ) evt.Params[ "params" ];

        switch ( cmd ) {
            case "gamelist":
                GameListResponse(sfsdata);
                break;

            case "transform":
                TransformResponse ( sfsdata );
                break;

            case "player.hit":
                PlayerHitResponse ( sfsdata );
                break;

            case "death":
                DeathResponse ( sfsdata );
                break;

            case "spawn":
                SpawnResponse ( sfsdata );
                break;

            case "$SignUp.Submit":
                SignUpResponse(sfsdata);
                break;

            case "scores":
                ScoresResponse(sfsdata);
                break;

            case "shoot":
                ShootResponse(sfsdata);
                break;

            case "game.start":
                GameStartResponse(sfsdata);
                break;

            case "gamelist.remove":
                GameListRemoveResponse(sfsdata);
                break;

            case "projectile.expire":
                int id = sfsdata.GetInt("networkId");
                OnEvent("projectile.expire", id);
                break;

            case "powerup":
                PowerUpResponse(sfsdata);
                break;

            default:
                break;
        }
    }

    private void OnUDPInit ( BaseEvent evt ) {
        if ( ( bool ) evt.Params[ "success" ] ) {
            Debug.Log ( "UPD Initialized" );
            useUDP = true;
        } else {
            Debug.LogWarning ( "UPD Not Initialized, using TCP" );
        }
    }

    private void OnLogout ( BaseEvent evt ) {
        SFSInstance.Disconnect();
        UnregisterCallbacks();
        Debug.Log ( "User Logged Out" );
    }

    private void OnPublicMessage ( BaseEvent evt ) {
        string message = ( string ) evt.Params[ "message" ];
        OnEvent ( "charmessage", message );
    }

    private void OnLoginError( BaseEvent evt ) {
        string message = (string)evt.Params["errorMessage"];
        Debug.Log(message);
    }

    private void OnJoinRoom ( BaseEvent evt ) {
        Room room = ( Room ) evt.Params[ "room" ];
        Debug.Log ( "[Room Joined: " + room.Name + "]" );
        if ( room.IsGame ) {
            UnregisterCallbacks ();
            Application.LoadLevel ( "queue" );
            RegisterCallbacks ();

            List<User> users = SFSInstance.LastJoinedRoom.UserList;
            foreach ( User u in users ) {
                if ( u.Id != SFSInstance.MySelf.Id ) {
                    GameManager.gameManager.AddRemotePlayer(u.Id, u.Name);
                    Debug.Log("User " + u.Name + " is in room.");
                }
            }
        } else if ( room.Name == "lobby" ) {
            UnregisterCallbacks ();
            Application.LoadLevel ( "lobby" );
            RegisterCallbacks ();
        }
        this.room = room.Name;
    }

    private void OnRoomCreationError ( BaseEvent evt ) {
        Debug.LogError ( "[Room Creation Error" + ( string ) evt.Params[ "error" ] + "]" );
    }

    private void OnRoomAdd ( BaseEvent evt ) {
        Debug.Log ( "[Room Added: " + ( ( Room ) evt.Params[ "room" ] ).Name + "]" );
        OnEvent ( "roomadd", ( string ) ( ( Room ) evt.Params[ "room" ] ).Name );
    }

    private void OnRoomRemove ( BaseEvent evt ) {
        Debug.Log ( "[Room Removed: " + ( ( Room ) evt.Params[ "room" ] ).Name + "]" );
        OnEvent("roomremove", (string)((Room)evt.Params["room"]).Name);
    }

    private void OnUserEnterRoom ( BaseEvent evt ) {
        User user = ( User ) evt.Params[ "user" ];
        Debug.Log ( "[User Enter Room: " + user.Name + "]" );
        Room room = ( Room ) evt.Params[ "room" ];

        if ( room.IsGame ) {
            int id = user.Id;
            if ( !GameManager.gameManager.Players.ContainsKey ( id ) && id != SFSInstance.MySelf.Id ) {
                GameManager.gameManager.AddRemotePlayer ( id, user.Name );
            }
        }
    }

    private void OnUserExitRoom ( BaseEvent evt ) {
        Debug.Log ( "[User Exit Room (" + ( ( Room ) evt.Params[ "room" ] ).Name + "): " + ( ( User ) evt.Params[ "user" ] ).Name + "]" );
        if ( ( ( Room ) evt.Params[ "room" ] ).IsGame ) {
            GameManager.gameManager.RemoveRemotePlayer ( ( ( User ) evt.Params[ "user" ] ).Id );
        }
    }

    private void OnLogin ( BaseEvent evt ) {
        
        if ( evt.Params.Contains ( "success" ) && !( bool ) evt.Params[ "success" ] ) {
            // Login failed
            currentMessage = ( string ) evt.Params[ "errorMessage" ];
            Debug.Log ( "Login error: " + currentMessage );
            //SFSInstance.InitUDP();
        } else {
            // On to the lobby
            currentMessage = "Successful Login.";

            if (Application.loadedLevelName != "register") {
                SendRoomJoinRequest("lobby");
            } else {
                OnEvent("login.success", null);
            }
        }
    }

    private void OnConnection ( BaseEvent evt ) {
        bool success = ( bool ) evt.Params[ "success" ];
        if ( success ) {
            currentMessage = "Connection successful.";
        } else {
            currentMessage = "Cannot connect to the server.";
            currentMessage += evt.Params["errorMessage"];
        }
        Debug.Log ( currentMessage );
    }

    private void OnConnectionLost ( BaseEvent evt ) {
        currentMessage = "Connection lost; reason: " + ( string ) evt.Params[ "reason" ];
    }

    private void OnDebugMessage ( BaseEvent evt ) {
        Debug.Log ( "[SFS DEBUG]: " + ( string ) evt.Params[ "message" ] );
    }

    //end SFS callbacks
    //start client interface methods
    public void Login ( string username, string password ) {
        this.username = username;
        if (!password.Equals("")) {
            password = Sfs2X.Util.PasswordUtil.MD5Password(password);
        }
        SFSInstance.Send ( new LoginRequest ( this.username, password, "StarboundAces" ) );
    }

    public void Logout() {
        SFSInstance.Send(new LogoutRequest());
    }

    public void Disconnect ( ) {
        if (SFSInstance.IsConnected) {
            Logout();
        }
    }

    public void Connect ( string server, int port ) {
        this.server = server;
        this.port = port;

        if (Application.isWebPlayer || Application.isEditor) {
            string dnsIp = System.Net.Dns.GetHostAddresses(this.server)[0].ToString();
            Debug.Log("IP for Prefetch: " + dnsIp);
            if (!Security.PrefetchSocketPolicy(dnsIp, port, 500)) {
                Debug.LogError("PrefetchSocketPolicy failure.");
            }
        }

        if ( !SFSInstance.IsConnected ) {
            SFSInstance.Connect ( server, port );
            while ( SFSInstance.IsConnecting ) {
                //block and show some kind of connection graphics
            }
        }
    }

    public void Send ( DataType type, object data ) {
        switch ( type ) {
            case DataType.GAMES_GET:
                SendGetGame(data);
                break;

            case DataType.TRANSFORM:
                SendTransform ( data );
                break;

            case DataType.CHARMESSAGE:
                SendCharMessage ( data );
                break;

            case DataType.MAKEGAME:
                SendRoomRequest ( data );
                break;

            case DataType.JOINGAME:
                SendRoomJoinRequest ( data );
                break;

            case DataType.SPAWNED:
                SendSpawnRequest ( data );
                break;

            case DataType.FIRE:
                SendFireRequest ( data );
                break;

            case DataType.DEATH:
                SendDeathRequest ( data );
                break;

            case DataType.REGISTER:
                SendRegistrationRequest(data);
                break;

            case DataType.SCORES_GET:
                SendGetScoresRequest(data);
                break;

            case DataType.SHOOT:
                SendShootRequest(data);
                break;

            case DataType.PLAYER_GAME_READY:
                SendGameStartRequest(data);
                break;

            case DataType.POWERUP:
                SendPowerUpRequest(data);
                break;

            default:
                Debug.LogError ( "Should not reach this point in Send( SendType, object)" );
                break;
        }
    }

    public void Update ( ) {
        SFSInstance.ProcessEvents ();
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
    public void Register ( IEventListener listener ) {
        if ( !listeners.Contains ( listener ) ) {
            listeners.Add ( listener );
        }
    }

    public void Unregister ( IEventListener listener ) {
        if ( listeners.Contains ( listener ) ) {
            listeners.Remove ( listener );
        }
    }

    public void OnEvent ( string eventType, object o ) {
        lock ( mutex ) {
            foreach ( IEventListener el in listeners ) {
                el.Notify ( eventType, o );
            }
        }
    }
    //end eventmessenger interface methods

    private void CheckType ( object data, System.Type expected ) {
        if ( data.GetType () != expected ) {
            Debug.LogError ( "Wrong Type passed for message!\n[Expected]: " + expected.ToString () + "\n[Actual]: " + data.GetType ().ToString () );
        }
    }

    //methods for send()
    private void SendShootRequest(object data){
        Dictionary<string, object> cdata = data as Dictionary<string, object>;
        SFSObject sfsdata = new SFSObject();

        sfsdata.PutFloat("position.x", (float)cdata["position.x"]);
        sfsdata.PutFloat("position.y", (float)cdata["position.y"]);
        sfsdata.PutFloat("position.z", (float)cdata["position.z"]);
        sfsdata.PutFloat("rotation.x", (float)cdata["rotation.x"]);
        sfsdata.PutFloat("rotation.y", (float)cdata["rotation.y"]);
        sfsdata.PutFloat("rotation.z", (float)cdata["rotation.z"]);
        sfsdata.PutFloat("rotation.w", (float)cdata["rotation.w"]);

        sfsdata.PutUtfString("type", (string)cdata["type"]);

        SFSInstance.Send(new ExtensionRequest("server.shoot", sfsdata));

    }

    private void SendGetGame(object data){
        SFSInstance.Send(new ExtensionRequest("server.gameslist", new SFSObject()));
    }

    private void SendRegistrationRequest(object data) {
        Dictionary<string, string> cdata = data as Dictionary<string, string>;
        SFSObject sfsdata = new SFSObject();
        sfsdata.PutUtfString("user_name", cdata["username"]);
        sfsdata.PutUtfString("user_password", cdata["password"]);
        sfsdata.PutUtfString("user_email", cdata["email"]);

        SFSUserVariable userVar = new SFSUserVariable("username", cdata["username"]);
        SFSInstance.MySelf.SetVariable(userVar);

        SFSInstance.Send(new SetUserVariablesRequest(SFSInstance.MySelf.GetVariables()));
        SFSInstance.Send(new ExtensionRequest("$SignUp.Submit", sfsdata));
    }

    private void SendPowerUpRequest(object data) {
        CheckType(data, typeof(string));
        SFSObject sfsdata = new SFSObject();
        sfsdata.PutUtfString("powerup", (string)data);
        SFSInstance.Send(new ExtensionRequest("server.powerup", sfsdata, SFSInstance.LastJoinedRoom));
    }

    private void SendSpawnRequest ( object data ) {
        SFSObject request = new SFSObject();
        request.PutUtfString("weaponType", (string)data);
        Debug.Log((string)data);
        SFSInstance.Send ( new ExtensionRequest ( "server.spawn", request ) );
    }

    private void SendTransform ( object data ) {
        SFSObject sfso = new SFSObject ();
        Dictionary<string, object> cdata = data as Dictionary<string, object>;
        Transform t = cdata["transform"] as Transform;
        string type = cdata["type"] as string;
        if (type == "projectile") {
            sfso.PutInt("networkId", (int)cdata["networkId"]);
        }
        sfso.PutUtfString("type", type);    
        
        sfso.PutFloat("position.x", t.position.x);
        sfso.PutFloat("position.y", t.position.y);
        sfso.PutFloat("position.z", t.position.z);
        sfso.PutFloat("rotation.x", t.rotation.x);
        sfso.PutFloat("rotation.y", t.rotation.y);
        sfso.PutFloat("rotation.z", t.rotation.z);
        sfso.PutFloat("rotation.w", t.rotation.w);

        SFSInstance.Send ( new ExtensionRequest ( "server.transform", sfso ) );//, null, useUDP) );
    }

    private void SendCharMessage ( object data ) {
        string message = SFSInstance.MySelf.Name + " said: " + data as string;
        if ( SFSInstance.LastJoinedRoom == null ) {
            Debug.LogError("User not in room, cannot send message.");
        } else {
            SFSInstance.Send ( new PublicMessageRequest ( message, new SFSObject (), SFSInstance.LastJoinedRoom ) );
        }
    }

    private void SendRoomRequest ( object data ) {
        RoomSettings settings = new RoomSettings ( SFSInstance.MySelf.Name + "'s game." );
        settings.Extension = new RoomExtension ( "StarboundAcesExtension", "com.gspteama.main.StarboundAcesExtension" );
        settings.MaxUsers = 8;
        settings.IsGame = true;
        SFSInstance.Send ( new CreateRoomRequest ( settings, true, SFSInstance.LastJoinedRoom ) );
    }

    private void SendRoomJoinRequest ( object data ) {
        if ( SFSInstance.LastJoinedRoom != null ) {
            //no private games/passwords supported as of yet.
            SFSInstance.Send ( new JoinRoomRequest ( data, "", SFSInstance.LastJoinedRoom.Id, false ) );
        } else {
            SFSInstance.Send ( new JoinRoomRequest ( data));
        }
    }

    private void SendFireRequest ( object data ) {
        SFSObject sfsdata = new SFSObject ();
        Dictionary<string, object> firedata = data as Dictionary<string, object>;

        sfsdata.PutFloat ( "damage", ( float ) firedata[ "damage" ] );
        sfsdata.PutUtfString("type", (string)firedata["type"]);
        sfsdata.PutInt ( "player.hit.id", ( int ) firedata[ "player.hit.id" ] );

        sfsdata.PutFloat ( "contact.point.x", ( float ) firedata[ "contact.point.x" ] );
        sfsdata.PutFloat ( "contact.point.y", ( float ) firedata[ "contact.point.y" ] );
        sfsdata.PutFloat ( "contact.point.z", ( float ) firedata[ "contact.point.z" ] );

        SFSInstance.Send ( new ExtensionRequest ( "server.fire", sfsdata, SFSInstance.LastJoinedRoom, useUDP ) );
    }

    private void SendDeathRequest ( object data ) {
        SFSObject sfsdata = new SFSObject ();
        if (data != null) {
            Dictionary<string, int> d = data as Dictionary<string, int>;
            if (d.ContainsKey("isProjectile")) {
                sfsdata.PutInt("networkId", d["networkId"]);
            }
        }
        SFSInstance.Send ( new ExtensionRequest ( "server.death", sfsdata, SFSInstance.LastJoinedRoom ) );
        Debug.Log("Death Request sent.");
    }

    private void SendGameStartRequest(object data){
        SFSInstance.Send(new ExtensionRequest("server.gamestart", new SFSObject(), SFSInstance.LastJoinedRoom));
    }

    private void SendGetScoresRequest(object data){
        SFSInstance.Send(new ExtensionRequest("server.scores", new SFSObject()));
    }
    //end methods for send()
    //methods for responses
    private void SignUpResponse(SFSObject sfsdata) {
        bool success = sfsdata.GetBool("success");
        if (success) {
            OnEvent("register", "Registration successful, please check your email for further instructions.");
        } else {
            OnEvent("register", sfsdata.GetUtfString("errorMessage"));
        }
    }

    private void SpawnResponse ( SFSObject sfsdata ) {
        int id = sfsdata.GetInt ( "player" );
        if ( id != SFSInstance.MySelf.Id ) {
            return;
        }

        float px = sfsdata.GetFloat ( "position.x" );
        float py = sfsdata.GetFloat ( "position.y" );
        float pz = sfsdata.GetFloat ( "position.z" );
        float rx = sfsdata.GetFloat ( "rotation.x" );
        float ry = sfsdata.GetFloat ( "rotation.y" );
        float rz = sfsdata.GetFloat ( "rotation.z" );
        float rw = sfsdata.GetFloat ( "rotation.w" );

        Dictionary<string, float> data = new Dictionary<string, float> ();
        data.Add ( "position.x", px );
        data.Add ( "position.y", py );
        data.Add ( "position.z", pz );
        data.Add ( "rotation.x", rx );
        data.Add ( "rotation.y", ry );
        data.Add ( "rotation.z", rz );
        data.Add ( "rotation.w", rw );

        data.Add("health", sfsdata.GetFloat("health"));
        data.Add("cooldown", sfsdata.GetFloat("cooldown"));
        data.Add("damage", sfsdata.GetFloat("damage"));
        data.Add("range", sfsdata.GetFloat("range"));

        OnEvent ( "spawn", data );
    }

    private void GameListResponse(SFSObject sfsdata) {
        string[] gamelist = sfsdata.GetUtfStringArray("games");
        foreach (string s in gamelist) {
            Debug.Log(s);
            OnEvent("roomadd", s);
        }
    }

    private void HandlePlayerTransformResponse(SFSObject sfsdata) {
        int id = sfsdata.GetInt("player");

        float px = sfsdata.GetFloat("position.x");
        float py = sfsdata.GetFloat("position.y");
        float pz = sfsdata.GetFloat("position.z");
        float rx = sfsdata.GetFloat("rotation.x");
        float ry = sfsdata.GetFloat("rotation.y");
        float rz = sfsdata.GetFloat("rotation.z");
        float rw = sfsdata.GetFloat("rotation.w");

        //hack, needs to be fixed
        if (!GameManager.gameManager.Players.ContainsKey(id) && id != SFSInstance.MySelf.Id) {
            //GameManager.gameManager.AddRemotePlayer(id, "[!]ERROR[!]" + id.ToString());
            //early out
            Debug.LogWarning("Null Player sending messages: " + id.ToString());
            return;
        }

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("id", id);
        data.Add("type", sfsdata.GetUtfString("type"));
        data.Add("position.x", px);
        data.Add("position.y", py);
        data.Add("position.z", pz);
        data.Add("rotation.x", rx);
        data.Add("rotation.y", ry);
        data.Add("rotation.z", rz);
        data.Add("rotation.w", rw);
        OnEvent("transform", data);
    }

    private void HandleProjectileTransformResponse(SFSObject sfsdata) {
        int id = sfsdata.GetInt("networkId");

        float px = sfsdata.GetFloat("position.x");
        float py = sfsdata.GetFloat("position.y");
        float pz = sfsdata.GetFloat("position.z");
        float rx = sfsdata.GetFloat("rotation.x");
        float ry = sfsdata.GetFloat("rotation.y");
        float rz = sfsdata.GetFloat("rotation.z");
        float rw = sfsdata.GetFloat("rotation.w");

        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("id", id);
        data.Add("type", sfsdata.GetUtfString("type"));
        data.Add("position.x", px);
        data.Add("position.y", py);
        data.Add("position.z", pz);
        data.Add("rotation.x", rx);
        data.Add("rotation.y", ry);
        data.Add("rotation.z", rz);
        data.Add("rotation.w", rw);
        OnEvent("transform", data);
    }

    private void TransformResponse ( SFSObject sfsdata ) {
        string type = sfsdata.GetUtfString("type");
        switch (type) {
            case "player":
                HandlePlayerTransformResponse(sfsdata);
                break;

            case "projectile":
                HandleProjectileTransformResponse(sfsdata);
                break;

            default:
                break;
        }
    }

    private void PlayerHitResponse ( SFSObject sfsdata ) {
        float damage = sfsdata.GetFloat ( "damage" );
        int playerid = sfsdata.GetInt ( "player.hit.id" );
        Debug.Log ( "Player hit: " + playerid.ToString () );

        Dictionary<string, object> fdata = new Dictionary<string, object> ();
        if (playerid == -1) {
            fdata.Add("player.hit.id", SFSInstance.MySelf.Id);
        } else {
            fdata.Add("player.hit.id", playerid);
        }
        fdata.Add ( "damage", damage );
        fdata.Add ( "contact.point.x", sfsdata.GetFloat ( "contact.point.x" ) );
        fdata.Add ( "contact.point.y", sfsdata.GetFloat ( "contact.point.y" ) );
        fdata.Add ( "contact.point.z", sfsdata.GetFloat ( "contact.point.z" ) );

        if ( playerid == SFSInstance.MySelf.Id ) {
            OnEvent ( "player.hit", fdata );
        } else {
            OnEvent ( "player.remote.hit", fdata );
        }
    }

    private void DeathResponse ( SFSObject sfsdata ) {
        Debug.Log(sfsdata.GetInt("id"));
        OnEvent ( "player.remote.death", sfsdata.GetInt ( "id" ) );
    }

    private void ShootResponse(SFSObject sfsdata) {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("position.x", sfsdata.GetFloat("position.x"));
        data.Add("position.y", sfsdata.GetFloat("position.y"));
        data.Add("position.z", sfsdata.GetFloat("position.y"));
        data.Add("rotation.x", sfsdata.GetFloat("rotation.x"));
        data.Add("rotation.y", sfsdata.GetFloat("rotation.y"));
        data.Add("rotation.z", sfsdata.GetFloat("rotation.z"));
        data.Add("rotation.w", sfsdata.GetFloat("rotation.w"));
        data.Add("damage", sfsdata.GetFloat("damage"));
        data.Add("speed", sfsdata.GetFloat("speed"));
        data.Add("range", sfsdata.GetFloat("range"));
        data.Add("networkId", sfsdata.GetInt("networkId"));

        //create instance of projectile
        //temporary
        if (sfsdata.GetInt("playerId") != SFSInstance.MySelf.Id) {
            GameManager.gameManager.CreateProjectile(data);
        } else {
            OnEvent("projectile.assign", data);
        }
    }

    private void ScoresResponse(SFSObject sfsdata) {
        Dictionary<string, long> scores = new Dictionary<string, long>();
        Dictionary<string, string> names = new Dictionary<string, string>();
        long myscore = sfsdata.GetLong("my.score");

        int size = sfsdata.GetInt("size");
        for (int i = 0; i < size; ++i) {
            scores.Add("score" + i.ToString(), sfsdata.GetLong("score" + i.ToString()));
            names.Add("player" + i.ToString(), sfsdata.GetUtfString("player" + i.ToString()));
        }

        Dictionary<string, object> package = new Dictionary<string,object>();
        package.Add("scores", scores);
        package.Add("names", names);
        package.Add("my.score", myscore);

        OnEvent("scores", package);
    }

    private void GameStartResponse(SFSObject data) {
        OnEvent("game.start", null);
    }

    private void GameListRemoveResponse(SFSObject data){
        string game = data.GetUtfString("game");
        OnEvent("roomremove", game);
    }

    private void PowerUpResponse(SFSObject data) {
        string type = data.GetUtfString("powerup");
        int id = data.GetInt("playerid");
        Debug.Log("Powerup Response. (ID: " + id.ToString() + "Type: " + type);
        Dictionary<string, object> cdata = new Dictionary<string, object>();
        cdata.Add("powerup", type);
        cdata.Add("playerid", id);
        OnEvent("powerup", cdata);
    }
    //end methods for response

    public void EvacuateTheDanceFloor() {
        listeners.Clear();
    }
}