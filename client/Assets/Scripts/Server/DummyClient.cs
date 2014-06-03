using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DummyClient : IClientController {
    private static object mutex = new object ();
    private static DummyClient _instance;
    private List<IEventListener> listeners = new List<IEventListener> ();
    private string username = "";
    private string server = "";
    private int port = 0;
    private string zone = "";
    private string room = "";

    private DummyClient ( ) {
    }

    public void Logout() {
        Debug.Log("Logged Out");
    }

    public static DummyClient Singleton {
        get {
            if ( _instance == null ) {
                lock ( mutex ) {
                    if ( _instance == null ) {
                        _instance = new DummyClient ();
                    }
                }
            }
            return _instance;
        }
    }


    public void Login ( string username, string password ) {
        this.username = username;
        Debug.Log ( "Dummy login\n\t<username>" + username + "\n\t<password>" + password );
    }

    public void Disconnect ( ) {
        Debug.Log ( "Dummy disconnect" );
    }

    public void Connect ( string server, int port ) {
        this.server = server;
        this.port = port;
        Debug.Log ( "Dummy connect <server><port> ("+server+":"+port.ToString() );
    }

    public void Send ( DataType type, object data ) {
        switch (type) {
            case DataType.CHARMESSAGE:
                ForwardCharMessage(data);
                break;

            case DataType.DEATH:
                ForwardDeath(data);
                break;

            case DataType.FIRE:
                ForwardFire(data);
                break;

            case DataType.JOINGAME:
                ForwardRoomJoinRequest(data);
                break;

            case DataType.MAKEGAME:
                ForwardRoomRequest(data);
                break;

            case DataType.SPAWNED:
                ForwardSpawned(data);
                break;

            case DataType.TRANSFORM:
                ForwardTransform(data);
                break;

            case DataType.SHOOT:
                break;

            case DataType.POWERUP:
                Debug.Log("Current Powerup in Play: " + (string)data);
                break;

            default:
                Debug.LogError("Unhandled DataType: " + type.ToString());
                break;
        }
    }

    public void Update ( ) {
    }

    public string Username {
        get {
            return this.username;
        }
    }

    public string Server {
        get {
            return this.server;
        }
    }

    public int Port {
        get {
            return this.port;
        }
    }

    public string Room {
        get {
            return this.room;    
        }
    }

    public string Zone {
        get {
            return this.zone;
        }
    }

    public void Register ( IEventListener listener ) {
        listeners.Add ( listener );
    }

    public void Unregister ( IEventListener listener ) {
        listeners.Remove ( listener );
    }

    public void OnEvent ( string eventType, object o ) {
        foreach ( IEventListener el in listeners ) {
            el.Notify ( eventType, o );
        }
    }

    //dummy client forwards (like send)
    private void CheckType(object data, System.Type expected){
        if (data.GetType() != expected) {
            Debug.LogError("Wrong Type passed for message!\n[Expected]: " + expected.ToString() + "\n[Actual]: " + data.GetType().ToString());
        }
    }

    private void ForwardTransform(object data) {
        CheckType(data, typeof(Dictionary<string, object>));
    }

    private void ForwardFire(object data) {
        CheckType(data, typeof(Dictionary<string, object>));
        Dictionary<string, object> fdata = data as Dictionary<string, object>;
        
        GameObject other = null;
        try {
            other = GameManager.gameManager.Players[((int)fdata["player.hit.id"])];
        } catch (System.Exception e) {
            //throw it away
        } finally {
            if (other == null) {
                OnEvent("player.hit", data);
            } else {
                OnEvent("player.remote.hit", fdata);
            }
            Debug.Log("Fire message received and handled");
        }
    }

    private void ForwardDeath(object data){
        //typical will be null data, don't need to check
    }

    private void ForwardSpawned(object data){
        //will also not need to send any data
    }

    private void ForwardCharMessage(object data) {
        CheckType(data, typeof(string));
    }

    private void ForwardRoomJoinRequest(object data) {
        CheckType(data, typeof(string));
        if (data.Equals("lobby")) {
            Application.LoadLevel("launch");
        }
    }

    private void ForwardRoomRequest(object data) {
        //will need to check for strings at a later date when we allow users to create rooms with thier own names
    }
    //end dummy client forwards
}