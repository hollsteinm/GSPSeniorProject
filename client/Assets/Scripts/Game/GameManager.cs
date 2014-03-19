using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum GameType {
    SINGLEPLAYER = 0,
    MULTIPLAYER
};


public class GameManager : MonoBehaviour {

    private static GameManager _gameManager = null;
    private static object mutex = new object();
    private static bool applicationIsQuitting = false;

    private GameManager ( ) {
    }

    public static GameManager gameManager {
        get {
            if ( applicationIsQuitting ) {
                return null;
            }
            lock ( mutex ) {
                if ( _gameManager == null ) {
                    GameObject singleton = new GameObject ();
                    _gameManager = singleton.AddComponent<GameManager> ();
                    singleton.name = "GameManager";
                    DontDestroyOnLoad ( singleton );
                }
                return _gameManager;
            }
        }
    }


    private IClientController client = null;
    private GameType type;

    void FixedUpdate ( ) {
        if ( client != null ) {
            client.Update ();
        }
    }

    public void OnDestroy ( ) {
        applicationIsQuitting = true;
    }

    void OnApplicationQuit ( ) {
        client.Disconnect ();
    }

    public GameType gameType {
        get {
            return type;
        }
        set {
            type = value;
            NotifyGameTypeChange ();
        }
    }

    private void NotifyGameTypeChange ( ) {
        switch ( type ) {
            case GameType.MULTIPLAYER:
                client = SFSClient.Singleton;
                break;
            case GameType.SINGLEPLAYER:
                client = DummyClient.Singleton;
                break;
            default:
                Debug.LogError ( "Unkown gametype value set" );
                break;
        }
    }

    public IClientController ClientController {
        get {
            return client;
        }
    }

    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject> ();

    public Dictionary<int, GameObject> Players {
        get {
            return players;
        }
    }

    public void AddRemotePlayer ( int id ) {
        GameObject newPlayer = (GameObject)Instantiate ( Resources.Load("RemotePlayer") );
        players.Add ( id, newPlayer );
        Debug.Log ( "New Player added: " + id.ToString () );
    }
}
