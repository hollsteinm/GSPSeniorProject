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
}